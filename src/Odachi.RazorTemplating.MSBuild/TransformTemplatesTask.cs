using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Collections.Generic;

namespace Odachi.RazorTemplating.MSBuild
{
    public class TransformTemplatesTask : ITask
	{
		public IBuildEngine BuildEngine { get; set; }
		public ITaskHost HostObject { get; set; }

		[Required]
		public string ProjectDirectory { get; set; }

		[Required]
		public ITaskItem[] InputItems { get; set; }

		[Output]
		public ITaskItem[] OutputItems { get; set; }

		public bool Execute()
		{
			// normalize project directory (must include trailing slash)
			ProjectDirectory = Path.GetFullPath(ProjectDirectory);
			if (ProjectDirectory[ProjectDirectory.Length - 1] != Path.DirectorySeparatorChar)
				ProjectDirectory += Path.DirectorySeparatorChar;

			// create templater
			BuildEngine.LogMessageEvent(new BuildMessageEventArgs($"Processing project \"{ProjectDirectory}\"", string.Empty, "Odachi.RazorTemplating.MSBuild.TransformTemplatesTask", MessageImportance.Normal));
			var templater = new RazorTemplater(ProjectDirectory);

			// create templater
			var outputFileNames = new List<string>();
			foreach (var inputItem in InputItems)
			{
				var inputFileName = inputItem.GetMetadata("FullPath");
				var outputFileName = Path.Combine(Path.GetDirectoryName(inputFileName), Path.GetFileNameWithoutExtension(inputFileName) + ".Designer.cs");

				BuildEngine.LogMessageEvent(new BuildMessageEventArgs($"Transforming \"{inputItem.ItemSpec}\" into \"{outputFileName}\"", string.Empty, "Odachi.RazorTemplating.MSBuild.TransformTemplatesTask", MessageImportance.Normal));

				templater.Generate(inputFileName, outputFileName);

				outputFileNames.Add(outputFileName);
			}

			OutputItems = outputFileNames.Select(fileName => new TaskItem(fileName.StartsWith(ProjectDirectory) ? fileName.Substring(ProjectDirectory.Length) : fileName)).ToArray();

			return true;
		}
	}
}
