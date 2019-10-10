using Odachi.RazorTemplating.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Odachi.RazorTemplating.Tests
{
	public class RazorTemplaterTests
	{
		//[Fact]
		//public void Works()
		//{
		//	var OutputDirectory = "d:\\razor-temp";

		//	// normalize project directory (must include trailing slash)
		//	var ProjectDirectory = Directory.GetCurrentDirectory();
		//	if (ProjectDirectory[ProjectDirectory.Length - 1] != Path.DirectorySeparatorChar)
		//		ProjectDirectory += Path.DirectorySeparatorChar;

		//	// create templater
		//	var templater = new RazorTemplater(ProjectDirectory);

		//	// create templater
		//	var outputFileNames = new List<string>();
		//	foreach (var inputItem in Directory.GetFiles(ProjectDirectory, "*.cshtml", SearchOption.AllDirectories))
		//	{
		//		var inputFileName = inputItem;
		//		var outputFileName = Path.Combine(
		//			OutputDirectory,
		//			PathTools.GetRelativePath(ProjectDirectory, Path.GetDirectoryName(inputFileName)).Replace(Path.DirectorySeparatorChar, '_').Replace(Path.AltDirectorySeparatorChar, '_') + "_" + Path.GetFileNameWithoutExtension(inputFileName) + ".cs"
		//		);

		//		if (!Directory.Exists(OutputDirectory))
		//		{
		//			Directory.CreateDirectory(OutputDirectory);
		//		}

		//		templater.Generate(inputFileName, outputFileName);

		//		outputFileNames.Add(outputFileName);
		//	}
		//}
	}
}
