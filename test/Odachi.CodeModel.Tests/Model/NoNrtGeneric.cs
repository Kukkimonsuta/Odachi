using System;
using System.Collections.Generic;
using System.Text;
using Odachi.Extensions.Primitives;

#nullable disable

namespace Odachi.CodeModel.Tests.Model
{
	public class NoNrtGenericItem<T1, T2> { }

	public class NoNrtGeneric
	{
		public List<NonNullableGenericItem<List<string>, string>> Foo { get; } = new();
	}

	public class NoNrtMethodParameters
	{
		public void Method(string[] param) { }
	}

	public class NoNrtOneOfMethodReturnType
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
