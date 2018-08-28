using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Odachi.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Odachi.Storage.Azure
{
	public class AzureBlobStorage : IBlobStorage
	{
		public AzureBlobStorage(CloudBlobClient client, string containerName)
		{
			_client = client;
			ContainerName = containerName;
		}

		private CloudBlobClient _client;
		private CloudBlobContainer _container;

		public string ContainerName { get; }

		public bool PreferAsync => true;

		private Task EnsureContainerExists()
		{
			if (_container != null)
				return Task.CompletedTask;

			_container = _client.GetContainerReference(ContainerName);

			return _container.CreateIfNotExistsAsync();
		}

		public void Store(string relativePath, IBlob blob, BlobStoreOptions options = BlobStoreOptions.None)
		{
			StoreAsync(relativePath, blob, options: options).GetAwaiter().GetResult();
		}
		public async Task StoreAsync(string relativePath, IBlob blob, BlobStoreOptions options = BlobStoreOptions.None)
		{
			await EnsureContainerExists();

			if (options != BlobStoreOptions.Overwrite && await _container.GetBlobReference(relativePath).ExistsAsync())
			{
				throw new IOException("Blob already exists");
			}

			using (var inputStream = await blob.OpenReadAsync())
			{
				var remoteBlob = await _container.GetBlobReferenceFromServerAsync(relativePath);

				await remoteBlob.UploadFromStreamAsync(inputStream);
			}
		}

		public void Store(string relativePath, Action<Stream> write, BlobStoreOptions options = BlobStoreOptions.None)
		{
			StoreAsync(relativePath, (s) =>
			{
				write(s);

				return Task.CompletedTask;
			}, options: options).GetAwaiter().GetResult();
		}
		public async Task StoreAsync(string relativePath, Func<Stream, Task> write, BlobStoreOptions options = BlobStoreOptions.None)
		{
			await EnsureContainerExists();

			if (options != BlobStoreOptions.Overwrite && await _container.GetBlobReference(relativePath).ExistsAsync())
			{
				throw new IOException("Blob already exists");
			}

			var remoteBlob = await _container.GetBlobReferenceFromServerAsync(relativePath);

			using (var outputStream = await ((CloudBlockBlob)remoteBlob).OpenWriteAsync())
			{
				await write(outputStream);
			}
		}

		public IStoredBlob Retrieve(string relativePath)
		{
			return RetrieveAsync(relativePath).GetAwaiter().GetResult();
		}
		public async Task<IStoredBlob> RetrieveAsync(string relativePath)
		{
			await EnsureContainerExists();

			if (!await _container.GetBlobReference(relativePath).ExistsAsync())
			{
				throw new FileNotFoundException();
			}

			var remoteBlob = await _container.GetBlobReferenceFromServerAsync(relativePath);

			await remoteBlob.FetchAttributesAsync();

			return new AzureStoredBlob(this, remoteBlob);
		}

		public bool Exists(string relativePath)
		{
			return ExistsAsync(relativePath).GetAwaiter().GetResult();
		}
		public async Task<bool> ExistsAsync(string relativePath)
		{
			await EnsureContainerExists();

			return await _container.GetBlobReference(relativePath).ExistsAsync();
		}

		public IEnumerable<string> List(string pattern = "**/*")
		{
			return ListAsync(pattern: pattern).GetAwaiter().GetResult();
		}
		public async Task<IEnumerable<string>> ListAsync(string pattern = "**/*")
		{
			await EnsureContainerExists();

			if (pattern != "**/*")
				throw new NotSupportedException("Globs are not yet supported");

			var results = new List<string>();

			var token = default(BlobContinuationToken);
			do
			{
				var list = await _container.ListBlobsSegmentedAsync(token);

				foreach (var item in list.Results)
				{
					results.Add(item.Uri.ToString());
				}

				token = list.ContinuationToken;
			}
			while (token != null);

			return results;
		}

		public void Delete(string relativePath)
		{
			DeleteAsync(relativePath).GetAwaiter().GetResult();
		}
		public async Task DeleteAsync(string relativePath)
		{
			await EnsureContainerExists();

			await _container.GetBlobReference(relativePath).DeleteIfExistsAsync();
		}
	}
}
