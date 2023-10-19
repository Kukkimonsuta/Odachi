using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using VerifyTests;
using VerifyXunit;

namespace Odachi.Build.OptionsFileGenerator.Tests.Framework;

public class VerifyOptionsFileGenerator
{
	public static bool Initialized { get; private set; }

	public static void Initialize()
	{
		if (Initialized)
		{
			throw new("Already Initialized");
		}

		Initialized = true;

		InnerVerifier.ThrowIfVerifyHasBeenRun();
		VerifierSettings.RegisterFileConverter<ISourceGeneratorTesterRunResult>(Convert);
	}

	static ConversionResult Convert(ISourceGeneratorTesterRunResult target, IReadOnlyDictionary<string, object> context)
	{
		var exceptions = new List<Exception>();
		var targets = new List<Target>();

		foreach (var result in target.Driver.GetRunResult().Results)
		{
			if (result.Exception != null)
			{
				exceptions.Add(result.Exception);
			}

			var collection = result.GeneratedSources
				.OrderBy(x => x.HintName)
				.Select(SourceToTarget);
			targets.AddRange(collection);
		}

		foreach (var path in target.Run.FileSystem.List())
		{
			var data = target.Run.FileSystem.Get(path);

			targets.Add(SourceToTarget(path, data));
		}

		if (exceptions.Count == 1)
		{
			throw exceptions.First();
		}

		if (exceptions.Count > 1)
		{
			throw new AggregateException(exceptions);
		}

		if (target.Diagnostics.Any())
		{
			var info = new
			{
				target.Diagnostics
			};
			return new(info, targets);
		}

		return new(null, targets);
	}

	static Target SourceToTarget(string path, string content)
	{
		var data = $"""
        // Path: {path}
        {content}
        """;

		return new(Path.GetExtension(path).Substring(1), data, Path.GetFileNameWithoutExtension(path));
	}

	static Target SourceToTarget(GeneratedSourceResult source)
	{
		var data = $"""
        // HintName: {source.HintName}
        {source.SourceText}
        """;

		return new("cs", data, Path.GetFileNameWithoutExtension(source.HintName));
	}
}
