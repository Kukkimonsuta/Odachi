using System;

namespace Odachi.Annotations
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class RpcMethod : Attribute
	{
		public RpcMethod()
		{
		}
		public RpcMethod(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}
