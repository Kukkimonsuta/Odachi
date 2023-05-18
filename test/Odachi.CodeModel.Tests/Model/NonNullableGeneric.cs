using System;
using System.Collections.Generic;
using System.Text;
using Odachi.Extensions.Primitives;

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

	public class NullableOneOfMethodReturnType
	{
		public OneOf<string, int>? Nullable()
		{
			return 0;
		}
		public OneOf<string, int> NonNullable()
		{
			return 0;
		}
	}
}
