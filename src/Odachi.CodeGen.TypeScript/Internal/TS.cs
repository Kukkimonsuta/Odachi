using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel;
using Odachi.Extensions.Formatting;

namespace Odachi.CodeGen.TypeScript.Internal
{
	public class TS
	{
		public static string Type(string name)
		{
			return name.ToPascalInvariant();
		}

		public static string Field(string name)
		{
			return name.ToCamelInvariant();
		}

		public static string Method(string name)
		{
			return name.ToCamelInvariant();
		}

		public static string Parameter(string name)
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

		public static string Constant(object value)
		{
			return value switch
			{
				null => "null",
				sbyte sb => sb.ToString(CultureInfo.InvariantCulture),
				byte b => b.ToString(CultureInfo.InvariantCulture),
				short s => s.ToString(CultureInfo.InvariantCulture),
				ushort us => us.ToString(CultureInfo.InvariantCulture),
				int i => i.ToString(CultureInfo.InvariantCulture),
				uint ui => ui.ToString(CultureInfo.InvariantCulture),
				long l => l.ToString(CultureInfo.InvariantCulture),
				ulong ul => ul.ToString(CultureInfo.InvariantCulture),
				float f => f.ToString(CultureInfo.InvariantCulture),
				double d => d.ToString(CultureInfo.InvariantCulture),
				decimal m => m.ToString(CultureInfo.InvariantCulture),
				string str => String(str),
				_ => throw new InvalidOperationException($"Cannot render constant of type '{value}'"),
			};
		}

		public static string String(string value)
		{
			return $"'{value.Replace("'", "\\'")}'";
		}
	}
}
