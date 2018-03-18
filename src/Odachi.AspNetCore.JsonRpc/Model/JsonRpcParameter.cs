using System;

namespace Odachi.AspNetCore.JsonRpc.Model
{
	public class JsonRpcParameter
	{
		public JsonRpcParameter(string name, Type type, bool isInternal, bool isOptional, object defaultValue)
		{
			Name = name;
			Type = type;
			IsInternal = isInternal;
			IsOptional = isOptional;
			DefaultValue = defaultValue;
		}

		public string Name { get; }
		public Type Type { get; }
		public bool IsInternal { get; }
		public bool IsOptional { get; }
		public object DefaultValue { get; }
	}
}
