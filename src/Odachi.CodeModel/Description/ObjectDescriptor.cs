using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders;

namespace Odachi.CodeModel.Description
{
	public interface IObjectDescriptor
	{
		/// <summary>
		/// Describe object. Called after builder creation for every object in package.
		/// </summary>
		void Describe(ObjectBuilder builder);
	}

	public static class ObjectDescriptorExtensions
	{
		public static void Describe(this IList<IObjectDescriptor> descriptors, ObjectBuilder builder)
		{
			for (var i = 0; i < descriptors.Count; i++)
			{
				descriptors[i].Describe(builder);
			}
		}
	}

	public class DefaultObjectDescriptor : IObjectDescriptor
	{
		protected virtual void DescribeType(ObjectBuilder builder, Type type)
		{
			builder.Hint("source-type", type.AssemblyQualifiedName);
		}

		/// <inheritdoc />
		public virtual void Describe(ObjectBuilder builder)
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
