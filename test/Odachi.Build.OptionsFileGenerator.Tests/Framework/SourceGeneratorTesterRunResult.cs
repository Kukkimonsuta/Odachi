using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Odachi.Build.OptionsFileGenerator.Tests.Framework;

public class SourceGeneratorTesterRunResult<TGenerator>
    where TGenerator : ISourceGenerator
{
    public SourceGeneratorTesterRun<TGenerator> Run { get; }

    public GeneratorDriver Driver { get; }
    public Compilation Compilation { get; }
    public ImmutableArray<Diagnostic> Diagnostics { get; }

    public SourceGeneratorTesterRunResult(SourceGeneratorTesterRun<TGenerator> run, GeneratorDriver driver, Compilation compilation, ImmutableArray<Diagnostic> diagnostics)
    {
        Run = run;
        Driver = driver;
        Compilation = compilation;
        Diagnostics = diagnostics;
    }

    public void AssertDiagnostics()
    {
        Assert.Empty(Diagnostics);
        Assert.Empty(Compilation.GetDiagnostics());
    }

    public void AssertSourceFile(string path, string expectedValue)
    {
        var generatorResult = Driver.GetRunResult().Results.Where(r => r.Generator == Run.Generator).Single();

        var generatedSource = generatorResult.GeneratedSources.Where(s => s.HintName == path).FirstOrDefault();
        Assert.NotNull(generatedSource.SourceText);

        Assert.True(SourceText.From(expectedValue).ContentEquals(generatedSource.SourceText));
    }

    public void AssertAdditionalFile(string path, string expectedValue)
    {
        Assert.Equal(expectedValue, Run.FileSystem.Get(path));
    }
}
