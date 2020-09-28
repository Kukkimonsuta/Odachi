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
	public interface IConstantDescriptor
	{
		/// <summary>
		/// Describe constant. Called after builder creation for every constant in package.
		/// </summary>
		void Describe(ConstantBuilder builder);
	}

	public static class ConstantDescriptorExtensions
	{
		public static void Describe(this IList<IConstantDescriptor> descriptors, ConstantBuilder builder)
		{
			for (var i = 0; i < descriptors.Count; i++)
			{
				descriptors[i].Describe(builder);
			}
		}
	}

	public class DefaultConstantDescriptor : IConstantDescriptor
	{
		protected virtual void DescribeConstantInfo(ConstantBuilder builder, Type type, FieldInfo fieldInfo)
		{
			builder.Hint("net-kind", "constant");
			builder.Hint("net-type", fieldInfo.FieldType.AssemblyQualifiedName);

			if (fieldInfo.IsNonNullable())
			{
				builder.Type.IsNullable = false;
			}
		}

		/// <inheritdoc />
		public virtual void Describe(ConstantBuilder builder)
		{
			switch (builder.Source)
			{
				case ValueTuple<Type, FieldInfo> constantSource:
					DescribeConstantInfo(builder, constantSource.Item1, constantSource.Item2);
					break;
			}
		}
	}
}
