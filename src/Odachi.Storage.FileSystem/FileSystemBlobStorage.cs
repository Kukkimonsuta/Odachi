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
			RootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
		}

		public string RootPath { get; }

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

		public async Task StoreAsync(string relativePath, IBlob blob, BlobStoreOptions options = BlobStoreOptions.None)
		{
			var absolutePath = ResolvePath(relativePath, createDirectories: true);

			using (var inputStream = blob.PreferAsync ? await blob.OpenReadAsync() : blob.OpenRead())
			using (var outputStream = new FileStream(absolutePath, options == BlobStoreOptions.Overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write, FileShare.Read))
			{
				await inputStream.CopyToAsync(outputStream);
			}
		}

		public Task<IBlob> RetrieveAsync(string relativePath)
		{
			var absolutePath = ResolvePath(relativePath);

			if (!File.Exists(absolutePath))
				throw new FileNotFoundException();

			return Task.FromResult<IBlob>(
				new FileSystemBlob(Path.GetFileName(absolutePath), absolutePath)
			);
		}

		public Task<bool> ExistsAsync(string relativePath)
		{
			var absolutePath = ResolvePath(relativePath);

			return Task.FromResult(
				File.Exists(absolutePath)
			);
		}

		public Task<IEnumerable<string>> ListAsync(string pattern = "**/*")
		{
			// todo: implement glob matching
			if (pattern != "**/*")
				throw new NotImplementedException("Patterns are not yet implemented");

			var absolutePath = ResolvePath(".");
			if (!Directory.Exists(absolutePath))
			{
				return Task.FromResult(Enumerable.Empty<string>());
			}

			var result = new List<string>();

			foreach (var entry in Directory.GetFiles(absolutePath, "*", SearchOption.AllDirectories))
			{
				if (!entry.StartsWith(RootPath))
					throw new InvalidOperationException("Listing entries cannot escape root directory");

				var path = entry.Substring(RootPath.Length).TrimStart('\\', '/');

				result.Add(path);
			}

			return Task.FromResult<IEnumerable<string>>(result);
		}

		public Task DeleteAsync(string relativePath)
		{
			var absolutePath = ResolvePath(relativePath);

			if (File.Exists(absolutePath))
			{
				File.Delete(absolutePath);
			}

			return Task.CompletedTask;
		}
	}
}
