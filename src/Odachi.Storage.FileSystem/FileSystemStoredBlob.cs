using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odachi.Abstractions;

namespace Odachi.Storage.FileSystem
{
	public class FileSystemStoredBlob : FileSystemBlob, IStoredBlob
	{
		public FileSystemStoredBlob(FileSystemBlobStorage storage, string path, string fullPath) :
			base(System.IO.Path.GetFileName(path), fullPath)
		{
			Storage = storage ?? throw new ArgumentNullException(nameof(storage));
			Length = new FileInfo(fullPath).Length;
			Path = path;
		}

		public string Path { get; }

		public long Length { get; }

		public IBlobStorage Storage { get; }
	}
}
