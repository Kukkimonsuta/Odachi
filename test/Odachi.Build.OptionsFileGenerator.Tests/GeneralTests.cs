using System;
using System.Threading.Tasks;
using Xunit;
using Odachi.Build.OptionsFileGenerator;
using Odachi.Build.OptionsFileGenerator.Tests.Framework;
using VerifyXunit;

namespace Odachi.Build.OptionsFileGenerator.Tests;

[UsesVerify]
public class GeneralTests
{
	[Fact]
	public Task HandlesMultipleSources()
	{
		var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
			(tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
			new[]
			{
				"""
				using Odachi.Annotations;

				[OptionsContainer(FileName = "appsettings1.json")]
				public class FooOptions1
				{
				    public bool Bool { get; set; } = true;
				}
				""",
				"""
				using Odachi.Annotations;

				[OptionsContainer(FileName = "appsettings2.json")]
				public class FooOptions2
				{
				    public bool Bool { get; set; } = false;
				}
				""",
			}
		);

		result.AssertDiagnostics();

		return Verifier.Verify(result);
	}

	[Fact]
	public Task HandlesAliasedAnnotations()
	{
		var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
			(tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
			new[]
			{
				"""
				using Odachi.Annotations;
				using A = Odachi.Annotations.OptionsContainerAttribute;

				[A(FileName = "appsettings1.json")]
				[OptionsContainerAttribute(FileName = "appsettings2.json")]
				[Odachi.Annotations.OptionsContainerAttribute(FileName = "appsettings3.json")]
				public class FooOptions2
				{
				    public bool Bool { get; set; } = false;
				}
				""",
			}
		);

		result.AssertDiagnostics();

		return Verifier.Verify(result);
	}

	[Fact]
	public Task HandlesMultipleOutputs()
	{
		var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
			(tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
			new[]
			{
				"""
				using Odachi.Annotations;

				public class TestC
				{
					public const string ConfigName = "appsettings";
				}

				[OptionsContainer(FileName = TestC.ConfigName + ".json")]
				[OptionsContainer(FileName = "appsettings2.json")]
				public class FooOptions1
				{
				    public bool Bool { get; set; } = true;
				}
				""",
			}
		);

		result.AssertDiagnostics();

		return Verifier.Verify(result);
	}
}
