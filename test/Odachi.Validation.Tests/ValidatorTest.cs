using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Odachi.Extensions.Primitives;

namespace Odachi.Validation
{
	public class ValidatorTest
	{
		private OneOf<string, ValidationState> TestBusinessMethod(string foo)
		{
			var validator = new Validator();

			if (foo.Length <= 0)
				return validator.Error(nameof(foo), "Required field").State;

			return $"bar_{foo}";
		}

		[Fact]
		public void Validation_success_result()
		{
			var result = TestBusinessMethod("test");

			result.Match(
				value => Assert.Equal("bar_test", value),
				validation => Assert.True(false)
			);
		}

		[Fact]
		public void Validation_error_result()
		{
			var result = TestBusinessMethod("");

			result.Match(
				value => Assert.True(false),
				validation =>
				{
					Assert.NotNull(validation);
					Assert.Equal("Required field", validation.GetErrorText("foo"));
					Assert.Null(validation.GetErrorText("nonexistant-field"));
				}
			);
		}

		[Fact]
		public void Validation_or_value_is_serializable_value()
		{
			var expected = new OneOf<string, ValidationState>("test");

			var serialized = JsonConvert.SerializeObject(expected);
			var deserialized = JsonConvert.DeserializeObject<OneOf<string, ValidationState>>(serialized);

			Assert.True(deserialized.Is1);
			Assert.Equal(expected.As1, deserialized.As1);
		}

		[Fact]
		public void Validation_or_value_is_serializable_validation()
		{
			var expected = new OneOf<string, ValidationState>(
				new Validator()
					.Error("SomeField", "Required field")
					.State
			);

			var serialized = JsonConvert.SerializeObject(expected);
			var deserialized = JsonConvert.DeserializeObject<OneOf<string, ValidationState>>(serialized);

			Assert.True(deserialized.Is2);
			Assert.Equal(expected.As2.Count, deserialized.As2.Count);
			Assert.Equal(expected.As2.GetErrorText("SomeField"), deserialized.As2.GetErrorText("SomeField"));
		}
	}
}
