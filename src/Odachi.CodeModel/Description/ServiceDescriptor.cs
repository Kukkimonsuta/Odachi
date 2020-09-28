using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders;

namespace Odachi.CodeModel.Description
{
	public interface IServiceDescriptor
	{
		/// <summary>
		/// Describe service. Called after builder creation for every service in package.
		/// </summary>
		void Describe(ServiceBuilder builder);
	}

	public static class ServiceDescriptorExtensions
	{
		public static void Describe(this IList<IServiceDescriptor> descriptors, ServiceBuilder builder)
		{
			for (var i = 0; i < descriptors.Count; i++)
			{
				descriptors[i].Describe(builder);
			}
		}
	}

	public class DefaultServiceDescriptor : IServiceDescriptor
	{
		protected virtual void DescribeType(ServiceBuilder builder, Type type)
		{
			builder.Hint("source-type", type.AssemblyQualifiedName);
		}

		/// <inheritdoc />
		public virtual void Describe(ServiceBuilder builder)
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
