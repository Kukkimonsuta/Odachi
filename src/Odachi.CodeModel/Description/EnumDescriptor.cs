using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders;

namespace Odachi.CodeModel.Description
{
	public interface IEnumDescriptor
	{
		/// <summary>
		/// Describe enum. Called after builder creation for every enum in package.
		/// </summary>
		void Describe(EnumBuilder builder);
	}

	public static class EnumDescriptorExtensions
	{
		public static void Describe(this IList<IEnumDescriptor> descriptors, EnumBuilder builder)
		{
			for (var i = 0; i < descriptors.Count; i++)
			{
				descriptors[i].Describe(builder);
			}
		}
	}

	public class DefaultEnumDescriptor : IEnumDescriptor
	{
		protected virtual void DescribeType(EnumBuilder builder, Type type)
		{
			if (type.GetTypeInfo().GetCustomAttribute<FlagsAttribute>() != null)
			{
				builder.Hint("enum-flags", "true");
			}
			builder.Hint("source-assembly", type.Assembly.FullName);
			builder.Hint("source-type", type.FullName);
		}

		/// <inheritdoc />
		public virtual void Describe(EnumBuilder builder)
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
