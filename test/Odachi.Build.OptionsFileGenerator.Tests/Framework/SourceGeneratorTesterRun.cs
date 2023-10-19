using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Odachi.Build.OptionsFileGenerator.Tests.Framework;

public interface ISourceGeneratorTesterRun
{
	string[] Inputs { get; }
	SourceGeneratorTester Tester { get; }
	InMemoryFileSystem FileSystem { get; }
	IIncrementalGenerator Generator { get; }
}

public class SourceGeneratorTesterRun<TGenerator> : ISourceGeneratorTesterRun
    where TGenerator : IIncrementalGenerator
{
    public SourceGeneratorTester<TGenerator> Tester { get; }
    public string[] Inputs { get; }
    public InMemoryFileSystem FileSystem { get; } = new();
    public IIncrementalGenerator Generator { get; }

    public SourceGeneratorTesterRun(SourceGeneratorTester<TGenerator> tester, string[] inputs)
    {
        Tester = tester ?? throw new ArgumentNullException(nameof(tester));
        Inputs = inputs ?? throw new ArgumentNullException(nameof(inputs));
        Generator = Tester.GeneratorFactory(Tester, this);
    }

    public SourceGeneratorTesterRunResult<TGenerator> Execute()
    {
        // Create the 'input' compilation that the generator will act on
        var inputCompilation = CreateCompilation(Inputs);

        // Create the driver that will control the generation, passing in our generator
        GeneratorDriver driver = CSharpGeneratorDriver.Create(Generator);

        // Run the generation pass
        // (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

        return new SourceGeneratorTesterRunResult<TGenerator>(this, driver, outputCompilation, diagnostics);
    }

    #region ISourceGeneratorTesterRun

    SourceGeneratorTester ISourceGeneratorTesterRun.Tester => Tester;

    #endregion

    private static Compilation CreateCompilation(IEnumerable<string> sources)
        => CSharpCompilation.Create(
            "compilation",
            sources.Select(source => CSharpSyntaxTree.ParseText(source)),
            new[]
            {
                MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location)
            },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
}
