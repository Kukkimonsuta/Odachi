using Newtonsoft.Json;
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
			Assert.True(actual.IsEmpty);
			Assert.False(actual.Is1);
			Assert.False(actual.Is2);
			Assert.Throws<InvalidOperationException>(() => actual.As1);
			Assert.Throws<InvalidOperationException>(() => actual.As2);
		}

		[Fact]
		public void Match_one_of_string()
		{
			var actual = Convert(10);

			Assert.Equal(1, actual.Index);
			Assert.False(actual.IsEmpty);
			Assert.True(actual.Is1);
			Assert.False(actual.Is2);
			Assert.Equal("10", actual.As1);
			Assert.Throws<InvalidOperationException>(() => actual.As2);
		}

		[Fact]
		public void Match_one_of_number()
		{
			var actual = Convert("10");

			Assert.Equal(2, actual.Index);
			Assert.False(actual.IsEmpty);
			Assert.False(actual.Is1);
			Assert.True(actual.Is2);
			Assert.Throws<InvalidOperationException>(() => actual.As1);
			Assert.Equal(10, actual.As2);
		}

		[Fact]
		public void Newtonsoft_json_roundtrip()
		{
			var expected = new OneOf<string, int>("test");
			var serialized = JsonConvert.SerializeObject(expected);
			var actual = JsonConvert.DeserializeObject<OneOf<string, int>>(serialized);

			// hotfix for removal of default values of selected option; in future should be replaced with custom serializer
			Assert.Equal("{\"Index\":1,\"Option1\":\"test\",\"Option2\":0}", serialized);
			Assert.False(expected.IsEmpty);
			Assert.Equal(expected.Index, actual.Index);
			Assert.Equal(expected.Option1, actual.Option1);
			Assert.Equal(expected.Option2, actual.Option2);
		}

		[Fact]
		public void Wrapped_values_can_be_checked_for_equality()
		{
			var value = new OneOf<string, int>("ten");
			Assert.True(new OneOf<string, int>("ten") == value);
			Assert.False(new OneOf<string, int>("xyz") == value);
		}

		[Fact]
		public void Can_use_classic_switch()
		{
			var actual = new OneOf<string, int>("foo");

			switch (actual.Value)
			{
				case string str:
					Assert.Equal("foo", str);
					break;

				default:
					Assert.True(false);
					break;
			}
		}
	}
}
