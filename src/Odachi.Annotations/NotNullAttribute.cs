using System;

namespace Odachi.Annotations
{
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false)]
	public class NotNullAttribute : Attribute
	{
		public NotNullAttribute()
		{
		}
	}
}
