using Azure.Storage.Blobs;
using Odachi.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Odachi.Storage.Azure
{
	public class AzureBlob : IBlob
	{
		public AzureBlob(BlobClient blob)
		{
			Blob = blob ?? throw new ArgumentNullException(nameof(blob));
		}

		public BlobClient Blob { get; }

		public string Name => Blob.Name;

		public bool PreferAsync => true;

		public Stream OpenRead()
		{
			return Blob.OpenRead();
		}

		public Task<Stream> OpenReadAsync()
		{
			return Blob.OpenReadAsync();
		}
	}
}
