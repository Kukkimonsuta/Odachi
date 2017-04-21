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
		/// Returns relative path from `from` to `to`. Note that directories must have trailing slash.
		/// </summary>
		public static string GetRelativePath(string from, string to)
		{
			if (string.IsNullOrEmpty(from))
				throw new ArgumentNullException(nameof(from));
			if (string.IsNullOrEmpty(to))
				throw new ArgumentNullException(nameof(to));

			var fromUri = new Uri(Path.GetFullPath(from), UriKind.Absolute);
			var toUri = new Uri(Path.GetFullPath(to), UriKind.Absolute);

			if (fromUri.Scheme != toUri.Scheme)
				throw new InvalidOperationException($"Cannot form relative path using different schemes");

			var relativeUri = fromUri.MakeRelativeUri(toUri);
			var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			if (toUri.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase))
				relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

			return relativePath;
		}
	}
}
