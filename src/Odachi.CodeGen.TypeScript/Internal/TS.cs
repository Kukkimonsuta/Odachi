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
			switch (value)
			{
				case null:
					return "null";

				case sbyte sb:
					return sb.ToString(CultureInfo.InvariantCulture);

				case byte b:
					return b.ToString(CultureInfo.InvariantCulture);

				case short s:
					return s.ToString(CultureInfo.InvariantCulture);

				case ushort us:
					return us.ToString(CultureInfo.InvariantCulture);

				case int i:
					return i.ToString(CultureInfo.InvariantCulture);

				case uint ui:
					return ui.ToString(CultureInfo.InvariantCulture);

				case long l:
					return l.ToString(CultureInfo.InvariantCulture);

				case ulong ul:
					return ul.ToString(CultureInfo.InvariantCulture);

				case float f:
					return f.ToString(CultureInfo.InvariantCulture);

				case double d:
					return d.ToString(CultureInfo.InvariantCulture);

				case decimal m:
					return m.ToString(CultureInfo.InvariantCulture);

				case string str:
					return $"'{value}'";

				default:
					throw new InvalidOperationException($"Cannot render constant of type '{value}'");
			}
		}
	}
}
