using Xunit;

namespace Odachi.Extensions.Formatting.Tests
{
	public class StringExtensionsTests
	{
		[Theory]
		[InlineData("en-US", new[] { "en", "US" })]
		[InlineData("enUS", new[] { "enUS" })]
		[InlineData("c:\\windows\\system32", new[] { "c", "windows", "system32" })]
		public void Splits_words(string input, string[] expected)
		{
			var actual = input.GetWords();

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("en-US", "En-US")]
		[InlineData("enUS", "EnUS")]
		[InlineData("c:\\windows\\system32", "C:\\Windows\\System32")]
		[InlineData("description_cs", "Description_Cs")]
		[InlineData("description_CS", "Description_CS")]
		public void Pascalizes_input(string input, string expected)
		{
			var actual = input.ToPascalInvariant();

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("en-US", "en-us")]
		[InlineData("enUS", "enUS")]
		[InlineData("c:\\windows\\system32", "c:\\windows\\system32")]
		[InlineData("description_cs", "description_cs")]
		[InlineData("description_CS", "description_cs")]
		public void Camelizes_input(string input, string expected)
		{
			var actual = input.ToCamelInvariant();

			Assert.Equal(expected, actual);
		}
	}
}
