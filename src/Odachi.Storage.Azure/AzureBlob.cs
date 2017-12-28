using Microsoft.WindowsAzure.Storage.Blob;
using Odachi.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Storage.Azure
{
	public class AzureBlob : IBlob
	{
		public AzureBlob(ICloudBlob blob)
		{
			Blob = blob ?? throw new ArgumentNullException(nameof(blob));
		}

		public ICloudBlob Blob { get; }

		public string Name => Blob.Name;

		public bool PreferAsync => true;

		public Stream OpenRead()
		{
			return OpenReadAsync().GetAwaiter().GetResult();
		}

		public Task<Stream> OpenReadAsync()
		{
			return Blob.OpenReadAsync(null, null, null);
		}
	}
}
