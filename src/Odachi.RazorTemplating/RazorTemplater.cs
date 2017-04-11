using Microsoft.AspNetCore.Razor;
using Odachi.RazorTemplating.Internal;
using System;
using System.IO;
using System.Text;

namespace Odachi.RazorTemplating
{
	public class RazorTemplater
	{
		private static readonly char[] DirectorySeparatorChars = new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

		public RazorTemplater(string projectDirectory, string rootNamespace = null, Encoding encoding = null)
		{
			if (projectDirectory == null)
				throw new ArgumentNullException(nameof(projectDirectory));
			if (!Directory.Exists(projectDirectory))
				throw new DirectoryNotFoundException($"Project directory '{projectDirectory}' not found");

			// normalize project directory (must not include trailing slash)
			ProjectDirectory = Path.GetFullPath(projectDirectory).TrimEnd(Path.DirectorySeparatorChar);
			RootNamespace = rootNamespace ?? Path.GetFileName(ProjectDirectory);
			Encoding = encoding ?? new UTF8Encoding(false);

			language = new CSharpRazorCodeLanguage();
			host = new RazorEngineHost(language);
			host.NamespaceImports.Add("System");
			host.GeneratedClassContext = new Microsoft.AspNetCore.Razor.CodeGenerators.GeneratedClassContext(
				"ExecuteAsync", "Write", "WriteLiteral",
				"WriteTo", "WriteLiteralTo", null, "DefineSection",
				new Microsoft.AspNetCore.Razor.CodeGenerators.GeneratedTagHelperContext()
			);
			engine = new RazorTemplateEngine(host);
		}

		private RazorCodeLanguage language;
		private RazorEngineHost host;
		private RazorTemplateEngine engine;

		public string ProjectDirectory { get; }
		public string RootNamespace { get; }
		public Encoding Encoding { get; }

		public string Generate(string inputFileName, string outputFileName)
		{
			// normalize file names
			inputFileName = Path.GetFullPath(inputFileName);
			outputFileName = Path.GetFullPath(outputFileName);

			var className = Path.GetFileNameWithoutExtension(inputFileName);
			var @namespace = $"{RootNamespace}.{PathTools.GetRelativePath($"{ProjectDirectory}{Path.DirectorySeparatorChar}", Path.GetDirectoryName(inputFileName)).Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.')}";

			using (var stream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (var reader = new StreamReader(stream))
			{
				var result = engine.GenerateCode(reader, className, @namespace, PathTools.GetRelativePath(outputFileName, inputFileName));

				using (var outputStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write, FileShare.Read))
				using (var writer = new StreamWriter(outputStream, Encoding))
				{
					writer.Write(result.GeneratedCode);
				}
			}

			return outputFileName;
		}
	}
}
