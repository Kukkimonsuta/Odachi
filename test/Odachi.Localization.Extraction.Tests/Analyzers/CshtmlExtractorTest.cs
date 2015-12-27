using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Odachi.Localization.Extraction.Analysis
{
	public class CshtmlExtractorTest
	{
		[Theory]
		[InlineData(@"@{ var block = GetString(""In block""); }")]
		[InlineData(@"@{ var block = T.GetString(""In block""); }")]
		[InlineData(@"<div>@{ var block = GetString(""In block""); }</div>")]
		[InlineData(@"<div>@{ var block = T.GetString(""In block""); }</div>")]
		public void From_block(string data)
		{
			// prepare
			var extractor = new CshtmlAnalyzer();

			// execute
			var results = extractor.GetInvocations(data);

			// verify
			Assert.Equal(1, results.Count());
		}

		[Theory]
		[InlineData(@"@GetString(""Inline"")")]
		[InlineData(@"@T.GetString(""Inline"")")]
		[InlineData(@"<div>@GetString(""Inline"")</div>")]
		[InlineData(@"<div>@T.GetString(""Inline"")</div>")]
		public void Inline(string data)
		{
			// prepare
			var extractor = new CshtmlAnalyzer();

			// execute
			var results = extractor.GetInvocations(data);

			// verify
			Assert.Equal(1, results.Count());
		}

		[Theory]
		[InlineData(@"<div data-test=""@GetString(""Inline"")""></div>")]
		[InlineData(@"<div data-test=""@T.GetString(""Inline"")""></div>")]
		public void Inline_in_attribute(string data)
		{
			// prepare
			var extractor = new CshtmlAnalyzer();

			// execute
			var results = extractor.GetInvocations(data);

			// verify
			Assert.Equal(1, results.Count());
		}

		[Theory]
		[InlineData(@"@GetString(""Unsupported extra parameters"", 1 + 55, new[] { ""foo"", ""bar"" }, T(troll))")]
		[InlineData(@"@T.GetString(""Unsupported extra parameters"", 1 + 55, new[] { ""foo"", ""bar"" }, T(troll))")]
		public void Unsupported_parameters(string data)
		{
			// prepare
			var extractor = new CshtmlAnalyzer();

			// execute
			var results = extractor.GetInvocations(data);

			// verify
			Assert.Equal(2, results.Count());
		}

		[Theory]
		[InlineData(@"@GetString(""Recursion"", GetString(""recursed""))")]
		[InlineData(@"@T.GetString(""Recursion"", T.GetString(""recursed""))")]
		public void Recursion(string data)
		{
			// prepare
			var extractor = new CshtmlAnalyzer();

			// execute
			var results = extractor.GetInvocations(data);

			// verify
			Assert.Equal(2, results.Count());
		}

		[Theory]
		[InlineData(@"@GetString(@""Verbatim"")")]
		[InlineData(@"@T.GetString(@""Verbatim"")")]
		public void Verbatim_string_literal(string data)
		{
			// prepare
			var extractor = new CshtmlAnalyzer();

			// execute
			var results = extractor.GetInvocations(data);

			// verify
			Assert.Equal(1, results.Count());
		}

		[Theory]
		[InlineData(@"@GetString[""indexer""]")]
		[InlineData(@"@T.GetString[""indexer""]")]
		public void From_indexer(string data)
		{
			// prepare
			var extractor = new CshtmlAnalyzer();

			// execute
			var results = extractor.GetInvocations(data);

			// verify
			Assert.Equal(1, results.Count());
		}
	}
}
