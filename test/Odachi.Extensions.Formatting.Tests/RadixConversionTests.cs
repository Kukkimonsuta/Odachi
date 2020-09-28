using System;
using System.Linq;
using Xunit;

namespace Odachi.Extensions.Formatting.Tests
{
	public class RadixConversionTests
	{
		[Theory]
		[InlineData(new byte[] { }, "")]
		[InlineData(new byte[] { 0x00 }, "0")]
		[InlineData(new byte[] { 0x01 }, "1")]
		[InlineData(new byte[] { 0x02 }, "10")]
		[InlineData(new byte[] { 0x03 }, "11")]
		public void ToString_base2(byte[] input, string expected)
		{
			var actual = RadixConversion.ToString(input, "01");

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(new byte[] { }, "")]
		[InlineData(new byte[] { 0x00 }, "0")]
		[InlineData(new byte[] { 0x01 }, "1")]
		[InlineData(new byte[] { 0x02 }, "2")]
		[InlineData(new byte[] { 0x03 }, "3")]
		[InlineData(new byte[] { 0x0a }, "10")]
		[InlineData(new byte[] { 0x0f }, "15")]
		[InlineData(new byte[] { 0x10 }, "16")]
		[InlineData(new byte[] { 0xff }, "255")]
		public void ToString_base10(byte[] input, string expected)
		{
			var actual = RadixConversion.ToString(input, "0123456789");

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData(new byte[] { }, "")]
		[InlineData(new byte[] { 0x00 }, "0")]
		[InlineData(new byte[] { 0x01 }, "1")]
		[InlineData(new byte[] { 0x02 }, "2")]
		[InlineData(new byte[] { 0x03 }, "3")]
		[InlineData(new byte[] { 0x0a }, "a")]
		[InlineData(new byte[] { 0x0f }, "f")]
		[InlineData(new byte[] { 0x10 }, "10")]
		[InlineData(new byte[] { 0xff }, "ff")]
		public void ToString_base16(byte[] input, string expected)
		{
			var actual = RadixConversion.ToString(input, "0123456789abcdef");

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("", new byte[] { })]
		[InlineData("0", new byte[] { 0x00 })]
		[InlineData("1", new byte[] { 0x01 })]
		[InlineData("2", new byte[] { 0x02 })]
		[InlineData("3", new byte[] { 0x03 })]
		[InlineData("10", new byte[] { 0x0a })]
		[InlineData("15", new byte[] { 0x0f })]
		[InlineData("16", new byte[] { 0x10 })]
		[InlineData("255", new byte[] { 0xff })]
		public void FromString_base10(string input, byte[] expected)
		{
			var actual = RadixConversion.FromString(input, "0123456789");
			
			Assert.Equal(expected, actual);
		}
	}
}
