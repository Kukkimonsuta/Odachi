using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Odachi.RazorTemplating.Internal
{
	public class PathToolsTest
	{
		[Theory]
		[InlineData(@"c:\test\file1.Designer.cs", @"c:\test\file1.cshtml", @"file1.cshtml")]
		[InlineData(@"c:\test\", @"c:\test\file1.cshtml", @"file1.cshtml")]
		[InlineData(@"c:\test\cookies", @"c:\test\file1.cshtml", @"file1.cshtml")]
		[InlineData(@"c:\test\cookies\", @"c:\test\file1.cshtml", @"..\file1.cshtml")]
		[InlineData(@"/mnt/d/foo/", @"/mnt/d/bar/", @"..\bar\")]
		public void Can_compute_relative_path(string from, string to, string expectedResult)
		{
			var actualResult = PathTools.GetRelativePath(from, to)
				.Replace('/', '\\');

			Assert.Equal(expectedResult, actualResult);
		}
	}
}
