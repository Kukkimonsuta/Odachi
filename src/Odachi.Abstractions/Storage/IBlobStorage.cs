using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Abstractions
{
	public enum BlobStoreOptions
	{
		None = 0,
		Overwrite = 1,
	}

	public interface IBlobStorage
	{
		Task StoreAsync(string relativePath, IBlob blob, BlobStoreOptions options = BlobStoreOptions.None);

		Task<IStoredBlob> RetrieveAsync(string relativePath);

		Task<bool> ExistsAsync(string relativePath);

		Task<IEnumerable<string>> ListAsync(string pattern = "**/*");

		Task DeleteAsync(string relativePath);
	}
}
