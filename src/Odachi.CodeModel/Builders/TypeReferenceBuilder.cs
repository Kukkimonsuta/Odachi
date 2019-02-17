using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders.Abstraction;
using Odachi.CodeModel.Description;
using System.Reflection;

namespace Odachi.CodeModel.Builders
{
	public class TypeReferenceBuilder : BuilderBase<TypeReferenceBuilder, TypeReference>
	{
		public TypeReferenceBuilder(PackageContext context, Type type, bool? isNullable = null)
			: base(context)
		{
			Type = type ?? throw new ArgumentNullException(nameof(type));
			IsNullable = isNullable ?? (!type.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(Type) != null);

			Context.TypeReferenceDescriptors.Describe(this);
		}

		public Type Type { get; }
		public bool IsNullable { get; set; }

		public override TypeReference Build()
		{
			var underlyingType = Nullable.GetUnderlyingType(Type) ?? Type;

			var kind = Type.GetTypeKind();

			if (kind == TypeKind.GenericParameter)
			{
				return new TypeReference(null, Type.Name, kind, IsNullable);
			}
			else
			{
				var type = Context.TypeMapper.Get(underlyingType, out var resolvedType, tryRegister: true);
				if (type == null)
				{
					throw new InvalidOperationException($"Cannot resolve reference '{ToString()}'");
				}

				var resolvedTypeGenericArguments = resolvedType.GetGenericArguments();

				var genericArguments = type.GenericArguments
					.Select((a, i) => new TypeReferenceBuilder(Context, resolvedTypeGenericArguments[i], null).Build())
					.ToArray();

				return new TypeReference(type.Module, type.Name, kind, IsNullable, genericArguments);
			}
		}
	}
}
