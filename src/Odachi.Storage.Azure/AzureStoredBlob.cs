using Microsoft.WindowsAzure.Storage.Blob;
using Odachi.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Storage.Azure
{
	public class AzureStoredBlob : AzureBlob, IStoredBlob
	{
		public AzureStoredBlob(AzureBlobStorage storage, ICloudBlob blob)
			: base(blob)
		{
			Location = storage ?? throw new ArgumentNullException(nameof(storage));
		}

		public long Length => Blob.Properties.Length;

		public IBlobStorage Location { get; }
	}
}
