using System;

namespace Odachi.Annotations
{
	/// <summary>
	/// Denotes whether value / parameter / field / property can be null. Please note that this attribute is experimental and intended use
	/// is to prepare for https://github.com/dotnet/csharplang/issues/36
	/// </summary>
	[AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class NullabilityAttribute : Attribute
	{
		public NullabilityAttribute(params bool[] values)
		{
			if (values == null)
				throw new ArgumentNullException(nameof(values));
			if (values.Length <= 0)
				throw new ArgumentOutOfRangeException(nameof(values));

			Values = values;
		}

		public bool[] Values { get; set; }
	}
}
