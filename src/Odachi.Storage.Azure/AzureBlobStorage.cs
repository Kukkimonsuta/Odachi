using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Odachi.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Odachi.Storage.Azure
{
	public class AzureBlobStorage : IBlobStorage
	{
		public AzureBlobStorage(BlobServiceClient client, string containerName)
		{
			_client = client;
			ContainerName = containerName;
		}

		private BlobServiceClient _client;
		private BlobContainerClient? ____containerClient;

		public string ContainerName { get; }

		public bool PreferAsync => true;

		private BlobContainerClient GetContainerClient()
		{
			if (____containerClient != null)
			{
				return ____containerClient;
			}

			____containerClient = _client.GetBlobContainerClient(ContainerName);

			____containerClient.CreateIfNotExists();

			return ____containerClient;
		}
		private async Task<BlobContainerClient> GetContainerClientAsync()
		{
			if (____containerClient != null)
			{
				return ____containerClient;
			}

			____containerClient = _client.GetBlobContainerClient(ContainerName);
			
			await ____containerClient.CreateIfNotExistsAsync();

			return ____containerClient;
		}

		public void Store(string relativePath, IBlob blob, BlobStoreOptions options = BlobStoreOptions.None)
		{
			var containerClient = GetContainerClient();

			var blobClient = containerClient.GetBlobClient(relativePath);

			if (options != BlobStoreOptions.Overwrite && blobClient.Exists())
			{
				throw new IOException("Blob already exists");
			}

			using (var inputStream = blob.OpenRead())
			{
				blobClient.Upload(inputStream);
			}
		}
		public async Task StoreAsync(string relativePath, IBlob blob, BlobStoreOptions options = BlobStoreOptions.None)
		{
			var containerClient = await GetContainerClientAsync();

			var blobClient = containerClient.GetBlobClient(relativePath);

			if (options != BlobStoreOptions.Overwrite && await blobClient.ExistsAsync())
			{
				throw new IOException("Blob already exists");
			}

			using (var inputStream = await blob.OpenReadAsync())
			{
				await blobClient.UploadAsync(inputStream);
			}
		}

		public void Store(string relativePath, Action<Stream> write, BlobStoreOptions options = BlobStoreOptions.None)
		{
			var containerClient = GetContainerClient();

			var blobClient = containerClient.GetBlobClient(relativePath);

			if (options != BlobStoreOptions.Overwrite && containerClient.Exists())
			{
				throw new IOException("Blob already exists");
			}

			// OpenWriteAsync is no longer available
			using var bufferStream = new MemoryStream();
			write(bufferStream);
			bufferStream.Seek(0, SeekOrigin.Begin);

			blobClient.Upload(bufferStream);
		}
		public async Task StoreAsync(string relativePath, Func<Stream, Task> write, BlobStoreOptions options = BlobStoreOptions.None)
		{
			var containerClient = await GetContainerClientAsync();

			var blobClient = containerClient.GetBlobClient(relativePath);

			if (options != BlobStoreOptions.Overwrite && await containerClient.ExistsAsync())
			{
				throw new IOException("Blob already exists");
			}

			// OpenWriteAsync is no longer available
			using var bufferStream = new MemoryStream();
			await write(bufferStream);
			bufferStream.Seek(0, SeekOrigin.Begin);

			await blobClient.UploadAsync(bufferStream);
		}

		public IStoredBlob Retrieve(string relativePath)
		{
			var containerClient = GetContainerClient();

			var blobClient = containerClient.GetBlobClient(relativePath);

			if (!blobClient.Exists())
			{
				throw new FileNotFoundException();
			}

			var properties = blobClient.GetProperties();

			return new AzureStoredBlob(this, blobClient, properties.Value);
		}
		public async Task<IStoredBlob> RetrieveAsync(string relativePath)
		{
			var containerClient = await GetContainerClientAsync();

			var blobClient = containerClient.GetBlobClient(relativePath);

			if (!await blobClient.ExistsAsync())
			{
				throw new FileNotFoundException();
			}

			var properties = await blobClient.GetPropertiesAsync();

			return new AzureStoredBlob(this, blobClient, properties);
		}

		public bool Exists(string relativePath)
		{
			var containerClient = GetContainerClient();

			var blobClient = containerClient.GetBlobClient(relativePath);

			return blobClient.Exists();
		}
		public async Task<bool> ExistsAsync(string relativePath)
		{
			var containerClient = await GetContainerClientAsync();

			var blobClient = containerClient.GetBlobClient(relativePath);
			
			return await blobClient.ExistsAsync();
		}

		public IEnumerable<string> List(string pattern = "**/*")
		{
			var containerClient = GetContainerClient();

			if (pattern != "**/*")
				throw new NotSupportedException("Globs are not yet supported");

			var results = new List<string>();

			foreach (var blob in containerClient.GetBlobs())
			{
				results.Add(blob.Name);
			}

			return results;
		}
		public async Task<IEnumerable<string>> ListAsync(string pattern = "**/*")
		{
			var containerClient = await GetContainerClientAsync();

			if (pattern != "**/*")
				throw new NotSupportedException("Globs are not yet supported");

			var results = new List<string>();

			await foreach (var blob in containerClient.GetBlobsAsync())
			{
				results.Add(blob.Name);
			}

			return results;
		}

		public void Delete(string relativePath)
		{
			var containerClient = GetContainerClient();

			var blobClient = containerClient.GetBlobClient(relativePath);

			blobClient.DeleteIfExists();
		}
		public async Task DeleteAsync(string relativePath)
		{
			var containerClient = await GetContainerClientAsync();

			var blobClient = containerClient.GetBlobClient(relativePath);

			await blobClient.DeleteIfExistsAsync();
		}
	}
}
