using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders.Abstraction;
using Odachi.CodeModel.Description;
using System.Reflection;
using Odachi.Extensions.Reflection;

namespace Odachi.CodeModel.Builders
{
	public class TypeReferenceBuilder : BuilderBase<TypeReferenceBuilder, TypeReference>
	{
		public TypeReferenceBuilder(PackageContext context, Type type, object source, int sourceIndex = 0)
			: base(context)
		{
			Type = type ?? throw new ArgumentNullException(nameof(type));
			IsNullable = !type.IsNonNullableValueType();
			Source = source;
			SourceIndex = sourceIndex;

			Context.TypeReferenceDescriptors.Describe(this);
		}

		public Type Type { get; }
		public bool IsNullable { get; set; }
		public object Source { get; }
		public int SourceIndex { get; }

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
					throw new InvalidOperationException($"Cannot resolve reference '{underlyingType.FullName}'");
				}

				var resolvedTypeGenericArguments = resolvedType.GetGenericArguments();

				var currentSourceIndex = SourceIndex + 1;

				var genericArguments = new TypeReference[type.GenericArguments.Count];
				for (var i = 0; i < type.GenericArguments.Count; i++)
				{
					var underlyingArgumentType = Nullable.GetUnderlyingType(resolvedTypeGenericArguments[i]) ?? resolvedTypeGenericArguments[i];
					var argumentType = Context.TypeMapper.Get(underlyingArgumentType, out var resolvedGenericArgumentType, tryRegister: true);
					if (argumentType == null)
					{
						throw new InvalidOperationException($"Cannot resolve reference '{underlyingType.FullName}'");
					}

					var builder = new TypeReferenceBuilder(Context, resolvedGenericArgumentType, Source, currentSourceIndex);
					var argumentTypeReference = builder.Build();

					genericArguments[i] = argumentTypeReference;

					currentSourceIndex += 1 + argumentTypeReference.GenericArguments.Length;
				}

				return new TypeReference(type.Module, type.Name, kind, IsNullable, genericArguments);
			}
		}
	}
}
