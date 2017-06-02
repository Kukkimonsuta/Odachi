using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Odachi.Validation
{
	public class ValidatorTest
	{
		private ValueOrState<string> TestBusinessMethod(string foo)
		{
			var validator = new Validator();

			if (foo.Length <= 0)
				return validator.Error(nameof(foo), "Required field");
			
			return $"bar_{foo}";
		}
		
		[Fact]
		public void Validation_success_result()
		{
			var result = TestBusinessMethod("test");

			Assert.NotNull(result.Value);
			Assert.Equal("bar_test", result.Value);

			Assert.Null(result.State);
		}

		[Fact]
		public void Validation_error_result()
		{
			var result = TestBusinessMethod("");

			Assert.Null(result.Value);

			Assert.NotNull(result.State);
			Assert.Equal("Required field", result.State.GetError("foo"));
			Assert.Null(result.State.GetError("nonexistant-field"));
		}
	}
}
