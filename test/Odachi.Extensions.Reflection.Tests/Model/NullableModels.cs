using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace Odachi.Extensions.Reflection.Tests.Model
{
	public class NullableBasicModel
	{
		public string NonNullable { get; set; } = "";
		public string? Nullable { get; set; }
		public int Value { get; set; }
		public int? NullableValue { get; set; }

		public string NonNullableResult()
		{
			return "";
		}

		public string? NullableResult()
		{
			return null;
		}

		public string? NullableResultWithNonNullableParam(string cookie)
		{
			return null;
		}
	}

	public class NullableNestedModel
	{
		public string NonNullable { get; set; } = "";
		public string? Nullable { get; set; }
		public int Value { get; set; }
		public int? NullableValue { get; set; }

		public class Nested
		{
			public string NonNullable { get; set; } = "";
			public string? Nullable { get; set; }
			public int Value { get; set; }
			public int? NullableValue { get; set; }
		}
	}

	public class NullableGenericModel
	{
		public List<string> NonNullable { get; set; } = new List<string>();
		public List<string?> Nullable { get; set; } = new List<string?>();
	}

	public class NullableGenericComplex
	{
		public List<Dictionary<List<string?>, string>> Foo { get; set; } = new List<Dictionary<List<string?>, string>>();
	}

	public class NullableArray
	{
		public string?[] Nullable { get; set; } = Array.Empty<string>();
		public string[] NonNullable { get; set; } = Array.Empty<string>();
	}

	public class NullableMethodParameters
	{
		public void Nullable(string?[] param) { }
		public void NonNullable(string[] param) { }
	}
}
