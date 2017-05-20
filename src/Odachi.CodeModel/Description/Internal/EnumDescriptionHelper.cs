using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Odachi.CodeModel.Description.Internal
{
	public class EnumDescriptionHelper
	{
		private static bool IsStaticClass(Type type)
		{
			return IsStaticClass(type.GetTypeInfo());
		}
		private static bool IsStaticClass(TypeInfo type)
		{
			return type.IsAbstract && type.IsSealed;
		}

		private static MethodInfo FindGetDisplayName(Type @for)
		{
			foreach (var staticType in @for.GetTypeInfo().Assembly.GetTypes().Where(IsStaticClass))
			{
				foreach (var staticMethod in staticType.GetMethods(BindingFlags.Static | BindingFlags.Public))
				{
					if (staticMethod.Name != "GetDisplayName")
						continue;

					var parameters = staticMethod.GetParameters();
					if (parameters.Length <= 0 || parameters[0].ParameterType != @for)
						continue;

					if (parameters.Skip(1).Any(p => !p.IsOptional))
						continue;

					return staticMethod;
				}
			}

			return null;
		}

		public static string GetDisplayName(Type @for, object value)
		{
			if (!TryGetDisplayName(@for, value, out var result))
				throw new InvalidOperationException($"Enum '{@for.FullName}' doesn't have GetDisplayName extension defined");

			return result;
		}

		public static bool TryGetDisplayName(Type @for, object value, out string displayName)
		{
			var method = FindGetDisplayName(@for);
			if (method == null)
			{
				displayName = null;
				return false;
			}

			displayName = (string)method.Invoke(null, new[] { value });
			return true;
		}
	}
}
