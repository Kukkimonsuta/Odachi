using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders;

namespace Odachi.CodeModel.Description
{
	public interface IClassDescriptor
	{
		/// <summary>
		/// Describe class. Called after builder creation for every class in package.
		/// </summary>
		void Describe(ClassBuilder builder);
	}

	public static class ClassDescriptorExtensions
	{
		public static void Describe(this IList<IClassDescriptor> descriptors, ClassBuilder builder)
		{
			for (var i = 0; i < descriptors.Count; i++)
			{
				descriptors[i].Describe(builder);
			}
		}
	}

	public class DefaultClassDescriptor : IClassDescriptor
	{
		protected virtual void DescribeType(ClassBuilder builder, Type type)
		{
			builder.Hint("source-type", type.AssemblyQualifiedName);
		}

		/// <inheritdoc />
		public virtual void Describe(ClassBuilder builder)
		{
			switch (builder.Source)
			{
				case Type type:
					DescribeType(builder, type);
					break;
			}
		}
	}
}
