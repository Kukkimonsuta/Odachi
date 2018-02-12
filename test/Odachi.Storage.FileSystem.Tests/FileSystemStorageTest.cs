using Odachi.Extensions.Primitives;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Odachi.Storage.FileSystem.Tests
{
	public class FileSystemStorageTest
	{
		[Fact]
		public async Task Store_retrieve_flow()
		{
			var path = Path.Combine(Path.GetTempPath(), "odachi-storage-filesystem-" + Guid.NewGuid());
			var storage = new FileSystemBlobStorage(path);
			try
			{
				using (var stream = new MemoryStream())
				{
					stream.Write(new byte[] { 0x01, 0x02, 0x03, 0x04 }, 0, 4);

					stream.Seek(0, SeekOrigin.Begin);
					using (var blob = new StreamBlob("data.dat", stream))
					{
						await storage.StoreAsync("./data.dat", blob);
					}
				}

				using (var stream = new MemoryStream())
				{
					stream.Write(new byte[] { 0x01, 0x02, 0x03, 0x04 }, 0, 4);

					stream.Seek(0, SeekOrigin.Begin);
					using (var blob = new StreamBlob("data.dat", stream))
					{
						await storage.StoreAsync("./some/deep/path/data.dat", blob);
					}
				}

				using (var stream = new MemoryStream())
				{
					stream.Write(new byte[] { 0x01, 0x02, 0x03, 0x04 }, 0, 4);

					stream.Seek(0, SeekOrigin.Begin);
					await Assert.ThrowsAsync<IOException>(async () =>
					{
						using (var blob = new StreamBlob("data.dat", stream))
						{
							await storage.StoreAsync("./data.dat", blob);
						}
					});
				}

				Assert.True(File.Exists(Path.Combine(path, "data.dat")));
				Assert.True(File.Exists(Path.Combine(path, "some/deep/path/data.dat")));

				Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04 }, File.ReadAllBytes(Path.Combine(path, "data.dat")));
				Assert.Equal(new byte[] { 0x01, 0x02, 0x03, 0x04 }, File.ReadAllBytes(Path.Combine(path, "some/deep/path/data.dat")));

				Assert.True(await storage.ExistsAsync("./data.dat"));
				Assert.True(await storage.ExistsAsync("./some/deep/path/data.dat"));
				Assert.False(await storage.ExistsAsync("./nope.dat"));

				{
					var retrieved = await storage.RetrieveAsync("./data.dat");

					Assert.Equal("data.dat", retrieved.Name);
					Assert.Equal("./data.dat", retrieved.Path);
					Assert.Equal(4, retrieved.Length);

					using (var retrievedStream = retrieved.OpenRead())
					{
						Assert.Equal(4, retrievedStream.Length);
					}
					using (var retrievedStream = await retrieved.OpenReadAsync())
					{
						Assert.Equal(4, retrievedStream.Length);
					}
				}
				{
					var retrieved = await storage.RetrieveAsync("./some/deep/path/data.dat");

					Assert.Equal("data.dat", retrieved.Name);
					Assert.Equal("./some/deep/path/data.dat", retrieved.Path);
					Assert.Equal(4, retrieved.Length);

					using (var retrievedStream = retrieved.OpenRead())
					{
						Assert.Equal(4, retrievedStream.Length);
					}
					using (var retrievedStream = await retrieved.OpenReadAsync())
					{
						Assert.Equal(4, retrievedStream.Length);
					}
				}
				await Assert.ThrowsAsync<FileNotFoundException>(async () =>
				{
					await storage.RetrieveAsync("./nope.dat");
				});
			}
			finally
			{
				Directory.Delete(path, true);
			}
		}
	}
}
