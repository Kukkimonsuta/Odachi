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
	}

	public static class TypeKindExtensions
	{
		public static TypeKind GetTypeKind(this Type type)
		{
			return type.GetTypeInfo().GetTypeKind();
		}
		public static TypeKind GetTypeKind(this TypeInfo info)
		{
			if (info.IsArray)
				return TypeKind.Array;

			if (info.IsEnum)
				return TypeKind.Enum;

			if (info.IsPrimitive)
				return TypeKind.Primitive;

			if (info.IsInterface)
				return TypeKind.Interface;

			if (info.IsValueType)
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
