using System;
using System.Reflection;
using Odachi.CodeModel.Builders;
using Odachi.CodeModel.Description;
using Odachi.CodeModel.Providers.FluentValidation.Description.Internal;

namespace Odachi.CodeModel.Providers.FluentValidation.Description
{
	public class FluentValidationFieldDescriptor : IFieldDescriptor
	{
		protected virtual void DescribeMemberInfo(FieldBuilder builder, Type type, MemberInfo memberInfo)
		{
			if (FluentValidationDescriptionHelper.IsRequired(type, memberInfo))
			{
				builder.Hint("validation:is-required", "true");
			}

			var (minLength, maxLength) = FluentValidationDescriptionHelper.Length(type, memberInfo);
			if (minLength != -1)
			{
				builder.Hint("validation:min-length", minLength.ToString());
			}
			if (maxLength != -1)
			{
				builder.Hint("validation:max-length", maxLength.ToString());
			}
		}

		protected virtual void DescribeFieldInfo(FieldBuilder builder, Type type, FieldInfo fieldInfo)
		{
		}

		protected virtual void DescribePropertyInfo(FieldBuilder builder, Type type, PropertyInfo propertyInfo)
		{
		}

		public void Describe(FieldBuilder builder)
		{
			switch (builder.Source)
			{
				case ValueTuple<Type, FieldInfo> fieldSource:
					DescribeMemberInfo(builder, fieldSource.Item1, fieldSource.Item2);
					DescribeFieldInfo(builder, fieldSource.Item1, fieldSource.Item2);
					break;

				case ValueTuple<Type, PropertyInfo> propertySource:
					DescribeMemberInfo(builder, propertySource.Item1, propertySource.Item2);
					DescribePropertyInfo(builder, propertySource.Item1, propertySource.Item2);
					break;
			}
		}
	}
}
