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
	public interface IFieldDescriptor
	{
		/// <summary>
		/// Describe field. Called after builder creation for every field in package.
		/// </summary>
		void Describe(FieldBuilder builder);
	}

	public static class FieldDescriptorExtensions
	{
		public static void Describe(this IList<IFieldDescriptor> descriptors, FieldBuilder builder)
		{
			for (var i = 0; i < descriptors.Count; i++)
			{
				descriptors[i].Describe(builder);
			}
		}
	}

	public class DefaultFieldDescriptor : IFieldDescriptor
	{
		protected virtual void DescribeFieldInfo(FieldBuilder builder, Type type, FieldInfo fieldInfo)
		{
			builder.Hint("net-kind", "field");
			builder.Hint("net-type", fieldInfo.FieldType.AssemblyQualifiedName);

			var displayName = ClassDescriptionHelper.GetDisplayName(fieldInfo);
			if (displayName != null)
			{
				builder.Hint("display-name", displayName);
			}
		}

		protected virtual void DescribePropertyInfo(FieldBuilder builder, Type type, PropertyInfo propertyInfo)
		{
			builder.Hint("net-kind", "property");
			builder.Hint("net-type", propertyInfo.PropertyType.AssemblyQualifiedName);

			var displayName = ClassDescriptionHelper.GetDisplayName(propertyInfo);
			if (displayName != null)
			{
				builder.Hint("display-name", displayName);
			}
		}

		/// <inheritdoc />
		public virtual void Describe(FieldBuilder builder)
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
