using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Odachi.Build.OptionsFileGenerator.Tests.Framework;

/// <summary>
/// Primitive in memory file system for text files. This is purely for unit test purposes. Doesn't support any concurrency.
/// </summary>
public class InMemoryFileSystem
{
    private readonly Dictionary<string, MemoryStream> _storage = new();

    public StreamWriter Create(string path)
    {
        var stream = new MemoryStream();

        if (_storage.TryGetValue(path, out var oldStream))
        {
            oldStream.Dispose();
        }

        _storage[path] = stream;

        return new StreamWriter(stream, Encoding.UTF8, leaveOpen: true);
    }

    public StreamReader Open(string path)
    {
        if (!_storage.TryGetValue(path, out var stream))
        {
            throw new FileNotFoundException();
        }

        stream.Seek(0, SeekOrigin.Begin);
        return new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
    }

    public string Get(string path)
    {
        using var reader = Open(path);

        return reader.ReadToEnd();
    }

    public bool Delete(string path)
    {
        if (!_storage.TryGetValue(path, out var stream))
        {
            return false;
        }

        stream.Dispose();
        _storage.Remove(path);

        return true;
    }

    public IEnumerable<string> List()
    {
        return _storage.Keys;
    }
}
