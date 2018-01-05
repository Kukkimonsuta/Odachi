using System.Linq;
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
		[InlineData("en-US", new[] { "en", "US" })]
		[InlineData("enUS", new[] { "en", "US" })]
		[InlineData("c:\\windows\\system32", new[] { "c", "windows", "system32" })]
		[InlineData("fooBarOne", new[] { "foo", "Bar", "One" })]
		[InlineData("fooBAR", new[] { "foo", "BAR" })]
		[InlineData("System32Test", new[] { "System32", "Test" })]
		[InlineData("System32TEST", new[] { "System32", "TEST" })]
		[InlineData("System32test", new[] { "System32test" })]
		public void Splits_words_using_upper_split(string input, string[] expected)
		{
			var actual = input.GetWords(options: WordSplitOptions.SplitOnUpperLetter);

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

		[Theory]
		[InlineData("en-US", new[] { "-" }, new[] { "en", "US" })]
		[InlineData("enUS", new[] { "n", "nU" }, new[] { "e", "US" })]
		[InlineData("enUS", new[] { "nU", "n" }, new[] { "e", "S" })]
		[InlineData("c:\\windows\\system32", new[] { "\\", ":" }, new[] { "c", "", "windows", "system32" })]
		public void Splits_parts(string input, string[] separators, string[] expected)
		{
			var actual = input.GetParts(separators);

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("en-US", new[] { "" }, new[] { "", "e", "n", "-", "U", "S", "" })]
		[InlineData("enUS", new[] { "" }, new[] { "", "e", "n", "U", "S", "" })]
		[InlineData("c:\\windows\\system32", new[] { "" }, new[] { "", "c", ":", "\\", "w", "i", "n", "d", "o", "w", "s", "\\", "s", "y", "s", "t", "e", "m", "3", "2", "" })]
		public void Splits_parts_using_empty_string(string input, string[] separators, string[] expected)
		{
			var actual = input.GetParts(separators);

			Assert.Equal(expected, actual);
		}

		[Theory]
		[InlineData("en-US", new[] { "en-US" })]
		[InlineData("\r\nen-US", new[] { "", "en-US" })]
		[InlineData("en-US\r\n", new[] { "en-US", "" })]
		[InlineData("\nen-US", new[] { "", "en-US" })]
		[InlineData("en-US\n", new[] { "en-US", "" })]
		[InlineData("\r\n\nen-US\n", new[] { "", "", "en-US", "" })]
		[InlineData("\r\n\nen\r-\nUS\n", new[] { "", "", "en\r-", "US", "" })]
		public void Splits_lines(string input, string[] expected)
		{
			var actual = input.GetLines();

			Assert.Equal(expected, actual);
		}
	}
}
