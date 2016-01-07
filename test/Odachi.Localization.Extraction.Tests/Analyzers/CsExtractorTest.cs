using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Odachi.Localization.Extraction.Analysis
{
	public class CsExtractorTest
	{
		[Theory]
		[InlineData(@"class Foo { public Foo() { GetString(""In constructor""); } }")]
		[InlineData(@"class Foo { public Foo() { T.GetString(""In constructor""); } }")]
		public void From_constructor(string data)
		{
			// prepare
			var extractor = new CsAnalyzer();

			// execute
			var results = extractor.GetInvocations(data);

			// verify
			Assert.Equal(1, results.Count());
		}

		[Theory]
		[InlineData(@"class Foo { public void Bar() { GetString(""In method""); } }")]
		[InlineData(@"class Foo { public void Bar() { T.GetString(""In method""); } }")]
		public void From_methods(string data)
		{
			// prepare
			var extractor = new CsAnalyzer();

			// execute
			var results = extractor.GetInvocations(data);

			// verify
			Assert.Equal(1, results.Count());
		}

		[Theory]
		[InlineData(@"class Foo { public void Bar() { GetString(""Unsupported extra parameters"", 1 + 55, new[] { ""foo"", ""bar"" }, T(troll)); } }")]
		[InlineData(@"class Foo { public void Bar() { T.GetString(""Unsupported extra parameters"", 1 + 55, new[] { ""foo"", ""bar"" }, T(troll)); } }")]
		public void Unsupported_parameters(string data)
		{
			// prepare
			var extractor = new CsAnalyzer();

			// execute
			var results = extractor.GetInvocations(data);

			// verify
			Assert.Equal(2, results.Count());
		}

		[Theory]
		[InlineData(@"class Foo { public void Bar() { GetString(""Recursion"", T.GetString(""recursed"")); } }")]
		[InlineData(@"class Foo { public void Bar() { T.GetString(""Recursion"", T.GetString(""recursed"")); } }")]
		public void Recursion(string data)
		{
			// prepare
			var extractor = new CsAnalyzer();

			// execute
			var results = extractor.GetInvocations(data);

			// verify
			Assert.Equal(2, results.Count());
		}

		[Theory]
		[InlineData(@"class Foo { public void Bar() { GetString(@""Verbatim""); } }")]
		[InlineData(@"class Foo { public void Bar() { T.GetString(@""Verbatim""); } }")]
		public void Verbatim_string_literal(string data)
		{
			// prepare
			var extractor = new CsAnalyzer();

			// execute
			var results = extractor.GetInvocations(data);

			// verify
			Assert.Equal(1, results.Count());
		}

		[Theory]
		[InlineData(@"class Foo { [GetString] public Bar { get; set; } } }")]
		[InlineData(@"class Foo { [GetString(""Foo"")] public Bar { get; set; } } }")]
		[InlineData(@"class Foo { [GetString(Name = ""Foo"")] public Bar { get; set; } } }")]
		public void From_attribute(string data)
		{
			// prepare
			var extractor = new CsAnalyzer();

			// execute
			var results = extractor.GetInvocations(data);

			// verify
			Assert.Equal(1, results.Count());
		}

		[Theory]
		[InlineData(@"class Foo { public string Bar { get { return $""Interpolated {GetString(""string"")}""; } } } }")]
		public void From_interpolated_string(string data)
		{
			// prepare
			var extractor = new CsAnalyzer();

			// execute
			var results = extractor.GetInvocations(data);

			// verify
			Assert.Equal(1, results.Count());
		}

		[Theory]
		[InlineData(@"class Foo { public Foo() { GetString[""indexer""]; } }")]
		[InlineData(@"class Foo { public Foo() { T.GetString[""indexer""]; } }")]
		public void From_indexer(string data)
		{
			// prepare
			var extractor = new CsAnalyzer();

			// execute
			var results = extractor.GetInvocations(data);

			// verify
			Assert.Equal(1, results.Count());
		}
	}
}
