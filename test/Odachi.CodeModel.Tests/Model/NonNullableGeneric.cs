using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace Odachi.CodeModel.Tests.Model
{
	public class NonNullableGenericItem<T1, T2> { }

	public class NonNullableGeneric
	{
		public List<NonNullableGenericItem<List<string?>, string>> Foo { get; } = new List<NonNullableGenericItem<List<string?>, string>>();
	}

	public class NullableMethodParameters
	{
		public void Nullable(string?[] param) { }
		public void NonNullable(string[] param) { }
	}
}
