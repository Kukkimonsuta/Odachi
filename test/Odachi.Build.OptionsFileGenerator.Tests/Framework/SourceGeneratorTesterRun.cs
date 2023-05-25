using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Odachi.Build.OptionsFileGenerator.Tests.Framework;

public class SourceGeneratorTesterRun<TGenerator>
    where TGenerator : ISourceGenerator
{
    public SourceGeneratorTester<TGenerator> Tester { get; }
    public string Input { get; }

    public InMemoryFileSystem FileSystem { get; } = new();
    public ISourceGenerator Generator { get; }

    public SourceGeneratorTesterRun(SourceGeneratorTester<TGenerator> tester, string input)
    {
        Tester = tester ?? throw new ArgumentNullException(nameof(tester));
        Input = input ?? throw new ArgumentNullException(nameof(input));
        Generator = Tester.GeneratorFactory(Tester, this);
    }

    public SourceGeneratorTesterRunResult<TGenerator> Execute()
    {
        // Create the 'input' compilation that the generator will act on
        var inputCompilation = CreateCompilation(Input);

        // Create the driver that will control the generation, passing in our generator
        GeneratorDriver driver = CSharpGeneratorDriver.Create(Generator);

        // Run the generation pass
        // (Note: the generator driver itself is immutable, and all calls return an updated version of the driver that you should use for subsequent calls)
        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);

        return new SourceGeneratorTesterRunResult<TGenerator>(this, driver, outputCompilation, diagnostics);
    }

    private static Compilation CreateCompilation(string source)
        => CSharpCompilation.Create(
            "compilation",
            new[]
            {
                CSharpSyntaxTree.ParseText(source)
            },
            new[]
            {
                MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location)
            },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
}
