using System;
using System.Collections.Generic;
using System.IO;
using Odachi.Build.OptionsFileGenerator.Model;

namespace Odachi.Build.OptionsFileGenerator.IO;

public abstract class OptionsFileWriter : IDisposable
{
    public string[] RootKey { get; }
    protected readonly TextWriter Writer;

    protected OptionsFileWriter(TextWriter writer, string? rootKey)
        : this(writer, (rootKey ?? "").Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries))
    {
    }
    protected OptionsFileWriter(TextWriter writer, string[]? rootKey)
    {
        Writer = writer ?? throw new ArgumentNullException(nameof(writer));
        RootKey = rootKey ?? Array.Empty<string>();
    }

    public abstract void Write(IReadOnlyList<OptionsProperty> properties);

    public void Dispose()
    {
        Writer.Dispose();
    }
}
