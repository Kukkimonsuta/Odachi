using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Odachi.Abstractions;
using System;

namespace Odachi.Storage.Azure
{
	public class AzureStoredBlob : AzureBlob, IStoredBlob
	{
		public AzureStoredBlob(AzureBlobStorage storage, BlobClient blob, BlobProperties properties)
			: base(blob)
		{
			Storage = storage ?? throw new ArgumentNullException(nameof(storage));
			Properties = properties ?? throw new ArgumentNullException(nameof(properties));
		}

		public long Length => Properties.ContentLength;

		public IBlobStorage Storage { get; }

		public BlobProperties Properties { get; set; }

		public string Path => Blob.Name;
	}
}
