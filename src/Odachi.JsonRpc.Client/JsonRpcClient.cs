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
using Microsoft.Extensions.Logging;
using Odachi.Abstractions;

namespace Odachi.JsonRpc.Client
{
	public interface IRequestFilter
	{
		Task<JsonRpcRequest> Process(JsonRpcClient client, JsonRpcRequest request);
	}

	public interface IResponseFilter
	{
		Task<JsonRpcResponse> Process(JsonRpcClient client, JsonRpcRequest request, JsonRpcResponse response);
	}

	public abstract class JsonRpcClient : IRpcClient, IDisposable
	{
		public JsonRpcClient(ILogger logger = null)
		{
			Serializer = JsonSerializer.Create();
			Serializer.NullValueHandling = NullValueHandling.Ignore;
			Serializer.TypeNameHandling = TypeNameHandling.None;
			Serializer.ContractResolver = new DefaultContractResolver()
			{
				NamingStrategy = new MultiWordCamelCaseNamingStrategy(true, false)
			};
			Serializer.Converters.Add(new PageConverter());
			Serializer.Converters.Add(new EntityReferenceConverter());
			Serializer.Converters.Add(new StreamReferenceConverter());

			RequestFilters = new List<IRequestFilter>();
			ResponseFilters = new List<IResponseFilter>();
		}

		public JsonSerializer Serializer { get; }

		public IList<IRequestFilter> RequestFilters { get; }
		public IList<IResponseFilter> ResponseFilters { get; }

		public bool UseJsonRpcConstant { get; set; } = false;

		protected string SerializeRequest(JsonRpcRequest request, Action<string, IStreamReference> streamReferenceHandler)
		{
			if (request == null)
				throw new ArgumentNullException(nameof(request));
			if (streamReferenceHandler == null)
				throw new ArgumentNullException(nameof(streamReferenceHandler));

			JObject jObject;
			using (new StreamReferenceHandler(streamReferenceHandler))
			{
				jObject = JObject.FromObject(request, Serializer);
			}

			// append jsonrpc constant if enabled
			if (UseJsonRpcConstant)
			{
				jObject.AddFirst(new JProperty("jsonrpc", "2.0"));
			}

			// serialize
			using (var writer = new StringWriter())
			{
				using (var jsonWriter = new JsonTextWriter(writer))
				{
					jObject.WriteTo(jsonWriter);
				}
				writer.Flush();

				return writer.GetStringBuilder().ToString();
			}
		}

		protected JsonRpcResponse DeserializeResponse(string response)
		{
			var jObject = JObject.Parse(response);

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

		protected abstract Task<JsonRpcResponse> CallInternalAsync(JsonRpcRequest request);

		public async Task<JsonRpcResponse> CallAsync(JsonRpcRequest request, bool throwOnError = true)
		{
			foreach (var filter in RequestFilters)
			{
				request = await filter.Process(this, request);
			}

			var response = await CallInternalAsync(request);

			foreach (var filter in ResponseFilters)
			{
				response = await filter.Process(this, request, response);
			}

			if (throwOnError && response.Error != null)
			{
				throw new JsonRpcException($"Rpc call failed: {response.Error}");
			}

			return response;
		}

		public Task<JsonRpcResponse> CallAsync(string method, object @params, bool throwOnError = true)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			var request = JsonRpcRequest.Create(method, @params);

			return CallAsync(request, throwOnError: throwOnError);
		}

		public async Task<T> CallAsync<T>(string method, object @params)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			var response = await CallAsync(method, @params, throwOnError: true);

			return response.Result.ToObject<T>(Serializer);
		}

		#region IRpcClient

		Task IRpcClient.CallAsync<TParams>(string service, string method, TParams @params)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			return CallAsync(service == null ? method : $"{service}.{method}", @params);
		}

		Task<TResult> IRpcClient.CallAsync<TResult, TParams>(string service, string method, TParams @params)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			return CallAsync<TResult>(service == null ? method : $"{service}.{method}", @params);
		}

		#endregion

		#region IDisposable

		private bool _disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
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
