using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Odachi.JsonRpc.Common;
using Odachi.JsonRpc.Common.Converters;
using Odachi.Abstractions;

namespace Odachi.JsonRpc.Server
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
		private readonly JToken _params;
		private readonly JsonSerializer _serializer;

		public bool IsNotification => Id == null;
		public bool IsIndexed => _params?.Type == JTokenType.Array;

		public object GetParameter(int index, Type type, object @default)
		{
			if (_params == null)
				return @default;

			if (_params.Type != JTokenType.Array)
				throw new InvalidOperationException("Params are not indexed");

			var paramsArray = (JArray)_params;

			if (paramsArray.Count <= index)
				return @default;

			return paramsArray[index].ToObject(type);
		}
		public object GetParameter(string name, Type type, object @default)
		{
			if (_params == null)
				return @default;

			if (_params.Type != JTokenType.Object)
				throw new InvalidOperationException("Params are not named");

			if (!((JObject)_params).TryGetValue(name, out var value))
				return @default;

			return value.ToObject(type, _serializer);
		}
	}
}
