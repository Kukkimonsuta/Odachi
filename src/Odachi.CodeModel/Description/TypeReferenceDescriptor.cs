using Odachi.CodeModel.Description.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders;
using Odachi.Extensions.Reflection;

namespace Odachi.CodeModel.Description
{
	public interface ITypeReferenceDescriptor
	{
		/// <summary>
		/// Describe type reference. Called after builder creation for every type reference in package.
		/// </summary>
		void Describe(TypeReferenceBuilder builder);
	}

	public static class TypeReferenceDescriptorExtensions
	{
		public static void Describe(this IList<ITypeReferenceDescriptor> descriptors, TypeReferenceBuilder builder)
		{
			for (var i = 0; i < descriptors.Count; i++)
			{
				descriptors[i].Describe(builder);
			}
		}
	}

	public class DefaultTypeReferenceDescriptor : ITypeReferenceDescriptor
	{
		protected virtual void DescribeFromPropertyInfo(TypeReferenceBuilder builder, PropertyInfo propertyInfo)
		{
			if (builder.SourceIndex == 0)
			{
				if (propertyInfo.IsNonNullable())
				{
					builder.IsNullable = false;
				}
			}
			else
			{
				if (propertyInfo.IsGenericArgumentNonNullable(builder.SourceIndex - 1))
				{
					builder.IsNullable = false;
				}
			}
		}

		protected virtual void DescribeFromFieldInfo(TypeReferenceBuilder builder, FieldInfo fieldInfo)
		{
			if (builder.SourceIndex == 0)
			{
				if (fieldInfo.IsNonNullable())
				{
					builder.IsNullable = false;
				}
			}
			else
			{
				if (fieldInfo.IsGenericArgumentNonNullable(builder.SourceIndex - 1))
				{
					builder.IsNullable = false;
				}
			}
		}

		protected virtual void DescribeFromMethodReturnType(TypeReferenceBuilder builder, MethodInfo methodInfo)
		{
			if (builder.SourceIndex == 0)
			{
				if (methodInfo.ReturnParameter?.IsNonNullable() == true)
				{
					builder.IsNullable = false;
				}
			}
			else
			{
				if (methodInfo.ReturnParameter?.IsGenericArgumentNonNullable(builder.SourceIndex - 1) == true)
				{
					builder.IsNullable = false;
				}
			}
		}

		/// <inheritdoc />
		public virtual void Describe(TypeReferenceBuilder builder)
		{
			switch (builder.Source)
			{
				case ValueTuple<Type, PropertyInfo> propertySource:
					DescribeFromPropertyInfo(builder, propertySource.Item2);
					break;

				case ValueTuple<Type, FieldInfo> fieldSource:
					DescribeFromFieldInfo(builder, fieldSource.Item2);
					break;

				case ValueTuple<Type, MethodInfo> methodSource:
					DescribeFromMethodReturnType(builder, methodSource.Item2);
					break;

				case PropertyInfo propertyInfo:
					DescribeFromPropertyInfo(builder, propertyInfo);
					break;

				case FieldInfo fieldInfo:
					DescribeFromFieldInfo(builder, fieldInfo);
					break;

				case MethodInfo methodInfo:
					DescribeFromMethodReturnType(builder, methodInfo);
					break;

				case ParameterInfo parameterInfo:
					if (builder.SourceIndex == 0)
					{
						if (parameterInfo.IsNonNullable())
						{
							builder.IsNullable = false;
						}
					}
					else
					{
						if (parameterInfo.IsGenericArgumentNonNullable(builder.SourceIndex - 1))
						{
							builder.IsNullable = false;
						}
					}
					break;

				default:
					break;
			}
		}
	}
}
