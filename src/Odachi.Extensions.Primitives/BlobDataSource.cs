using System;
using System.IO;
using System.Threading.Tasks;
using Odachi.Abstractions;

namespace Odachi.Extensions.Primitives;

public class BlobDataSource : IBlobDataSource
{
	private readonly Action<Stream>? _writeTo;
	private readonly Func<Stream, Task>? _writeToAsync;

	public BlobDataSource(Action<Stream> writeTo)
	{
		_writeTo = writeTo ?? throw new ArgumentNullException(nameof(writeTo));
	}
	public BlobDataSource(Func<Stream, Task> writeToAsync)
	{
		_writeToAsync = writeToAsync ?? throw new ArgumentNullException(nameof(writeToAsync));
	}

	public bool PreferAsync => _writeToAsync != null;

	public void WriteTo(Stream stream)
	{
		if (_writeToAsync != null)
		{
			_writeToAsync(stream).GetAwaiter().GetResult();
		}
		else if (_writeTo != null)
		{
			_writeTo(stream);
		}
		else
		{
			// this should never happen
			throw new InvalidOperationException("Neither sync nor async writeTo present");
		}
	}

	public Task WriteToAsync(Stream stream)
	{
		if (_writeToAsync != null)
		{
			return _writeToAsync(stream);
		}
		else if (_writeTo != null)
		{
			_writeTo(stream);
			return Task.CompletedTask;
		}
		else
		{
			// this should never happen
			throw new InvalidOperationException("Neither sync nor async writeTo present");
		}
	}
}
