using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Abstractions;

public enum BlobStoreOptions
{
	None = 0,
	Overwrite = 1,
}

public interface IBlobStorage
{
	/// <summary>
	/// Returns whether it is preffered to use async. Not respecting this value can lead to degraded perfromance.
	/// </summary>
	bool PreferAsync { get; }

	/// <summary>
	/// Store a blob in storage on given path.
	/// </summary>
	/// <param name="relativePath">Store relative path.</param>
	/// <param name="blob">Blob to be stored.</param>
	/// <param name="options">Additional options for store operation.</param>
	void Store(string relativePath, IBlob blob, BlobStoreOptions options = BlobStoreOptions.None);
	/// <summary>
	/// Store a blob in storage on given path.
	/// </summary>
	/// <param name="relativePath">Store relative path.</param>
	/// <param name="blob">Blob to be stored.</param>
	/// <param name="options">Additional options for store operation.</param>
	Task StoreAsync(string relativePath, IBlob blob, BlobStoreOptions options = BlobStoreOptions.None);

	/// <summary>
	/// Retrieve a blob from storage that resides on given path.
	/// </summary>
	/// <param name="relativePath">Store relative path.</param>
	IStoredBlob Retrieve(string relativePath);
	/// <summary>
	/// Retrieve a blob from storage that resides on given path.
	/// </summary>
	/// <param name="relativePath">Store relative path.</param>
	Task<IStoredBlob> RetrieveAsync(string relativePath);

	/// <summary>
	/// Check whether a blob exists on given path.
	/// </summary>
	/// <param name="relativePath">Store relative path.</param>
	bool Exists(string relativePath);
	/// <summary>
	/// Check whether a blob exists on given path.
	/// </summary>
	/// <param name="relativePath">Store relative path.</param>
	Task<bool> ExistsAsync(string relativePath);

	/// <summary>
	/// Retrieve all blobs matching given pattern.
	/// </summary>
	/// <param name="pattern">Pattern against which blob paths are tested.</param>
	IEnumerable<string> List(string pattern = "**/*");
	/// <summary>
	/// Retrieve all blobs matching given pattern.
	/// </summary>
	/// <param name="pattern">Pattern against which blob paths are tested.</param>
	Task<IEnumerable<string>> ListAsync(string pattern = "**/*");

	/// <summary>
	/// Delete blob from storage.
	/// </summary>
	/// <param name="relativePath">Store relative path.</param>
	void Delete(string relativePath);
	/// <summary>
	/// Delete blob from storage.
	/// </summary>
	/// <param name="relativePath">Store relative path.</param>
	Task DeleteAsync(string relativePath);
}
