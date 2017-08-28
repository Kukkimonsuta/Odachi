using Microsoft.AspNetCore.Http;
using Odachi.Abstractions;
using System;
using System.IO;

namespace Odachi.AspNetCore.JsonRpc.Internal
{
	public class FormFileStreamReference : IStreamReference
	{
		public FormFileStreamReference(IFormFile file)
		{
			if (file == null)
				throw new ArgumentNullException(nameof(file));

			_file = file;
		}

		private IFormFile _file;

		public string Name => _file.FileName;

		public Stream OpenReadStream() => _file.OpenReadStream();
	}
}
