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
		public RpcMethodAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; set; }
	}
}
