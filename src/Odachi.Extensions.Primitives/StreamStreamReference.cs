using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odachi.Abstractions;
using System.IO;

namespace Odachi.Extensions.Primitives
{
	public class StreamStreamReference : IStreamReference, IDisposable
	{
		public StreamStreamReference(string name, Stream stream)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));

			Name = name;
			_stream = stream;
		}

		private Stream _stream;

		public string Name { get; }

		public Stream OpenReadStream()
		{
			if (_stream == null)
				throw new InvalidOperationException("StreamStreamReference supports only single stream retrieval");

			var result = _stream;
			_stream = null;
			return result;
		}

		#region IDisposable

		public void Dispose()
		{
			if (_stream != null)
			{
				_stream.Dispose();
			}
		}

		#endregion
	}
}
