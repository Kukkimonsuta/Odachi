using System;
using Xunit;
using Odachi.Build.OptionsFileGenerator;
using Odachi.Build.OptionsFileGenerator.Tests.Framework;

namespace Odachi.Build.OptionsFileGenerator.Tests;

public class GeneralTests
{
    [Fact]
    public void Can_handle_multiple_files()
    {
        var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
            (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
            """
            [OptionsContainer(FileName = "appsettings1.json")]
            public class FooOptions1
            {
                public BarOptions Bar { get; set; } = new();

                [OptionsContainer]
                public class BarOptions
                {
                    public string SimpleString { get; set; } = "string";
                }
            }

            [OptionsContainer(FileName = "appsettings2.json", Key = "Test")]
            public class FooOptions2
            {
                public BarOptions Bar { get; set; } = new();

                [OptionsContainer]
                public class BarOptions
                {
                    public string SimpleString { get; set; } = "string";
                }
            }
            """
        );

        result.AssertDiagnostics();
        result.AssertAdditionalFile(
            "test://appsettings1.json",
            """
            {
              "Bar": {
                "SimpleString": "string",
              },
            }

            """
        );
        result.AssertAdditionalFile(
	        "test://appsettings2.json",
	        """
            {
              "Test": {
                "Bar": {
                  "SimpleString": "string",
                },
              },
            }

            """
        );
    }
}
