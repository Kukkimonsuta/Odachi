using System;
using System.IO;
using System.Threading.Tasks;
using Odachi.Extensions.Primitives;
using Odachi.JsonRpc.Client;
using Odachi.JsonRpc.Client.Http;

namespace JsonRpcClientSample
{
	class Program
	{
		static void Main(string[] args)
		{
			MainAsync().GetAwaiter().GetResult();
		}

		static async Task MainAsync()
		{
			Console.WriteLine("Call (raw response):");
			using (var client = new JsonRpcHttpClient("http://localhost:56956/api"))
			{
				var response = await client.CallAsync("Server.listMethods", null);
				Console.WriteLine($"\t=> id: {response.Id}");
				Console.WriteLine($"\t=> result: {response.Result}");
				Console.WriteLine($"\t=> error: {response.Error}");
			}
			Console.WriteLine();

			Console.WriteLine("Call (typed response):");
			using (var client = new JsonRpcHttpClient("http://localhost:56956/api"))
			{
				var response = await client.CallAsync<string[]>("Server.listMethods", null);

				Console.WriteLine($"\t=> {string.Join(", ", response)}");
			}
			Console.WriteLine();

			Console.WriteLine("Call (typed response, stream request):");
			using (var client = new JsonRpcHttpClient("http://localhost:56956/api"))
			{
				using (var stream = new MemoryStream())
				{
					stream.Write(new byte[] { 0x01, 0x02, 0x03, 0x04 }, 0, 4);
					stream.Seek(0, SeekOrigin.Begin);

					var response = await client.CallAsync<string[]>("Storage.upload", new { file = new StreamBlob("test-file.bin", stream) });

					Console.WriteLine($"\t=> {string.Join(", ", response)}");
				}
			}
			Console.WriteLine();
		}
	}
}
