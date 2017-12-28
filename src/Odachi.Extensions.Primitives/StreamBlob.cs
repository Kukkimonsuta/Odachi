using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odachi.Abstractions;
using System.IO;

namespace Odachi.Extensions.Primitives
{
	public class StreamBlob : IBlob, IDisposable
	{
		public StreamBlob(string name, Func<Stream> openReadStream)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			_openRead = openReadStream ?? throw new ArgumentNullException(nameof(openReadStream));
		}
		public StreamBlob(string name, Func<Task<Stream>> openReadStreamAsync)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			_openReadAsync = openReadStreamAsync ?? throw new ArgumentNullException(nameof(openReadStreamAsync));
		}
		public StreamBlob(string name, Stream stream)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			_stream = stream ?? throw new ArgumentNullException(nameof(stream));
		}

		private Stream _stream;
		private Func<Stream> _openRead;
		private Func<Task<Stream>> _openReadAsync;

		public string Name { get; }

		public bool PreferAsync => _openReadAsync != null;

		public Stream OpenRead()
		{
			if (_openReadAsync != null)
			{
				return _openReadAsync().GetAwaiter().GetResult();
			}
			else if (_openRead != null)
			{
				return _openRead();
			}
			else
			{
				if (_stream == null)
					throw new InvalidOperationException("StreamStreamReference supports only single stream retrieval");

				var result = _stream;
				_stream = null;
				return result;
			}
		}

		public Task<Stream> OpenReadAsync()
		{
			if (_openReadAsync != null)
			{
				return _openReadAsync();
			}
			else if (_openRead != null)
			{
				return Task.FromResult(_openRead());
			}
			else
			{
				if (_stream == null)
					throw new InvalidOperationException($"{nameof(StreamBlob)} supports only single stream retrieval");

				var result = _stream;
				_stream = null;
				return Task.FromResult(result);
			}
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
