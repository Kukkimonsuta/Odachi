using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Odachi.Extensions.Primitives;
using Odachi.Extensions.Reflection.Tests.Model;
using Xunit;

namespace Odachi.Extensions.Reflection.Tests
{
	public class NullabilityTests
	{
		[Fact]
		public void Basic_model_properties_and_fields()
		{
			Assert.True(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.NonNullable))!.IsNonNullable());
			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.NonNullable))!.IsNonNullableValueType());
			Assert.True(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.NonNullable))!.IsNonNullableReferenceType());

			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.Nullable))!.IsNonNullable());
			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.Nullable))!.IsNonNullableValueType());
			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.Nullable))!.IsNonNullableReferenceType());

			Assert.True(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.Value))!.IsNonNullable());
			Assert.True(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.Value))!.IsNonNullableValueType());
			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.Value))!.IsNonNullableReferenceType());

			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.NullableValue))!.IsNonNullable());
			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.NullableValue))!.IsNonNullableValueType());
			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.NullableValue))!.IsNonNullableReferenceType());
		}

		[Fact]
		public void Nested_model_properties_and_fields()
		{
			Assert.True(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.NonNullable))!.IsNonNullable());
			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.NonNullable))!.IsNonNullableValueType());
			Assert.True(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.NonNullable))!.IsNonNullableReferenceType());

			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.Nullable))!.IsNonNullable());
			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.Nullable))!.IsNonNullableValueType());
			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.Nullable))!.IsNonNullableReferenceType());

			Assert.True(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.Value))!.IsNonNullable());
			Assert.True(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.Value))!.IsNonNullableValueType());
			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.Value))!.IsNonNullableReferenceType());

			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.NullableValue))!.IsNonNullable());
			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.NullableValue))!.IsNonNullableValueType());
			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.NullableValue))!.IsNonNullableReferenceType());


			Assert.True(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.NonNullable))!.IsNonNullable());
			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.NonNullable))!.IsNonNullableValueType());
			Assert.True(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.NonNullable))!.IsNonNullableReferenceType());

			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.Nullable))!.IsNonNullable());
			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.Nullable))!.IsNonNullableValueType());
			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.Nullable))!.IsNonNullableReferenceType());

			Assert.True(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.Value))!.IsNonNullable());
			Assert.True(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.Value))!.IsNonNullableValueType());
			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.Value))!.IsNonNullableReferenceType());

			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.NullableValue))!.IsNonNullable());
			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.NullableValue))!.IsNonNullableValueType());
			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.NullableValue))!.IsNonNullableReferenceType());
		}

		[Fact]
		public void Basic_model_method_return_types()
		{
			Assert.False(typeof(NullableBasicModel).GetMethod(nameof(NullableBasicModel.NullableResult))!.ReturnParameter!.IsNonNullable());
			Assert.False(typeof(NullableBasicModel).GetMethod(nameof(NullableBasicModel.NullableResult))!.ReturnParameter!.IsNonNullableReferenceType());

			Assert.True(typeof(NullableBasicModel).GetMethod(nameof(NullableBasicModel.NonNullableResult))!.ReturnParameter!.IsNonNullable());
			Assert.True(typeof(NullableBasicModel).GetMethod(nameof(NullableBasicModel.NonNullableResult))!.ReturnParameter!.IsNonNullableReferenceType());

			Assert.False(typeof(NullableBasicModel).GetMethod(nameof(NullableBasicModel.NullableResultWithNonNullableParam))!.ReturnParameter!.IsNonNullable());
			Assert.False(typeof(NullableBasicModel).GetMethod(nameof(NullableBasicModel.NullableResultWithNonNullableParam))!.ReturnParameter!.IsNonNullableReferenceType());
		}

		[Fact]
		public void Generic_model()
		{
			var nonNullableProperty = typeof(NullableGenericModel).GetProperty(nameof(NullableGenericModel.NonNullable))!;

			Assert.True(nonNullableProperty.IsNonNullable());
			Assert.False(nonNullableProperty.IsNonNullableValueType());
			Assert.True(nonNullableProperty.IsNonNullableReferenceType());

			Assert.True(nonNullableProperty.IsGenericArgumentNonNullable(0));
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullableValueType(0));
			Assert.True(nonNullableProperty.IsGenericArgumentNonNullableReferenceType(0));

			var nullableProperty = typeof(NullableGenericModel).GetProperty(nameof(NullableGenericModel.Nullable))!;

			Assert.True(nullableProperty.IsNonNullable());
			Assert.False(nullableProperty.IsNonNullableValueType());
			Assert.True(nullableProperty.IsNonNullableReferenceType());

			Assert.False(nullableProperty.IsGenericArgumentNonNullable(0));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableValueType(0));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableReferenceType(0));
		}

		[Fact]
		public void Array_reference_model()
		{
			var nullableProperty = typeof(NullableArray).GetProperty(nameof(NullableArray.Nullable))!;

			Assert.True(nullableProperty.IsNonNullable());
			Assert.False(nullableProperty.IsNonNullableValueType());
			Assert.True(nullableProperty.IsNonNullableReferenceType());

			Assert.False(nullableProperty.IsGenericArgumentNonNullable(0));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableValueType(0));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableReferenceType(0));


			var nonNullableProperty = typeof(NullableArray).GetProperty(nameof(NullableArray.NonNullable))!;

			Assert.True(nonNullableProperty.IsNonNullable());
			Assert.False(nonNullableProperty.IsNonNullableValueType());
			Assert.True(nonNullableProperty.IsNonNullableReferenceType());

			Assert.True(nonNullableProperty.IsGenericArgumentNonNullable(0));
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullableValueType(0));
			Assert.True(nonNullableProperty.IsGenericArgumentNonNullableReferenceType(0));
		}

		[Fact]
		public void Array_value_model()
		{
			var nullableProperty = typeof(NullableValueArray).GetProperty(nameof(NullableValueArray.Nullable))!;

			Assert.True(nullableProperty.IsNonNullable());
			Assert.False(nullableProperty.IsNonNullableValueType());
			Assert.True(nullableProperty.IsNonNullableReferenceType());

			Assert.False(nullableProperty.IsGenericArgumentNonNullable(0));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableValueType(0));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableReferenceType(0));


			var nonNullableProperty = typeof(NullableValueArray).GetProperty(nameof(NullableValueArray.NonNullable))!;

			Assert.True(nonNullableProperty.IsNonNullable());
			Assert.False(nonNullableProperty.IsNonNullableValueType());
			Assert.True(nonNullableProperty.IsNonNullableReferenceType());

			Assert.True(nonNullableProperty.IsGenericArgumentNonNullable(0));
			Assert.True(nonNullableProperty.IsGenericArgumentNonNullableValueType(0));
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullableReferenceType(0));
		}

		[Fact]
		public void Method_params_model()
		{
			var nullableProperty = typeof(NullableMethodParameters).GetMethod(nameof(NullableMethodParameters.Nullable))!.GetParameters().Single();

			Assert.True(nullableProperty.IsNonNullable());
			Assert.False(nullableProperty.IsNonNullableValueType());
			Assert.True(nullableProperty.IsNonNullableReferenceType());

			Assert.False(nullableProperty.IsGenericArgumentNonNullable(0));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableValueType(0));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableReferenceType(0));

			var nonNullableProperty = typeof(NullableMethodParameters).GetMethod(nameof(NullableMethodParameters.NonNullable))!.GetParameters().Single();

			Assert.True(nonNullableProperty.IsNonNullable());
			Assert.False(nonNullableProperty.IsNonNullableValueType());
			Assert.True(nonNullableProperty.IsNonNullableReferenceType());

			Assert.True(nonNullableProperty.IsGenericArgumentNonNullable(0));
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullableValueType(0));
			Assert.True(nonNullableProperty.IsGenericArgumentNonNullableReferenceType(0));
		}

		[Fact]
		public void Complex_generic_type_model()
		{
			// OneOf<string, string?, int, int?, object, object?>
			// flags=[0, 1, 2, 1, 2]
			var nonNullableProperty = typeof(ComplexGenericTypeModel).GetProperty(nameof(ComplexGenericTypeModel.NonNullable))!;

			// OneOf<>
			Assert.True(nonNullableProperty.IsNonNullable());
			Assert.True(nonNullableProperty.IsNonNullableValueType());
			Assert.False(nonNullableProperty.IsNonNullableReferenceType());

			// string
			Assert.True(nonNullableProperty.IsGenericArgumentNonNullable(0));
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullableValueType(0));
			Assert.True(nonNullableProperty.IsGenericArgumentNonNullableReferenceType(0));

			// string?
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullable(1));
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullableValueType(1));
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullableReferenceType(1));

			// int
			Assert.True(nonNullableProperty.IsGenericArgumentNonNullable(2));
			Assert.True(nonNullableProperty.IsGenericArgumentNonNullableValueType(2));
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullableReferenceType(2));

			// int?
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullable(3));
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullableValueType(3));
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullableReferenceType(3));

			// object
			Assert.True(nonNullableProperty.IsGenericArgumentNonNullable(4));
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullableValueType(4));
			Assert.True(nonNullableProperty.IsGenericArgumentNonNullableReferenceType(4));

			// object?
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullable(5));
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullableValueType(5));
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullableReferenceType(5));

			// error
			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				nonNullableProperty.IsGenericArgumentNonNullable(6);
			});

			// OneOf<string, string?, int, int?, object, object?>?
			var nullableProperty = typeof(ComplexGenericTypeModel).GetProperty(nameof(ComplexGenericTypeModel.Nullable))!;

			// OneOf<>
			Assert.False(nullableProperty.IsNonNullable());
			Assert.False(nullableProperty.IsNonNullableValueType());
			Assert.False(nullableProperty.IsNonNullableReferenceType());

			// string
			Assert.True(nullableProperty.IsGenericArgumentNonNullable(0));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableValueType(0));
			Assert.True(nullableProperty.IsGenericArgumentNonNullableReferenceType(0));

			// string?
			Assert.False(nullableProperty.IsGenericArgumentNonNullable(1));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableValueType(1));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableReferenceType(1));

			// int
			Assert.True(nullableProperty.IsGenericArgumentNonNullable(2));
			Assert.True(nullableProperty.IsGenericArgumentNonNullableValueType(2));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableReferenceType(2));

			// int?
			Assert.False(nullableProperty.IsGenericArgumentNonNullable(3));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableValueType(3));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableReferenceType(3));

			// object
			Assert.True(nullableProperty.IsGenericArgumentNonNullable(4));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableValueType(4));
			Assert.True(nullableProperty.IsGenericArgumentNonNullableReferenceType(4));

			// object?
			Assert.False(nullableProperty.IsGenericArgumentNonNullable(5));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableValueType(5));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableReferenceType(5));

			// error
			Assert.Throws<ArgumentOutOfRangeException>(() =>
			{
				nullableProperty.IsGenericArgumentNonNullable(6);
			});
		}
	}
}
