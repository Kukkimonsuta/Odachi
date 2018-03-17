using Odachi.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Storage.FileSystem
{
	public class FileSystemBlobStorage : IBlobStorage
	{
		public FileSystemBlobStorage(string rootPath)
		{
			if (rootPath == null)
				throw new ArgumentNullException(nameof(rootPath));

			RootPath = Path.GetFullPath(rootPath);
		}

		public string RootPath { get; }

		public bool PreferAsync => false;

		private string ResolvePath(string relativePath, bool createDirectories = false)
		{
			var result = Path.GetFullPath(Path.Combine(RootPath, relativePath));

			if (!result.StartsWith(RootPath))
				throw new ArgumentException("Path must remain within root directory", nameof(relativePath));

			if (createDirectories)
			{
				var directory = Path.GetDirectoryName(result);

				if (!Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}
			}

			return result;
		}

		public void Store(string relativePath, IBlob blob, BlobStoreOptions options = BlobStoreOptions.None)
		{
			var absolutePath = ResolvePath(relativePath, createDirectories: true);

			using (var inputStream = blob.OpenRead())
			using (var outputStream = new FileStream(absolutePath, options == BlobStoreOptions.Overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.Read))
			{
				inputStream.CopyTo(outputStream);
			}
		}
		public async Task StoreAsync(string relativePath, IBlob blob, BlobStoreOptions options = BlobStoreOptions.None)
		{
			var absolutePath = ResolvePath(relativePath, createDirectories: true);

			using (var inputStream = blob.PreferAsync ? await blob.OpenReadAsync() : blob.OpenRead())
			using (var outputStream = new FileStream(absolutePath, options == BlobStoreOptions.Overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.Read))
			{
				await inputStream.CopyToAsync(outputStream);
			}
		}

		public void StoreAsync(string relativePath, Action<Stream> write, BlobStoreOptions options = BlobStoreOptions.None)
		{
			var absolutePath = ResolvePath(relativePath, createDirectories: true);

			using (var outputStream = new FileStream(absolutePath, options == BlobStoreOptions.Overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.Read))
			{
				write(outputStream);
			}
		}
		public async Task StoreAsync(string relativePath, Func<Stream, Task> write, BlobStoreOptions options = BlobStoreOptions.None)
		{
			var absolutePath = ResolvePath(relativePath, createDirectories: true);

			using (var outputStream = new FileStream(absolutePath, options == BlobStoreOptions.Overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.Read))
			{
				await write(outputStream);
			}
		}

		public IStoredBlob Retrieve(string relativePath)
		{
			var absolutePath = ResolvePath(relativePath);

			if (!File.Exists(absolutePath))
				throw new FileNotFoundException();

			// todo: path should be normalized
			return new FileSystemStoredBlob(this, relativePath, absolutePath);
		}
		public Task<IStoredBlob> RetrieveAsync(string relativePath)
		{
			var result = Retrieve(relativePath);

			return Task.FromResult<IStoredBlob>(result);
		}

		public bool Exists(string relativePath)
		{
			var absolutePath = ResolvePath(relativePath);

			return File.Exists(absolutePath);
		}
		public Task<bool> ExistsAsync(string relativePath)
		{
			var result = Exists(relativePath);

			return Task.FromResult(result);
		}

		public IEnumerable<string> List(string pattern = "**/*")
		{
			// todo: implement glob matching
			if (pattern != "**/*")
				throw new NotImplementedException("Patterns are not yet implemented");

			var absolutePath = ResolvePath(".");
			if (!Directory.Exists(absolutePath))
			{
				return Enumerable.Empty<string>();
			}

			var result = new List<string>();

			foreach (var entry in Directory.GetFiles(absolutePath, "*", SearchOption.AllDirectories))
			{
				if (!entry.StartsWith(RootPath))
					throw new InvalidOperationException("Listing entries cannot escape root directory");

				var path = entry.Substring(RootPath.Length).TrimStart('\\', '/');

				result.Add(path);
			}

			return result;
		}
		public Task<IEnumerable<string>> ListAsync(string pattern = "**/*")
		{
			var result = List(pattern: pattern);

			return Task.FromResult<IEnumerable<string>>(result);
		}

		public void Delete(string relativePath)
		{
			var absolutePath = ResolvePath(relativePath);

			if (File.Exists(absolutePath))
			{
				File.Delete(absolutePath);
			}
		}
		public Task DeleteAsync(string relativePath)
		{
			Delete(relativePath);

			return Task.CompletedTask;
		}
	}
}
