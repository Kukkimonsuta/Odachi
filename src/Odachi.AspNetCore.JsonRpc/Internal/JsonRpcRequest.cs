using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Odachi.AspNetCore.JsonRpc.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.AspNetCore.JsonRpc.Internal
{
	public class JsonRpcRequest
	{
		public JsonRpcRequest(object id, string method, JToken @params, JsonSerializer serializer)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));
			if (method.Length <= 0)
				throw new ArgumentException("Argument must not be an empty string", nameof(method));
			if (serializer == null)
				throw new ArgumentNullException(nameof(serializer));

			Id = id;
			Method = method;
			_params = @params;

			_serializer = serializer;
		}

		public object Id { get; }
		public string Method { get; }
		private JToken _params;
		private JsonSerializer _serializer;

		public bool IsNotification => Id == null;
		public bool IsIndexed => _params?.Type == JTokenType.Array;

		public object GetParameter(int index, JsonMappedType type, object @default)
		{
			if (_params == null)
				return @default;

			if (_params.Type != JTokenType.Array)
				throw new InvalidOperationException("Params are not indexed");

			var paramsArray = ((JArray)_params);

			if (paramsArray.Count <= index)
				return @default;

			return paramsArray[index].ToObject(type.NetType);
		}
		public object GetParameter(string name, JsonMappedType type, object @default)
		{
			if (_params == null)
				return @default;

			if (_params.Type != JTokenType.Object)
				throw new InvalidOperationException("Params are not named");

			JToken value;
			if (!((JObject)_params).TryGetValue(name, out value))
				return @default;

			return value.ToObject(type.NetType, _serializer);
		}

		#region Static members

		private static async Task<JsonReader> CreateReaderAsync(HttpContext httpContext, JsonSerializer serializer)
		{
			if (!httpContext.Request.HasFormContentType)
			{
				return new JsonTextReader(new StreamReader(httpContext.Request.Body));
			}

			var form = await httpContext.Request.ReadFormAsync();

			serializer.Converters.Add(new StreamReferenceConverter(form));

			return new JsonTextReader(new StringReader(form.Single().Value));
		}

		public static async Task<JsonRpcRequest> CreateAsync(HttpContext httpContext, JsonSerializer serializer)
		{
			using (var reader = await CreateReaderAsync(httpContext, serializer))
			{
				var requestJsonToken = JToken.ReadFrom(reader);
				if (requestJsonToken.Type != JTokenType.Object)
					throw new JsonRpcException(JsonRpcError.INVALID_REQUEST, "Invalid request (wrong root type)");

				var requestJson = (JObject)requestJsonToken;

				JToken idJson;
				object id = null;
				if (requestJson.TryGetValue("id", out idJson))
					id = idJson.Value<object>();

				JToken methodJson;
				string method;
				if (!requestJson.TryGetValue("method", out methodJson) || methodJson.Type != JTokenType.String)
					throw new JsonRpcException(JsonRpcError.INVALID_REQUEST, "Invalid 'method' (missing or wrong type)");
				method = methodJson.Value<string>();

				JToken paramsJson = null;
				if (!requestJson.TryGetValue("params", out paramsJson))
					paramsJson = null;
				if (paramsJson != null && paramsJson.Type != JTokenType.Object && paramsJson.Type != JTokenType.Array)
					throw new JsonRpcException(JsonRpcError.INVALID_REQUEST, "Invalid 'params' (wrong type)");

				return new JsonRpcRequest(id, method, paramsJson, serializer);
			}
		}

		#endregion
	}
}
