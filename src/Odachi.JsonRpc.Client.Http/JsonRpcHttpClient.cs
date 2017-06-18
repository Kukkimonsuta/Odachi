using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using Odachi.Abstractions;
using Microsoft.Extensions.Logging;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Odachi.JsonRpc.Client.Http
{
	public class JsonRpcHttpClient : JsonRpcClient
	{
		public JsonRpcHttpClient(string endpoint, ILogger<JsonRpcHttpClient> logger = null)
			: base(logger: logger)
		{
			if (endpoint == null)
				throw new ArgumentNullException(nameof(endpoint));

			Endpoint = endpoint;

			_httpClient = new HttpClient();
			_logger = logger;
		}

		private readonly HttpClient _httpClient;
		private readonly ILogger _logger;

		public string Endpoint { get; }

		public HttpRequestHeaders DefaultRequestHeaders
		{
			get => _httpClient.DefaultRequestHeaders;
		}

		private MultipartFormDataContent CreateRequestContent(JsonRpcRequest request)
		{
			var content = new MultipartFormDataContent();

			void HandleReference(string path, IStreamReference reference)
			{
				content.Add(new StreamContent(reference.OpenReadStream()), path, reference.Name);
			}

			try
			{
				var requestString = SerializeRequest(request, HandleReference);

				if (_logger != null && _logger.IsEnabled(LogLevel.Debug))
				{
					_logger.LogDebug($"Sending request: {requestString}");
				}

				content.Add(new StringContent(requestString), "json-request");

				return content;
			}
			catch (Exception)
			{
				content.Dispose();
				throw;
			}
		}

		private async Task<JsonRpcResponse> CreateJsonRpcResponseAsync(HttpContent content)
		{
			switch (content)
			{
				case StreamContent streamContent:
					var responseString = await streamContent.ReadAsStringAsync();

					if (_logger != null && _logger.IsEnabled(LogLevel.Debug))
					{
						_logger.LogDebug($"Received response: {responseString}");
					}

					return DeserializeResponse(responseString);

				default:
					throw new InvalidOperationException($"Undefined behavior for content '{content.GetType().FullName}'");
			}
		}

		protected override async Task<JsonRpcResponse> CallInternalAsync(JsonRpcRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			var requestContent = CreateRequestContent(request);

			using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, Endpoint))
			{
				httpRequest.Content = requestContent;

				using (var httpResponse = await _httpClient.SendAsync(httpRequest))
				{
					httpResponse.EnsureSuccessStatusCode();

					return await CreateJsonRpcResponseAsync(httpResponse.Content);
				}
			}
		}

		#region IDisposable

		private bool _disposed = false;

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				if (_httpClient != null)
				{
					_httpClient.Dispose();
				}
			}

			base.Dispose(disposing);

			_disposed = true;
		}

		#endregion
	}
}
