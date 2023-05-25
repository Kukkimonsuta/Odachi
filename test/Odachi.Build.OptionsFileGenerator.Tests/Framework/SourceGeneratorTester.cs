using System;
using Microsoft.CodeAnalysis;

namespace Odachi.Build.OptionsFileGenerator.Tests.Framework;

public abstract class SourceGeneratorTester
{
    public static SourceGeneratorTesterRunResult<TGenerator> Run<TGenerator>(
        Func<SourceGeneratorTester<TGenerator>, SourceGeneratorTesterRun<TGenerator>, TGenerator> generatorFactory,
        string input
    )
        where TGenerator : ISourceGenerator
    {
        var sourceGeneratorTester = new SourceGeneratorTester<TGenerator>(generatorFactory);
        var sourceGeneratorTesterRun = sourceGeneratorTester.CreateRun(input);
        var sourceGeneratorTesterRunResult = sourceGeneratorTesterRun.Execute();

        return sourceGeneratorTesterRunResult;
    }
}

public class SourceGeneratorTester<TGenerator> : SourceGeneratorTester
    where TGenerator : ISourceGenerator
{
    public Func<SourceGeneratorTester<TGenerator>, SourceGeneratorTesterRun<TGenerator>, TGenerator> GeneratorFactory { get; }

    public SourceGeneratorTester(Func<SourceGeneratorTester<TGenerator>, SourceGeneratorTesterRun<TGenerator>, TGenerator> generatorFactory)
    {
        GeneratorFactory = generatorFactory ?? throw new ArgumentNullException(nameof(generatorFactory));
    }

    public SourceGeneratorTesterRun<TGenerator> CreateRun(string input)
    {
        return new SourceGeneratorTesterRun<TGenerator>(this, input);
    }
}
