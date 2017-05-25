using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odachi.Abstractions;

namespace Odachi.Extensions.Primitives
{
	public class FileSystemStreamReference : IStreamReference
	{
		public FileSystemStreamReference(string name, string fullPath)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			if (fullPath == null)
				throw new ArgumentNullException(nameof(fullPath));

			Name = name;
			FullPath = fullPath;
		}

		public string Name { get; }

		public string FullPath { get; }

		public Stream OpenReadStream() => File.Open(FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
	}
}
