using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Odachi.CodeModel.Description.Internal
{
	public class ClassDescriptionHelper
	{
		public static string GetDisplayName(MemberInfo member)
		{
			var displayAttribute = member.GetCustomAttribute<DisplayAttribute>();
			if (displayAttribute == null)
				return null;

			var displayName = displayAttribute.GetName();
			if (string.IsNullOrEmpty(displayName))
				return null;

			return displayName;
		}

		public static bool IsRequired(MemberInfo member)
		{
			var requiredAttribute = member.GetCustomAttribute<RequiredAttribute>();
			if (requiredAttribute == null)
				return false;

			return true;
		}
	}
}
