using System;
using Xunit;
using Odachi.Build.OptionsFileGenerator;
using Odachi.Build.OptionsFileGenerator.Tests.Framework;

namespace Odachi.Build.OptionsFileGenerator.Tests;

public class DefaultValueTests
{
	[Fact]
	public void Can_handle_boolean()
	{
		var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
			(tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
			"""
            using Odachi.Annotations;

            [OptionsContainer(FileName = "appsettings.json")]
            public class FooOptions
            {
                public bool Bool { get; set; } = true;
            }
            """
		);

		result.AssertDiagnostics();
		result.AssertAdditionalFile(
			"test://appsettings.json",
			"""
            {
              "Bool": true,
            }

            """
		);
	}

	[Fact]
	public void Can_handle_boolean_implicit()
	{
		var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
			(tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
			"""
			using Odachi.Annotations;

			[OptionsContainer(FileName = "appsettings.json")]
			public class FooOptions
			{
			    public bool Bool { get; set; }
			}
			"""
		);

		result.AssertDiagnostics();
		result.AssertAdditionalFile(
			"test://appsettings.json",
			"""
			{
			  "Bool": false,
			}

			"""
		);
	}

    [Fact]
    public void Can_handle_integers()
    {
        var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
            (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
            """
            using Odachi.Annotations;

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
    public void Can_handle_integers_implicit()
    {
	    var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
		    (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
		    """
		    using Odachi.Annotations;

		    [OptionsContainer(FileName = "appsettings.json")]
		    public class FooOptions
		    {
		        public sbyte SimpleInt8 { get; set; }
		        public short SimpleInt16 { get; set; }
		        public int SimpleInt32 { get; set; }
		        public long SimpleInt64 { get; set; }

		        public byte SimpleUInt8 { get; set; }
		        public ushort SimpleUInt16 { get; set; }
		        public uint SimpleUInt32 { get; set; }
		        public ulong SimpleUInt64 { get; set; }
		    }
		    """
	    );

	    result.AssertDiagnostics();
	    result.AssertAdditionalFile(
		    "test://appsettings.json",
		    """
		    {
		      "SimpleInt8": 0,
		      "SimpleInt16": 0,
		      "SimpleInt32": 0,
		      "SimpleInt64": 0,
		      "SimpleUInt8": 0,
		      "SimpleUInt16": 0,
		      "SimpleUInt32": 0,
		      "SimpleUInt64": 0,
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
            using Odachi.Annotations;

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
    public void Can_handle_decimals_implicit()
    {
	    var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
		    (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
		    """
		    using Odachi.Annotations;

		    [OptionsContainer(FileName = "appsettings.json")]
		    public class FooOptions
		    {
		        public float SimpleSingle { get; set; }
		        public double SimpleDouble { get; set; }
		        public decimal SimpleDecimal { get; set; }
		    }
		    """
	    );

	    result.AssertDiagnostics();
	    result.AssertAdditionalFile(
		    "test://appsettings.json",
		    """
		    {
		      "SimpleSingle": 0,
		      "SimpleDouble": 0,
		      "SimpleDecimal": 0,
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
            using Odachi.Annotations;

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
    public void Can_handle_strings_implicit()
    {
	    var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
		    (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
		    """
		    using Odachi.Annotations;

		    [OptionsContainer(FileName = "appsettings.json")]
		    public class FooOptions
		    {
		        public string SimpleString { get; set; }
		    }
		    """
	    );

	    result.AssertDiagnostics();
	    result.AssertAdditionalFile(
		    "test://appsettings.json",
		    """
		    {
		      "SimpleString": null,
		    }

		    """
	    );
    }

    [Fact]
    public void Can_handle_strings_implicit_nullable()
    {
	    var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
		    (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
		    """
		    #nullable enable

		    using Odachi.Annotations;

		    [OptionsContainer(FileName = "appsettings.json")]
		    public class FooOptions
		    {
		        public string? SimpleString { get; set; }
		    }
		    """
	    );

	    result.AssertDiagnostics();
	    result.AssertAdditionalFile(
		    "test://appsettings.json",
		    """
		    {
		      "SimpleString": null,
		    }

		    """
	    );
    }

    [Fact]
    public void Can_handle_strings_implicit_nonnullable()
    {
	    var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
		    (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
		    """
		    #nullable enable

		    using Odachi.Annotations;

		    [OptionsContainer(FileName = "appsettings.json")]
		    public class FooOptions
		    {
		        [System.Diagnostics.CodeAnalysis.AllowNull]
		        public string SimpleString { get; set; }
		    }
		    """
	    );

	    result.AssertDiagnostics();
	    result.AssertAdditionalFile(
		    "test://appsettings.json",
		    """
		    {
		      "SimpleString": "",
		    }

		    """
	    );
    }

    [Fact]
    public void Can_handle_timespan()
    {
	    var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
		    (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
		    """
            using System;
            using Odachi.Annotations;

            [OptionsContainer(FileName = "appsettings.json")]
            public class FooOptions
            {
                public TimeSpan TimeSpanZero { get; set; } = TimeSpan.Zero;
                public TimeSpan TimeSpanCtor { get; set; } = new TimeSpan();
                public TimeSpan TimeSpanCtorTicks { get; set; } = new TimeSpan(12345670);
                public TimeSpan TimeSpanCtorHoursMinutesSeconds { get; set; } = new TimeSpan(2, 3, 4);
                public TimeSpan TimeSpanCtorDaysHoursMinutesSeconds { get; set; } = new TimeSpan(1, 2, 3, 4);
                public TimeSpan TimeSpanCtorDaysHoursMinutesSecondsMilliseconds { get; set; } = new TimeSpan(1, 2, 3, 4, 5);
                public TimeSpan TimeSpanCtorDaysHoursMinutesSecondsMillisecondsMicroseconds { get; set; } = new TimeSpan(1, 2, 3, 4, 5, 6);
            }
            """
	    );

	    result.AssertDiagnostics();
	    result.AssertAdditionalFile(
		    "test://appsettings.json",
		    """
            {
              "TimeSpanZero": "00:00:00",
              "TimeSpanCtor": "00:00:00",
              "TimeSpanCtorTicks": "00:00:01.234567",
              "TimeSpanCtorHoursMinutesSeconds": "02:03:04",
              "TimeSpanCtorDaysHoursMinutesSeconds": "1.02:03:04",
              "TimeSpanCtorDaysHoursMinutesSecondsMilliseconds": "1.02:03:04.005",
              "TimeSpanCtorDaysHoursMinutesSecondsMillisecondsMicroseconds": "1.02:03:04.005006",
            }

            """
	    );
    }

    [Fact]
    public void Can_handle_array()
    {
	    var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
		    (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
		    """
		    using Odachi.Annotations;

		    [OptionsContainer(FileName = "appsettings.json")]
		    public class FooOptions
		    {
		        public int[] IntArray { get; set; }
		    }
		    """
	    );

	    result.AssertDiagnostics();
	    result.AssertAdditionalFile(
		    "test://appsettings.json",
		    """
		    {
		      "IntArray": null,
		    }

		    """
	    );
    }

    [Fact]
    public void Can_handle_array_nonnullable()
    {
	    var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
		    (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
		    """
		    #nullable enable

		    using Odachi.Annotations;

		    [OptionsContainer(FileName = "appsettings.json")]
		    public class FooOptions
		    {
		        [System.Diagnostics.CodeAnalysis.AllowNull]
		        public int[] IntArray { get; set; }
		    }
		    """
	    );

	    result.AssertDiagnostics();
	    result.AssertAdditionalFile(
		    "test://appsettings.json",
		    """
		    {
		      "IntArray": [],
		    }

		    """
	    );
    }

    [Fact]
    public void Can_handle_array_nullable()
    {
	    var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
		    (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
		    """
		    #nullable enable

		    using Odachi.Annotations;

		    [OptionsContainer(FileName = "appsettings.json")]
		    public class FooOptions
		    {
		        public int[]? IntArray { get; set; }
		    }
		    """
	    );

	    result.AssertDiagnostics();
	    result.AssertAdditionalFile(
		    "test://appsettings.json",
		    """
		    {
		      "IntArray": null,
		    }

		    """
	    );
    }

    [Fact]
    public void Can_handle_array_of_ints()
    {
	    var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
		    (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
		    """
		    using Odachi.Annotations;

		    [OptionsContainer(FileName = "appsettings.json")]
		    public class FooOptions
		    {
		        public int[] IntArray { get; set; } = new int[]
		        {
		            1,
		            2,
		            3,
		        };

		        public int[] IntArrayImplicit { get; set; } = new[]
		        {
		            1,
		            2,
		            3,
		        };

		        public int[] IntArrayNoNew { get; set; } =
		        {
		            1,
		            2,
		            3,
		        };

		        public int[] IntArrayCollectionExpression { get; set; } =
		        [
		            1,
		            2,
		            3,
		        ];
		    }
		    """
	    );

	    result.AssertDiagnostics();
	    result.AssertAdditionalFile(
		    "test://appsettings.json",
		    """
		    {
		      "IntArray": [1, 2, 3],
		      "IntArrayImplicit": [1, 2, 3],
		      "IntArrayNoNew": [1, 2, 3],
		      "IntArrayCollectionExpression": [1, 2, 3],
		    }

		    """
	    );
    }

    [Fact]
    public void Can_handle_array_of_strings()
    {
	    var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
		    (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
		    """
            using Odachi.Annotations;

            [OptionsContainer(FileName = "appsettings.json")]
            public class FooOptions
            {
                public string[] StringArray { get; set; } = new string[]
                {
                    "Test1",
                    "Test2",
                };

                public string[] StringArrayImplicit { get; set; } = new[]
                {
                    "Test1",
                    "Test2",
                };

                public string[] StringArrayNoNew { get; set; } =
                {
                    "Test1",
                    "Test2",
                };

                public string[] StringArrayCollectionExpression { get; set; } =
                [
                    "Test1",
                    "Test2",
                ];
            }
            """
	    );

	    result.AssertDiagnostics();
	    result.AssertAdditionalFile(
		    "test://appsettings.json",
		    """
            {
              "StringArray": ["Test1", "Test2"],
              "StringArrayImplicit": ["Test1", "Test2"],
              "StringArrayNoNew": ["Test1", "Test2"],
              "StringArrayCollectionExpression": ["Test1", "Test2"],
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
            using Odachi.Annotations;

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
    public void Can_handle_enums_implicit()
    {
	    var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
		    (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
		    """
		    using Odachi.Annotations;

		    public enum Bar
		    {
		        Option1,
		        Option2,
		        Option3,
		    }

		    [OptionsContainer(FileName = "appsettings.json")]
		    public class FooOptions
		    {
		        public Bar MyEnum { get; set; }
		    }
		    """
	    );

	    result.AssertDiagnostics();
	    result.AssertAdditionalFile(
		    "test://appsettings.json",
		    """
		    {
		      "MyEnum": null,
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
            using Odachi.Annotations;

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
    public void Can_handle_flags_enums_implicit()
    {
	    var result = SourceGeneratorTester.Run<OptionsFileSourceGenerator>(
		    (tester, run) => new OptionsFileSourceGenerator(run.FileSystem.Create, "test://"),
		    """
		    using Odachi.Annotations;

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
		        public Bar MyEnum { get; set; }
		    }
		    """
	    );

	    result.AssertDiagnostics();
	    result.AssertAdditionalFile(
		    "test://appsettings.json",
		    """
		    {
		      "MyEnum": null,
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
            using Odachi.Annotations;

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
            using Odachi.Annotations;

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
            using Odachi.Annotations;

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
