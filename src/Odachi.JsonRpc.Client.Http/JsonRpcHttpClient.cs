#nullable enable

using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Odachi.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Odachi.Extensions.Formatting;

namespace Odachi.JsonRpc.Client.Http
{
	public class JsonRpcHttpClient : JsonRpcClient
	{
		// TODO: remove on next major
		// preserve legacy behavior
		[ActivatorUtilitiesConstructor]
		public JsonRpcHttpClient(string endpoint, ILogger<JsonRpcHttpClient>? logger = null)
			: this(endpoint, new HttpClient(), logger: logger)
		{
		}
		public JsonRpcHttpClient(string endpoint, HttpClient httpClient, ILogger<JsonRpcHttpClient>? logger = null)
			: base(logger: logger)
		{
			if (endpoint == null)
				throw new ArgumentNullException(nameof(endpoint));

			Endpoint = endpoint;

			_httpClient = httpClient;
			_logger = logger;
		}

		private readonly HttpClient _httpClient;
		private readonly ILogger? _logger;

		public string Endpoint { get; }

		public string? QueryRequestIdKey { get; set; } = "i";
		public string? QueryRequestMethodKey { get; set; } = "m";

		public HttpRequestHeaders DefaultRequestHeaders
		{
			get => _httpClient.DefaultRequestHeaders;
		}

		private MultipartFormDataContent CreateRequestContent(JsonRpcRequest request)
		{
			var content = new MultipartFormDataContent();

			void HandleBlob(string path, IBlob blob)
			{
				content.Add(new StreamContent(blob.OpenRead()), path, blob.Name);
			}

			try
			{
				var requestString = SerializeRequest(request, HandleBlob);

				if (_logger != null && _logger.IsEnabled(LogLevel.Debug))
				{
					_logger.LogDebug("Sending request: {RequestString}", requestString);
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
			switch (content.Headers.ContentType?.MediaType)
			{
				case "multipart/form-data":
					throw new NotSupportedException();

				default:
					var responseString = await content.ReadAsStringAsync();

					if (_logger != null && _logger.IsEnabled(LogLevel.Debug))
					{
						_logger.LogDebug("Received response: {ResponseString}", responseString);
					}

					return DeserializeResponse(responseString);
			}
		}

		protected override async Task<JsonRpcResponse> CallInternalAsync(JsonRpcRequest request)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));

			var requestContent = CreateRequestContent(request);

			var uri = new UriBuilder(Endpoint);
			if (!string.IsNullOrEmpty(QueryRequestIdKey) && request.Id != null)
			{
				uri.AppendQuery(QueryRequestIdKey!, value: request.Id.ToString());
			}
			if (!string.IsNullOrEmpty(QueryRequestMethodKey))
			{
				uri.AppendQuery(QueryRequestMethodKey!, value: request.Method);
			}

			using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, uri.ToString()))
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
