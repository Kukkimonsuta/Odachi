using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Humanizer;
using Odachi.CodeModel.Builders;

namespace Odachi.CodeModel.Description
{
	public interface IGlobalDescriptor
	{
		/// <summary>
		/// Infer module name from fragment name.
		/// </summary>
		string GetModuleName(PackageContext context, string fragmentName);

		/// <summary>
		/// Infer fragment name from type.
		/// </summary>
		string GetFragmentName(PackageContext context, Type type);
	}

	public class DefaultGlobalDescriptor : IGlobalDescriptor
	{
		/// <inheritdoc />
		public virtual string GetModuleName(PackageContext context, string fragmentName)
		{
			return fragmentName.Underscore().Hyphenate();
		}

		/// <inheritdoc />
		public virtual string GetFragmentName(PackageContext context, Type type)
		{
			if (type.IsNested)
			{
				return $"{GetFragmentName(context, type.DeclaringType)}_{type.Name}";
			}

			return type.Name;
		}
	}
}
