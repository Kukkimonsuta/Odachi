using System;

namespace Odachi.JsonRpc.Server.Model
{
	public enum JsonRpcParameterSource
	{
		Request = 1,
		AppServices = 2,
		RpcServices = 3,
	}

	public class JsonRpcParameter
	{
		public JsonRpcParameter(string name, Type type, JsonRpcParameterSource source, bool isOptional, object defaultValue)
		{
			Name = name;
			Type = type;
			Source = source;
			IsOptional = isOptional;
			DefaultValue = defaultValue;
		}

		public string Name { get; }
		public Type Type { get; }
		public JsonRpcParameterSource Source { get; }
		public bool IsOptional { get; }
		public object DefaultValue { get; }
	}
}
