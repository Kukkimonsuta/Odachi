using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.CodeModel.Mapping
{
	/// <summary>
	/// Represents a type reference for specified CLR type.
	/// </summary>
	public class ClrTypeReference : ITypeReference
	{
		public ClrTypeReference(Type type, bool? isNullable = null)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			Type = type;
			IsNullable = isNullable ?? (!type.GetTypeInfo().IsValueType || Nullable.GetUnderlyingType(Type) != null);
		}

		public Type Type { get; }
		public bool IsNullable { get; set; }

		public TypeReference Resolve(TypeMapper mapper)
		{
			var underlyingType = Nullable.GetUnderlyingType(Type) ?? Type;

			var kind = Type.GetTypeKind();

			if (kind == TypeKind.GenericParameter)
			{
				return new TypeReference(null, Type.Name, kind, IsNullable);
			}
			else
			{
				var type = mapper.Get(underlyingType, out var resolvedType, tryRegister: true);
				if (type == null)
				{
					throw new InvalidOperationException($"Cannot resolve reference '{ToString()}'");
				}

				var resolvedTypeGenericArguments = resolvedType.GetGenericArguments();
				var genericArguments = type.GenericArguments
					.Select((a, i) => new ClrTypeReference(resolvedTypeGenericArguments[i]).Resolve(mapper))
					.ToArray();

				return new TypeReference(type.Module, type.Name, kind, IsNullable, genericArguments);
			}
		}

		public override string ToString()
		{
			return $"CLR:{Type.FullName}";
		}

		#region Static members

		public static ClrTypeReference Create<T>()
		{
			return new ClrTypeReference(typeof(T));
		}
		public static ClrTypeReference Create(Type type)
		{
			return new ClrTypeReference(type);
		}

		#endregion
	}
}
