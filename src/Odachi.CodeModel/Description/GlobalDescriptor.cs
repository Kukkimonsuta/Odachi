using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders;
using Odachi.Extensions.Formatting;

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
			return string.Join("-", fragmentName.GetWords());
		}

		/// <inheritdoc />
		public virtual string GetFragmentName(PackageContext context, Type type)
		{
			var typeName = type.GetTypeInfo().IsGenericType ? type.Name.Remove(type.Name.IndexOf('`')) : type.Name;

			if (type.IsNested)
			{
				return $"{GetFragmentName(context, type.DeclaringType)}_{typeName}";
			}

			return typeName;
		}
	}
}
