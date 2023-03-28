using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odachi.Abstractions;

namespace Odachi.Storage.FileSystem
{
	public class FileSystemBlob : IBlob
	{
		public FileSystemBlob(string fullPath)
		{
			FullPath = fullPath ?? throw new ArgumentNullException(nameof(fullPath));
			Name = Path.GetFileName(fullPath);
		}
		public FileSystemBlob(string name, string fullPath)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			FullPath = fullPath ?? throw new ArgumentNullException(nameof(fullPath));
		}

		public string Name { get; }

		public string FullPath { get; }

		public bool PreferAsync => false;

		public Stream OpenRead()
		{
			return File.Open(FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		public Task<Stream> OpenReadAsync()
		{
			return Task.FromResult(OpenRead());
		}

		public void WriteTo(Stream destination)
		{
			using var stream = OpenRead();

			stream.CopyTo(destination);
		}

		public async Task WriteToAsync(Stream destination)
		{
			using var stream = await OpenReadAsync();

			await stream.CopyToAsync(destination);
		}
	}
}
