using System;
using Microsoft.CodeAnalysis;

namespace Odachi.Build.OptionsFileGenerator.Tests.Framework;

public abstract class SourceGeneratorTester
{
	public static SourceGeneratorTesterRunResult<TGenerator> Run<TGenerator>(
		Func<SourceGeneratorTester<TGenerator>, SourceGeneratorTesterRun<TGenerator>, TGenerator> generatorFactory,
		string input
	)
		where TGenerator : IIncrementalGenerator
	{
		var sourceGeneratorTester = new SourceGeneratorTester<TGenerator>(generatorFactory);
		var sourceGeneratorTesterRun = sourceGeneratorTester.CreateRun(new[] { input });
		var sourceGeneratorTesterRunResult = sourceGeneratorTesterRun.Execute();

		return sourceGeneratorTesterRunResult;
	}

    public static SourceGeneratorTesterRunResult<TGenerator> Run<TGenerator>(
        Func<SourceGeneratorTester<TGenerator>, SourceGeneratorTesterRun<TGenerator>, TGenerator> generatorFactory,
        string[] inputs
    )
        where TGenerator : IIncrementalGenerator
    {
        var sourceGeneratorTester = new SourceGeneratorTester<TGenerator>(generatorFactory);
        var sourceGeneratorTesterRun = sourceGeneratorTester.CreateRun(inputs);
        var sourceGeneratorTesterRunResult = sourceGeneratorTesterRun.Execute();

        return sourceGeneratorTesterRunResult;
    }
}

public class SourceGeneratorTester<TGenerator> : SourceGeneratorTester
    where TGenerator : IIncrementalGenerator
{
    public Func<SourceGeneratorTester<TGenerator>, SourceGeneratorTesterRun<TGenerator>, TGenerator> GeneratorFactory { get; }

    public SourceGeneratorTester(Func<SourceGeneratorTester<TGenerator>, SourceGeneratorTesterRun<TGenerator>, TGenerator> generatorFactory)
    {
        GeneratorFactory = generatorFactory ?? throw new ArgumentNullException(nameof(generatorFactory));
    }

    public SourceGeneratorTesterRun<TGenerator> CreateRun(string[] inputs)
    {
        return new SourceGeneratorTesterRun<TGenerator>(this, inputs);
    }
}
