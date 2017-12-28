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

#pragma warning disable CS0618 // Type or member is obsolete
			void HandleReference(string path, IStreamReference reference)
#pragma warning restore CS0618 // Type or member is obsolete
			{
				content.Add(new StreamContent(reference.OpenReadStream()), path, reference.Name);
			}
			void HandleBlob(string path, IBlob blob)
			{
				content.Add(new StreamContent(blob.OpenRead()), path, blob.Name);
			}

			try
			{
				var requestString = SerializeRequest(request, HandleReference, HandleBlob);

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
			switch (content.Headers.ContentType.MediaType)
			{
				case "multipart/form-data":
					throw new NotSupportedException();

				default:
					var responseString = await content.ReadAsStringAsync();

					if (_logger != null && _logger.IsEnabled(LogLevel.Debug))
					{
						_logger.LogDebug($"Received response: {responseString}");
					}

					return DeserializeResponse(responseString);
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
