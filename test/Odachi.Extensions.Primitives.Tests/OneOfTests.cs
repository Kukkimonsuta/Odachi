using System;
using Xunit;

namespace Odachi.Extensions.Primitives.Tests
{
	public class OneOfTests
	{
		private OneOf<string, int> Convert(OneOf<string, int> source)
		{
			return source.Match<OneOf<string, int>>(
				(str) => int.Parse(str),
				(num) => num.ToString()
			);
		}

		[Fact]
		public void Match_one_of_empty()
		{
			var actual = default(OneOf<string, int>);

			Assert.Equal(0, actual.Index);
			Assert.Equal(true, actual.IsEmpty);
			Assert.Equal(false, actual.Is1);
			Assert.Equal(false, actual.Is2);
			Assert.Throws<InvalidOperationException>(() => actual.As1);
			Assert.Throws<InvalidOperationException>(() => actual.As2);
		}

		[Fact]
		public void Match_one_of_string()
		{
			var actual = Convert(10);

			Assert.Equal(1, actual.Index);
			Assert.Equal(false, actual.IsEmpty);
			Assert.Equal(true, actual.Is1);
			Assert.Equal(false, actual.Is2);
			Assert.Equal("10", actual.As1);
			Assert.Throws<InvalidOperationException>(() => actual.As2);
		}

		[Fact]
		public void Match_one_of_number()
		{
			var actual = Convert("10");

			Assert.Equal(2, actual.Index);
			Assert.Equal(false, actual.IsEmpty);
			Assert.Equal(false, actual.Is1);
			Assert.Equal(true, actual.Is2);
			Assert.Throws<InvalidOperationException>(() => actual.As1);
			Assert.Equal(10, actual.As2);
		}
	}
}
