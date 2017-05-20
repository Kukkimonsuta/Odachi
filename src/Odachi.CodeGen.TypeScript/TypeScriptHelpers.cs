using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Odachi.CodeModelGen.TypeScript
{
	public static class TypeScriptHelpers
    {
		private static string GetCleanTypeName(Type type)
		{
			var name = type.Name;

			if (type.GetTypeInfo().IsGenericType)
			{
				name = name.Remove(name.IndexOf('`'));
			}

			return name;
		}

        public static string GetModuleName(Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			var result = GetCleanTypeName(type);

			var parentType = type.DeclaringType;
			while (parentType != null)
			{
				result = $"{GetCleanTypeName(parentType)}_{result}";

				parentType = parentType.DeclaringType;
			}

			return result;
		}

		public static string GetModulePath(Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			return GetModulePath(GetModuleName(type), isInterface: type.GetTypeInfo().IsInterface);
		}
		public static string GetModulePath(string typeName, bool isInterface = false)
		{
			if (typeName == null)
				throw new ArgumentNullException(nameof(typeName));

			if (isInterface && typeName.StartsWith("I") && char.IsUpper(typeName, 1))
			{
				typeName = typeName.Substring(1);
			}

			typeName = typeName.Replace('_', '-');
			typeName = Regex.Replace(typeName, "(?<=.)([A-Z])", "-$0").ToLowerInvariant();
			typeName = Regex.Replace(typeName, "--+", "-");

			return "./" + typeName;
		}

		public static string GetPropertyName(PropertyInfo property)
		{
			if (property == null)
				throw new ArgumentNullException(nameof(property));

			return GetPropertyName(property.Name);
		}
		public static string GetPropertyName(string propertyName)
		{
			if (propertyName == null)
				throw new ArgumentNullException(nameof(propertyName));
			if (propertyName.Length <= 0)
				throw new ArgumentOutOfRangeException(nameof(propertyName));

			return Regex.Replace(propertyName, "(^|_)[A-Z]+", m => m.Value.ToLowerInvariant());
		}
	}
}
