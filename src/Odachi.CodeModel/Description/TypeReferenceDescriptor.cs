using Odachi.CodeModel.Description.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders;

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
		/// <inheritdoc />
		public virtual void Describe(TypeReferenceBuilder builder)
		{
		}
	}
}
