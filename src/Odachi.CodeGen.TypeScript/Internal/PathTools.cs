using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.CodeGen.TypeScript.Internal
{
	public static class PathTools
	{
		/// <summary>
		/// Returns relative file path from `from` to `to`. Note that directories must have trailing slash.
		/// </summary>
		public static string GetRelativePath(string from, string to)
		{
			if (string.IsNullOrEmpty(from))
				throw new ArgumentNullException(nameof(from));
			if (string.IsNullOrEmpty(to))
				throw new ArgumentNullException(nameof(to));

			var fromUri = new Uri($"file://{Path.GetFullPath(from)}", UriKind.Absolute);
			var toUri = new Uri($"file://{Path.GetFullPath(to)}", UriKind.Absolute);

			var relativeUri = fromUri.MakeRelativeUri(toUri);

			var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			if (!relativePath.StartsWith("."))
				relativePath = "./" + relativePath;

			return Normalize(relativePath);
		}
		
		public static string Normalize(string path)
		{
			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException(nameof(path));

			var result = path.Replace("\\", "/");

			return result;
		}

		public static bool IsRelative(string path)
		{
			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException(nameof(path));

			return path[0] == '.';
		}

		public static bool IsRelativeRoot(string path)
		{
			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException(nameof(path));

			var normalized = Normalize(path);

			return normalized == "." || normalized == "./";
		}

		public static string GetParentPath(string path)
		{
			if (string.IsNullOrEmpty(path))
				throw new ArgumentNullException(nameof(path));

			if (IsRelativeRoot(path))
				return null;

			var normalized = Normalize(path);

			var index = normalized.LastIndexOf('/');

			return normalized.Substring(0, index);
		}
	}
}
