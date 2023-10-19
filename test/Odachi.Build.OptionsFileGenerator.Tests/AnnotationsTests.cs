using System.Threading.Tasks;
using Xunit;
using Odachi.Build.OptionsFileGenerator.Tests.Framework;
using VerifyXunit;

namespace Odachi.Build.OptionsFileGenerator.Tests;

[UsesVerify]
public class AnnotationsTests
{
    [Fact]
    public Task Can_generate_annotations()
    {
        var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
            (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
            @"namespace MyCode;

public class Dummy
{
}
");

        result.AssertDiagnostics();

        return Verifier.Verify(result);
    }
}
