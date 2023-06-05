using Xunit;
using Odachi.Build.OptionsFileGenerator;
using Odachi.Build.OptionsFileGenerator.Tests.Framework;

namespace Odachi.Build.OptionsFileGenerator.Tests;

public class DefaultValueTests
{
    [Fact]
    public void Can_handle_integers()
    {
        var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
            (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
            """
            [OptionsContainer(FileName = "appsettings.json")]
            public class FooOptions
            {
                public sbyte SimpleInt8 { get; set; } = 123;
                public short SimpleInt16 { get; set; } = 12345;
                public int SimpleInt32 { get; set; } = 1234567;
                public long SimpleInt64 { get; set; } = 1234567890;

                public byte SimpleUInt8 { get; set; } = 123;
                public ushort SimpleUInt16 { get; set; } = 12345;
                public uint SimpleUInt32 { get; set; } = 1234567;
                public ulong SimpleUInt64 { get; set; } = 1234567890;
            }
            """
        );

        result.AssertDiagnostics();
        result.AssertAdditionalFile(
            "test://appsettings.json",
            """
            {
              "SimpleInt8": 123,
              "SimpleInt16": 12345,
              "SimpleInt32": 1234567,
              "SimpleInt64": 1234567890,
              "SimpleUInt8": 123,
              "SimpleUInt16": 12345,
              "SimpleUInt32": 1234567,
              "SimpleUInt64": 1234567890,
            }

            """
        );
    }

    [Fact]
    public void Can_handle_decimals()
    {
        var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
            (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
            """
            [OptionsContainer(FileName = "appsettings.json")]
            public class FooOptions
            {
                public float SimpleSingle { get; set; } = 1234.1234F;
                public double SimpleDouble { get; set; } = 1234567890.1234;
                public decimal SimpleDecimal { get; set; } = 987654321.1234M;
            }
            """
        );

        result.AssertDiagnostics();
        result.AssertAdditionalFile(
            "test://appsettings.json",
            """
            {
              "SimpleSingle": 1234.1234,
              "SimpleDouble": 1234567890.1234,
              "SimpleDecimal": 987654321.1234,
            }

            """
        );
    }

    [Fact]
    public void Can_handle_strings()
    {
        var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
            (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
            """
            [OptionsContainer(FileName = "appsettings.json")]
            public class FooOptions
            {
                public string SimpleString { get; set; } = "string";
            }
            """
        );

        result.AssertDiagnostics();
        result.AssertAdditionalFile(
            "test://appsettings.json",
            """
            {
              "SimpleString": "string",
            }

            """
        );
    }

    [Fact]
    public void Can_handle_enums()
    {
        var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
            (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
            """
            public enum Bar
            {
                Option1,
                Option2,
                Option3,
            }

            [OptionsContainer(FileName = "appsettings.json")]
            public class FooOptions
            {
                public Bar MyEnum { get; set; } = Bar.Option2;
            }
            """
        );

        result.AssertDiagnostics();
        result.AssertAdditionalFile(
            "test://appsettings.json",
            """
            {
              "MyEnum": "Option2",
            }

            """
        );
    }

    [Fact]
    public void Can_handle_flags_enums()
    {
        var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
            (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
            """
            [System.Flags]
            public enum Bar
            {
                Option1 = 1,
                Option2 = 2,
                Option3 = 4,
            }

            [OptionsContainer(FileName = "appsettings.json")]
            public class FooOptions
            {
                public Bar MyEnum { get; set; } = Bar.Option1 | Bar.Option2;
            }
            """
        );

        result.AssertDiagnostics();
        result.AssertAdditionalFile(
            "test://appsettings.json",
            """
            {
              "MyEnum": "Option1, Option2",
            }

            """
        );
    }

    [Fact]
    public void Can_handle_object_initializer()
    {
        var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
            (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
            """
            [OptionsContainer(FileName = "appsettings.json")]
            public class FooOptions
            {
                public BarOptions Bar { get; set; } = new()
                {
                    SimpleString = "bar",
                };
            }

            [OptionsContainer]
            public class BarOptions
            {
                public string SimpleString { get; set; } = "string";
            }
            """
        );

        result.AssertDiagnostics();
        result.AssertAdditionalFile(
            "test://appsettings.json",
            """
            {
              "Bar": {
                "SimpleString": "bar",
              },
            }

            """
        );
    }

    [Fact]
    public void Can_handle_object_initializer_null()
    {
        var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
            (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
            """
            [OptionsContainer(FileName = "appsettings.json")]
            public class FooOptions
            {
                public BarOptions Bar { get; set; } = new()
                {
                    SimpleString = null,
                };
            }

            [OptionsContainer]
            public class BarOptions
            {
                public string SimpleString { get; set; } = "string";
            }
            """
        );

        result.AssertDiagnostics();
        result.AssertAdditionalFile(
            "test://appsettings.json",
            """
            {
              "Bar": {
                "SimpleString": null,
              },
            }

            """
        );
    }

    [Fact]
    public void Can_handle_object_initializer_deep()
    {
        var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
            (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
            """
            [System.Flags]
            public enum Bar
            {
                Option1 = 1,
                Option2 = 2,
                Option3 = 4,
            }

            [OptionsContainer(FileName = "appsettings.json")]
            public class FooOptions
            {
                public IntermediateOptions Intermediate { get; set; } = new()
                {
                    Bar = new()
                    {
                        SimpleString = "bar",
                        Flags = Bar.Option1 | Bar.Option3,
                    }
                };

                public IntermediateOptions NoInitializer { get; set; }
            }

            [OptionsContainer]
            public class IntermediateOptions
            {
                public BarOptions Bar { get; set; }
            }

            [OptionsContainer]
            public class BarOptions
            {
                public string SimpleString { get; set; } = "string";
                public Bar Flags { get; set; } = Bar.Option2;
            }
            """
        );

        result.AssertDiagnostics();
        result.AssertAdditionalFile(
            "test://appsettings.json",
            """
            {
              "Intermediate": {
                "Bar": {
                  "SimpleString": "bar",
                  "Flags": "Option1, Option3",
                },
              },
              "NoInitializer": {
                "Bar": {
                  "SimpleString": "string",
                  "Flags": "Option2",
                },
              },
            }

            """
        );
    }
}