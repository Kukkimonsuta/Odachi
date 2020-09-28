using Odachi.CodeModel.Builders;
using Odachi.CodeModel.Providers.FluentValidation.Tests.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Odachi.CodeModel.Providers.FluentValidation.Tests
{
	public class DescriptionTests
	{
		private void AssertIsRequired(FieldFragment field, bool required)
		{
			field.Hints.TryGetValue("validation:is-required", out var actualRequired);

			if (required)
			{
				Assert.Equal("true", actualRequired);
			}
			else
			{
				if (actualRequired == null)
				{
					Assert.Null(actualRequired);
				}
				else
				{
					Assert.Equal("false", actualRequired);
				}
			}
		}

		private void AssertLength(FieldFragment field, int min, int max)
		{
			field.Hints.TryGetValue("validation:min-length", out var actualMinLength);
			field.Hints.TryGetValue("validation:max-length", out var actualMaxLength);

			if (min != -1)
			{
				Assert.Equal(min.ToString(), actualMinLength);
			}
			else
			{
				Assert.Null(actualMinLength);
			}

			if (max != -1)
			{
				Assert.Equal(max.ToString(), actualMaxLength);
			}
			else
			{
				Assert.Null(actualMaxLength);
			}
		}

		[Fact]
		public void Can_describe_object_with_ambient_validation()
		{
			var package = new PackageBuilder("Test")
				.UseFluentValidation()
				.Module_Object_Default<ObjectWithAmbientValidation>()
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Objects);

			var obj = package.Objects[0];
			Assert.Equal(nameof(ObjectWithAmbientValidation), obj.Name);

			var optionalIntField = obj.Fields.Where(f => f.Name == nameof(ObjectWithAmbientValidation.OptionalInt)).SingleOrDefault();
			Assert.NotNull(optionalIntField);
			AssertIsRequired(optionalIntField, false);
			AssertLength(optionalIntField, -1, -1);

			var requiredStringField = obj.Fields.Where(f => f.Name == nameof(ObjectWithAmbientValidation.RequiredString)).SingleOrDefault();
			Assert.NotNull(requiredStringField);
			AssertIsRequired(requiredStringField, true);
			AssertLength(requiredStringField, -1, -1);

			var requiredLengthString = obj.Fields.Where(f => f.Name == nameof(ObjectWithAmbientValidation.RequiredLengthString)).SingleOrDefault();
			Assert.NotNull(requiredLengthString);
			AssertIsRequired(requiredLengthString, true);
			AssertLength(requiredLengthString, 10, 20);
		}

		[Fact]
		public void Can_describe_object_with_ambient_nested_validation()
		{
			var package = new PackageBuilder("Test")
				.UseFluentValidation()
				.Module_Object_Default<ObjectWithAmbientNestedValidation>()
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Objects);

			var obj = package.Objects[0];
			Assert.Equal(nameof(ObjectWithAmbientNestedValidation), obj.Name);

			var optionalIntField = obj.Fields.Where(f => f.Name == nameof(ObjectWithAmbientNestedValidation.OptionalInt)).SingleOrDefault();
			Assert.NotNull(optionalIntField);
			AssertIsRequired(optionalIntField, false);
			AssertLength(optionalIntField, -1, -1);

			var requiredStringField = obj.Fields.Where(f => f.Name == nameof(ObjectWithAmbientNestedValidation.RequiredString)).SingleOrDefault();
			Assert.NotNull(requiredStringField);
			AssertIsRequired(requiredStringField, true);
			AssertLength(requiredStringField, -1, -1);

			var requiredLengthString = obj.Fields.Where(f => f.Name == nameof(ObjectWithAmbientNestedValidation.RequiredLengthString)).SingleOrDefault();
			Assert.NotNull(requiredLengthString);
			AssertIsRequired(requiredLengthString, true);
			AssertLength(requiredLengthString, 10, 20);
		}
	}
}
