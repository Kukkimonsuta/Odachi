using Microsoft.AspNetCore.Http;
using Odachi.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Odachi.AspNetCore.JsonRpc.Internal
{
	public class FormFileBlob : IBlob
	{
		public FormFileBlob(IFormFile file)
		{
			if (file == null)
				throw new ArgumentNullException(nameof(file));

			_file = file;
		}

		private IFormFile _file;

		public string Name => Path.GetFileName(_file.FileName);

		public bool PreferAsync => false;

		public Stream OpenRead() => _file.OpenReadStream();

		public Task<Stream> OpenReadAsync() => Task.FromResult(OpenRead());

		public void WriteTo(Stream destination)
		{
			using var stream = OpenRead();

			stream.CopyTo(destination);
		}

		public async Task WriteToAsync(Stream destination)
		{
			await using var stream = await OpenReadAsync();

			await stream.CopyToAsync(destination);
		}
	}
}
