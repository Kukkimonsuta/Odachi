using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.IO;
using Newtonsoft.Json.Serialization;
using Odachi.JsonRpc.Common.Internal;
using Odachi.JsonRpc.Common.Converters;
using Odachi.AspNetCore.JsonRpc.Converters;
using System.Diagnostics;

namespace Odachi.JsonRpc.Client
{
	public class JsonRpcClient : IDisposable
	{
		public JsonRpcClient(string endpoint)
		{
			if (endpoint == null)
				throw new ArgumentNullException(nameof(endpoint));

			_endpoint = endpoint;

			_client = new HttpClient();
			_serializerSettings = new JsonSerializerSettings()
			{
				ContractResolver = new DefaultContractResolver
				{
					NamingStrategy = new MultiWordCamelCaseNamingStrategy(true, false)
				},
				NullValueHandling = NullValueHandling.Ignore,
				TypeNameHandling = TypeNameHandling.None,
			};
			_serializerSettings.Converters.Add(new PageConverter());
			_serializerSettings.Converters.Add(new EntityReferenceConverter());
			_serializerSettings.Converters.Add(new StreamReferenceConverter());
			_serializer = JsonSerializer.Create(_serializerSettings);
		}

		private int _id;
		private string _endpoint;
		private HttpClient _client;
		private JsonSerializerSettings _serializerSettings;
		private JsonSerializer _serializer;

		public bool UseJsonRpcConstant { get; set; } = false;

		private MultipartFormDataContent CreateRequestContent(string method, object @params)
		{
			var id = Interlocked.Increment(ref _id);

			var request = new JsonRpcRequest(id, method, @params);

			return CreateRequestContent(request);
		}
		private MultipartFormDataContent CreateRequestContent(JsonRpcRequest request)
		{
			var content = new MultipartFormDataContent();
			try
			{
				JObject jObject;
				using (new StreamReferenceHandler((path, reference) => content.Add(new StreamContent(reference.OpenReadStream()), path, reference.Name)))
				{
					jObject = JObject.FromObject(request, _serializer);
				}

				// append jsonrpc constant if enabled
				if (UseJsonRpcConstant)
				{
					jObject.AddFirst(new JProperty("jsonrpc", "2.0"));
				}

				// add to content
				{
					using (var stringWriter = new StringWriter())
					{
						using (var jsonWriter = new JsonTextWriter(stringWriter))
						{
							jObject.WriteTo(jsonWriter);
						}

						stringWriter.Flush();

						content.Add(new StringContent(stringWriter.GetStringBuilder().ToString()), "json-request");
					}
				}

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

					return CreateJsonRpcResponse(responseString);

				default:
					throw new InvalidOperationException($"Undefined behavior for content '{content.GetType().FullName}'");
			}
		}
		private JsonRpcResponse CreateJsonRpcResponse(string responseString)
		{
			var jObject = JObject.Parse(responseString);

			var hasId = jObject.TryGetValue("id", out var jId);
			var hasResult = jObject.TryGetValue("result", out var jResult);
			var hasError = jObject.TryGetValue("error", out var jError);

			if (!hasResult && !hasError)
				throw new JsonRpcException("Malformed response: must define either 'result' or 'error'");
			if (hasResult && hasError)
				throw new JsonRpcException("Malformed response: cannot define both 'result' and 'error'");
			if (hasResult && !hasId)
				throw new JsonRpcException("Malformed response: missing 'id'");

			return new JsonRpcResponse(jId, jResult, jError);
		}

		public async Task<JsonRpcResponse> CallAsync(string method, object @params)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, _endpoint))
			{
				httpRequest.Content = CreateRequestContent(method, @params);

				using (var httpResponse = await _client.SendAsync(httpRequest))
				{
					httpResponse.EnsureSuccessStatusCode();

					return await CreateJsonRpcResponseAsync(httpResponse.Content);
				}
			}
		}

		public async Task<T> CallAsync<T>(string method, object @params)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			var response = await CallAsync(method, @params);

			if (response.Error != null)
				throw new JsonRpcException($"Rpc call failed: {response.Error}");

			return response.Result.ToObject<T>(_serializer);
		}

		#region IDisposable

		private bool _disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				if (_client != null)
				{
					_client.Dispose();
					_client = null;
				}
			}

			_disposed = true;
		}

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion
	}
}
