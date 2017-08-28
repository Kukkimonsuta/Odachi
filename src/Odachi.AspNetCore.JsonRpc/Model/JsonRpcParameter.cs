namespace Odachi.AspNetCore.JsonRpc.Model
{
	public class JsonRpcParameter
	{
		public JsonRpcParameter(string name, JsonMappedType type, bool isInternal, bool isOptional, object defaultValue)
		{
			Name = name;
			Type = type;
			IsInternal = isInternal;
			IsOptional = isOptional;
			DefaultValue = defaultValue;
		}

		public string Name { get; }
		public JsonMappedType Type { get; }
		public bool IsInternal { get; }
		public bool IsOptional { get; }
		public object DefaultValue { get; }
	}
}
