using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.RazorTemplating.Internal
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

			var relativePath = Uri.UnescapeDataString(relativeUri.ToString())
				.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

			return relativePath;
		}
	}
}
