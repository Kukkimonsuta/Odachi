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
					Assert.Equal("Required field", validation.GetError("foo"));
					Assert.Null(validation.GetError("nonexistant-field"));
				}
			);
		}

		[Fact]
		public void Validation_or_value_is_serializable()
		{
			var expected = new OneOf<string, ValidationState>("test");

			var serialized = JsonConvert.SerializeObject(expected);
			var deserialized = JsonConvert.DeserializeObject<OneOf<string, ValidationState>>(serialized);
			
			Assert.True(deserialized.Is1);
			Assert.Equal(expected.As1, deserialized.As1);
		}
	}
}
