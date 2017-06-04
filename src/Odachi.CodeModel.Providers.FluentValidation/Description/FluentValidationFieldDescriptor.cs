using System;
using System.Reflection;
using Odachi.CodeModel.Builders;
using Odachi.CodeModel.Description;
using Odachi.CodeModel.Providers.FluentValidation.Description.Internal;

namespace Odachi.CodeModel.Providers.FluentValidation.Description
{
	public class FluentValidationFieldDescriptor : IFieldDescriptor
	{
		protected virtual void DescribeFieldInfo(FieldBuilder builder, Type type, FieldInfo fieldInfo)
		{
			if (FluentValidationDescriptionHelper.IsRequired(type, fieldInfo))
			{
				builder.Type.IsNullable = false;
			}
		}

		protected virtual void DescribePropertyInfo(FieldBuilder builder, Type type, PropertyInfo propertyInfo)
		{
			if (FluentValidationDescriptionHelper.IsRequired(type, propertyInfo))
			{
				builder.Type.IsNullable = false;
			}
		}

		public void Describe(FieldBuilder builder)
		{
			switch (builder.Source)
			{
				case ValueTuple<Type, FieldInfo> fieldSource:
					DescribeFieldInfo(builder, fieldSource.Item1, fieldSource.Item2);
					break;

				case ValueTuple<Type, PropertyInfo> propertySource:
					DescribePropertyInfo(builder, propertySource.Item1, propertySource.Item2);
					break;
			}
		}
	}
}
