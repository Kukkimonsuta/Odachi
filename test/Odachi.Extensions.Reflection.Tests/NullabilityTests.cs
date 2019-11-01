using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Odachi.Extensions.Reflection.Tests.Model;
using Xunit;

namespace Odachi.Extensions.Reflection.Tests
{
	public class NullabilityTests
	{
		[Fact]
		public void Basic_model_properties_and_fields()
		{
			Assert.True(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.NonNullable)).IsNonNullable());
			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.NonNullable)).IsNonNullableValueType());
			Assert.True(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.NonNullable)).IsNonNullableReferenceType());

			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.Nullable)).IsNonNullable());
			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.Nullable)).IsNonNullableValueType());
			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.Nullable)).IsNonNullableReferenceType());

			Assert.True(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.Value)).IsNonNullable());
			Assert.True(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.Value)).IsNonNullableValueType());
			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.Value)).IsNonNullableReferenceType());

			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.NullableValue)).IsNonNullable());
			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.NullableValue)).IsNonNullableValueType());
			Assert.False(typeof(NullableBasicModel).GetProperty(nameof(NullableBasicModel.NullableValue)).IsNonNullableReferenceType());
		}

		[Fact]
		public void Nested_model_properties_and_fields()
		{
			Assert.True(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.NonNullable)).IsNonNullable());
			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.NonNullable)).IsNonNullableValueType());
			Assert.True(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.NonNullable)).IsNonNullableReferenceType());

			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.Nullable)).IsNonNullable());
			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.Nullable)).IsNonNullableValueType());
			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.Nullable)).IsNonNullableReferenceType());

			Assert.True(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.Value)).IsNonNullable());
			Assert.True(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.Value)).IsNonNullableValueType());
			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.Value)).IsNonNullableReferenceType());

			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.NullableValue)).IsNonNullable());
			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.NullableValue)).IsNonNullableValueType());
			Assert.False(typeof(NullableNestedModel).GetProperty(nameof(NullableNestedModel.NullableValue)).IsNonNullableReferenceType());


			Assert.True(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.NonNullable)).IsNonNullable());
			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.NonNullable)).IsNonNullableValueType());
			Assert.True(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.NonNullable)).IsNonNullableReferenceType());

			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.Nullable)).IsNonNullable());
			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.Nullable)).IsNonNullableValueType());
			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.Nullable)).IsNonNullableReferenceType());

			Assert.True(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.Value)).IsNonNullable());
			Assert.True(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.Value)).IsNonNullableValueType());
			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.Value)).IsNonNullableReferenceType());

			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.NullableValue)).IsNonNullable());
			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.NullableValue)).IsNonNullableValueType());
			Assert.False(typeof(NullableNestedModel.Nested).GetProperty(nameof(NullableNestedModel.Nested.NullableValue)).IsNonNullableReferenceType());
		}

		[Fact]
		public void Basic_model_method_return_types()
		{
			Assert.False(typeof(NullableBasicModel).GetMethod(nameof(NullableBasicModel.NullableResult)).ReturnParameter.IsNonNullable());
			Assert.False(typeof(NullableBasicModel).GetMethod(nameof(NullableBasicModel.NullableResult)).ReturnParameter.IsNonNullableReferenceType());

			Assert.True(typeof(NullableBasicModel).GetMethod(nameof(NullableBasicModel.NonNullableResult)).ReturnParameter.IsNonNullable());
			Assert.True(typeof(NullableBasicModel).GetMethod(nameof(NullableBasicModel.NonNullableResult)).ReturnParameter.IsNonNullableReferenceType());

			Assert.False(typeof(NullableBasicModel).GetMethod(nameof(NullableBasicModel.NullableResultWithNonNullableParam)).ReturnParameter.IsNonNullable());
			Assert.False(typeof(NullableBasicModel).GetMethod(nameof(NullableBasicModel.NullableResultWithNonNullableParam)).ReturnParameter.IsNonNullableReferenceType());
		}

		[Fact]
		public void Generic_model()
		{
			var nonNullableProperty = typeof(NullableGenericModel).GetProperty(nameof(NullableGenericModel.NonNullable));

			Assert.True(nonNullableProperty.IsNonNullable());
			Assert.False(nonNullableProperty.IsNonNullableValueType());
			Assert.True(nonNullableProperty.IsNonNullableReferenceType());

			Assert.True(nonNullableProperty.IsGenericArgumentNonNullable(0));
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullableValueType(0));
			Assert.True(nonNullableProperty.IsGenericArgumentNonNullableReferenceType(0));

			var nullableProperty = typeof(NullableGenericModel).GetProperty(nameof(NullableGenericModel.Nullable));

			Assert.True(nullableProperty.IsNonNullable());
			Assert.False(nullableProperty.IsNonNullableValueType());
			Assert.True(nullableProperty.IsNonNullableReferenceType());

			Assert.False(nullableProperty.IsGenericArgumentNonNullable(0));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableValueType(0));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableReferenceType(0));
		}

		[Fact]
		public void Array_model()
		{
			var nullableProperty = typeof(NullableArray).GetProperty(nameof(NullableArray.Nullable));

			Assert.True(nullableProperty.IsNonNullable());
			Assert.False(nullableProperty.IsNonNullableValueType());
			Assert.True(nullableProperty.IsNonNullableReferenceType());

			Assert.False(nullableProperty.IsGenericArgumentNonNullable(0));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableValueType(0));
			Assert.False(nullableProperty.IsGenericArgumentNonNullableReferenceType(0));


			var nonNullableProperty = typeof(NullableArray).GetProperty(nameof(NullableArray.NonNullable));

			Assert.True(nonNullableProperty.IsNonNullable());
			Assert.False(nonNullableProperty.IsNonNullableValueType());
			Assert.True(nonNullableProperty.IsNonNullableReferenceType());

			Assert.True(nonNullableProperty.IsGenericArgumentNonNullable(0));
			Assert.False(nonNullableProperty.IsGenericArgumentNonNullableValueType(0));
			Assert.True(nonNullableProperty.IsGenericArgumentNonNullableReferenceType(0));
		}
	}
}
