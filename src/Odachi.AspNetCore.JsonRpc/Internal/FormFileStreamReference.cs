using Microsoft.AspNetCore.Http;
using Odachi.Abstractions;
using System;
using System.IO;

namespace Odachi.AspNetCore.JsonRpc.Internal
{
#pragma warning disable CS0618 // Type or member is obsolete
	public class FormFileStreamReference : IStreamReference
#pragma warning restore CS0618 // Type or member is obsolete
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
