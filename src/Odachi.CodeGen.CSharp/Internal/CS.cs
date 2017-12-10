using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Odachi.Extensions.Formatting;

namespace Odachi.CodeGen.CSharp.Internal
{
	public class CS
	{
		public static string Field(string name)
		{
			return name.ToPascalInvariant();
		}

		public static string Method(string name)
		{
			return name.ToPascalInvariant();
		}

		public static string ModuleFileName(string moduleName)
		{
			var segments = moduleName
				.ToPascalInvariant()
				.Split(new[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => string.Join("", s.GetWords()))
				.Where(s => !string.IsNullOrEmpty(s));

			return string.Join(Path.DirectorySeparatorChar.ToString(), segments) + ".cs";
		}

		public static string ModuleNamespace(string packageNamespace, string moduleName)
		{
			var moduleNamespace = Namespace(Path.GetDirectoryName(moduleName));

			var dot = string.IsNullOrEmpty(packageNamespace) || string.IsNullOrEmpty(moduleNamespace) ? "" : ".";

			return $"{packageNamespace}{dot}{moduleNamespace}";
		}

		public static string Namespace(string name)
		{
			var segments = name
				.ToPascalInvariant()
				.Split(new[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => string.Join("", s.GetWords()))
				.Where(s => !string.IsNullOrEmpty(s));

			return string.Join(".", segments);
		}
	}
}
