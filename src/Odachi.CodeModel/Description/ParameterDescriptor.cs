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
	public interface IParameterDescriptor
	{
		/// <summary>
		/// Describe parameter. Called after builder creation for every parameter in package.
		/// </summary>
		void Describe(ParameterBuilder builder);
	}

	public static class ParameterDescriptorExtensions
	{
		public static void Describe(this IList<IParameterDescriptor> descriptors, ParameterBuilder builder)
		{
			for (var i = 0; i < descriptors.Count; i++)
			{
				descriptors[i].Describe(builder);
			}
		}
	}

	public class DefaultParameterDescriptor : IParameterDescriptor
	{
		protected virtual void DescribeType(ParameterBuilder builder, ParameterInfo parameterInfo)
		{
			builder.Hint("net-kind", "parameter");
			builder.Hint("net-assembly", parameterInfo.ParameterType.Assembly.FullName);
			builder.Hint("net-type", parameterInfo.ParameterType.FullName);
		}

		/// <inheritdoc />
		public virtual void Describe(ParameterBuilder builder)
		{
			switch (builder.Source)
			{
				case ParameterInfo parameterInfo:
					DescribeType(builder, parameterInfo);
					break;
			}
		}
	}
}
