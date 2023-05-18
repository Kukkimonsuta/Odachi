#nullable enable

using System;

namespace Odachi.CodeModel.Tests.Model;

public class DecimalArrayNrtOn
{
	public decimal[] Arr { get; set; } = Array.Empty<decimal>();
}

public class NullableDecimalArrayNrtOn
{
	public decimal?[] Arr { get; set; } = Array.Empty<decimal?>();
}
