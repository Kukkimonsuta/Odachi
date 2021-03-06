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
			Module = module;
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Kind = kind;
			IsNullable = isNullable;
			GenericArguments = genericArguments ?? throw new ArgumentNullException(nameof(genericArguments));
		}

		/*
		Future?
		 generic-parameter://
		 builtin://odachi.code-gen/file
		 package://./my-dal-thing
		 package://odachi.extensions.collections/page
		*/
		public string Module { get; set; }
		public string Name { get; set; }
		public TypeKind Kind { get; set; }
		public bool IsNullable { get; set; }
		public TypeReference[] GenericArguments { get; set; }

		public override string ToString()
		{
			return $"{Name}{(GenericArguments.Length > 0 ? $"<{string.Join(", ", GenericArguments.Select(a => a.ToString()))}>" : "")}{(IsNullable ? "?" : "")}";
		}
	}
}
