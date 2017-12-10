using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Odachi.Extensions.Formatting;

namespace Odachi.CodeGen.TypeScript.Internal
{
    public class TS
    {
		public static string Field(string name)
		{
			return name.ToCamelInvariant();
		}

		public static string Method(string name)
		{
			return name.ToCamelInvariant();
		}

		public static string ModuleName(string moduleName)
		{
			var isRelative = moduleName.StartsWith("./") || moduleName.StartsWith(".\\");

			var segments = moduleName
				.Split(new[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => string.Join("-", s.GetWords(options: WordSplitOptions.SplitOnUpperLetter).Select(w => w.ToLowerInvariant())))
				.Where(s => !string.IsNullOrEmpty(s));

			return (isRelative ? "./" : "") + string.Join("/", segments);
		}

		public static string ModuleFileName(string moduleName)
		{
			var segments = moduleName
				.Split(new[] { "/", "\\" }, StringSplitOptions.RemoveEmptyEntries)
				.Select(s => string.Join("-", s.GetWords(options: WordSplitOptions.SplitOnUpperLetter).Select(w => w.ToLowerInvariant())))
				.Where(s => !string.IsNullOrEmpty(s));

			return string.Join(Path.DirectorySeparatorChar.ToString(), segments) + ".ts";
		}
	}
}
