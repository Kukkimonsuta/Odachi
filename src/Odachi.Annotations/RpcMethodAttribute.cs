using System;

namespace Odachi.Annotations
{
	/// <summary>
	/// Marks methods for rpc registration.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class RpcMethodAttribute : Attribute
	{
		public RpcMethodAttribute()
		{
		}
		public RpcMethodAttribute(string? moduleName, string? methodName)
		{
			ModuleName = moduleName;
			MethodName = methodName;
		}

		public string? ModuleName { get; set; }
		public string? MethodName { get; set; }
	}
}
