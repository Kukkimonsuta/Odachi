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
		public FileSystemStoredBlob(FileSystemBlobStorage storage, string name, string fullPath) :
			base(name, fullPath)
		{
			Location = storage ?? throw new ArgumentNullException(nameof(storage));
			_fileInfo = new FileInfo(fullPath);
		}

		private FileInfo _fileInfo;

		public long Length => _fileInfo.Length;

		public IBlobStorage Location { get; }
	}
}
