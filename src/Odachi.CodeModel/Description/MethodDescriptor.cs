using Odachi.CodeModel.Description.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders;

namespace Odachi.CodeModel.Description
{
	public interface IMethodDescriptor
	{
		/// <summary>
		/// Describe method. Called after builder creation for every method in package.
		/// </summary>
		void Describe(MethodBuilder builder);
	}

	public static class MethodDescriptorExtensions
	{
		public static void Describe(this IList<IMethodDescriptor> descriptors, MethodBuilder builder)
		{
			for (var i = 0; i < descriptors.Count; i++)
			{
				descriptors[i].Describe(builder);
			}
		}
	}

	public class DefaultMethodDescriptor : IMethodDescriptor
	{
		protected virtual void DescribeMethodInfo(MethodBuilder builder, MethodInfo methodInfo)
		{
			builder.Hint("net-kind", "method");
			builder.Hint("net-return-type", methodInfo.ReturnType.AssemblyQualifiedName);
		}

		/// <inheritdoc />
		public virtual void Describe(MethodBuilder builder)
		{
			switch (builder.Source)
			{
				case MethodInfo methodInfo:
					DescribeMethodInfo(builder, methodInfo);
					break;
			}
		}
	}
}
