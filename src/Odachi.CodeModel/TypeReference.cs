using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odachi.CodeModel.Mapping;
using System.Reflection;

namespace Odachi.CodeModel
{
	public enum TypeKind
	{
		Array,
		Enum,
		Primitive,
		Interface,
		Struct,
		Class,
		GenericParameter,
		Tuple,
	}

	public static class TypeKindExtensions
	{
		private static readonly HashSet<Type> ValueTupleTypes = new HashSet<Type>(new Type[]
		{
			typeof(ValueTuple<>),
			typeof(ValueTuple<,>),
			typeof(ValueTuple<,,>),
			typeof(ValueTuple<,,,>),
			typeof(ValueTuple<,,,,>),
			typeof(ValueTuple<,,,,,>),
			typeof(ValueTuple<,,,,,,>),
			typeof(ValueTuple<,,,,,,,>),
		});

		public static TypeKind GetTypeKind(this Type type)
		{
			return type.GetTypeInfo().GetTypeKind();
		}
		public static TypeKind GetTypeKind(this TypeInfo info)
		{
			if (info.IsGenericParameter)
				return TypeKind.GenericParameter;

			var underlyingType = Nullable.GetUnderlyingType(info.UnderlyingSystemType) ?? info.UnderlyingSystemType;
			var underlyingTypeInfo = underlyingType.GetTypeInfo();

			if (underlyingTypeInfo.IsGenericType && ValueTupleTypes.Contains(underlyingType.GetGenericTypeDefinition()))
				return TypeKind.Tuple;

			if (underlyingTypeInfo.IsArray)
				return TypeKind.Array;

			if (underlyingTypeInfo.IsEnum)
				return TypeKind.Enum;

			if (underlyingTypeInfo.IsPrimitive)
				return TypeKind.Primitive;

			if (underlyingTypeInfo.IsInterface)
				return TypeKind.Interface;

			if (underlyingTypeInfo.IsValueType)
				return TypeKind.Struct;

			return TypeKind.Class;
		}
	}

	public class TypeReference
	{
		public TypeReference(string module, string name, TypeKind kind, bool isNullable, params TypeReference[] genericArguments)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			if (genericArguments == null)
				throw new ArgumentNullException(nameof(genericArguments));

			Module = module;
			Name = name;
			Kind = kind;
			IsNullable = isNullable;
			GenericArguments = genericArguments;
		}

		public string Module { get; }
		public string Name { get; }
		public TypeKind Kind { get; }
		public bool IsNullable { get; set; }
		public TypeReference[] GenericArguments { get; }

		public override string ToString()
		{
			return $"{Name}{(GenericArguments.Length > 0 ? $"<{string.Join(", ", GenericArguments.Select(a => a.ToString()))}>" : "")}{(IsNullable ? "?" : "")}";
		}
	}
}
