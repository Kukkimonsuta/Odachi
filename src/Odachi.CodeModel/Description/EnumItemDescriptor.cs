using Odachi.CodeModel.Description.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders;

namespace Odachi.CodeModel.Description
{
	public interface IEnumItemDescriptor
	{
		/// <summary>
		/// Describe enum item. Called after builder creation for every enum item in package.
		/// </summary>
		void Describe(EnumItemBuilder builder);
	}

	public static class EnumItemDescriptorExtensions
	{
		public static void Describe(this IList<IEnumItemDescriptor> descriptors, EnumItemBuilder builder)
		{
			for (var i = 0; i < descriptors.Count; i++)
			{
				descriptors[i].Describe(builder);
			}
		}
	}

	public class DefaultEnumItemDescriptor : IEnumItemDescriptor
	{
		protected virtual void DescribeType(EnumItemBuilder builder, Type enumType)
		{
			var enumValue = Enum.ToObject(enumType, builder.Value);

			if (EnumDescriptionHelper.TryGetDisplayName(enumType, enumValue, out var displayName))
			{
				builder.Hint("display-name", displayName);
			}
		}

		/// <inheritdoc />
		public virtual void Describe(EnumItemBuilder builder)
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
