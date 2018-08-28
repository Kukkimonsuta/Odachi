using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Odachi.RazorTemplating.Internal;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Odachi.RazorTemplating
{
	public class ChangeNamespacePass : IntermediateNodePassBase, IRazorDocumentClassifierPass
	{
		public ChangeNamespacePass(string projectDirectory, string rootNamespace)
		{
			ProjectDirectory = projectDirectory;
			RootNamespace = rootNamespace;
		}

		public string ProjectDirectory { get; }
		public string RootNamespace { get; }

		public override int Order => 1100;

		protected override void ExecuteCore(RazorCodeDocument codeDocument, DocumentIntermediateNode documentNode)
		{
			var @namespace = documentNode.FindPrimaryNamespace();
			var @class = documentNode.FindPrimaryClass();

			@namespace.Content = $"{RootNamespace}.{PathTools.GetRelativePath($"{ProjectDirectory}{Path.DirectorySeparatorChar}", Path.GetDirectoryName(codeDocument.Source.FilePath)).Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.')}";
			@class.ClassName = Path.GetFileNameWithoutExtension(codeDocument.Source.FilePath);
		}
	}

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

			engine = RazorProjectEngine.Create(
				RazorConfiguration.Default,
				RazorProjectFileSystem.Create(ProjectDirectory),
				config =>
				{
					InheritsDirective.Register(config);

					config.Features.Add(new ChangeNamespacePass(ProjectDirectory, RootNamespace));

					// this is horrible hack that will bite me in the bum
					var targetExtension = config.Features.OfType<IRazorTargetExtensionFeature>().FirstOrDefault();
					if (targetExtension != null)
					{
						foreach (var metadataExtension in targetExtension.TargetExtensions.Where(x => x.GetType().FullName == "Microsoft.AspNetCore.Razor.Language.Extensions.MetadataAttributeTargetExtension").ToArray())
						{
							targetExtension.TargetExtensions.Remove(metadataExtension);
						}
					}
				}
			);
			templateEngine = new RazorTemplateEngine(engine.Engine, engine.FileSystem);
		}

		private RazorProjectEngine engine;
		private RazorTemplateEngine templateEngine;

		public string ProjectDirectory { get; }
		public string RootNamespace { get; }
		public Encoding Encoding { get; }

		public void Generate(string inputFileName, string outputFileName)
		{
			// normalize file names
			inputFileName = Path.GetFullPath(inputFileName);
			outputFileName = Path.GetFullPath(outputFileName);

			using (var stream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (var reader = new StreamReader(stream))
			{
				var codeDocument = templateEngine.CreateCodeDocument(inputFileName);
				var result = templateEngine.GenerateCode(codeDocument);
				// normalize new lines (is there better way to do this?)
				var code = result.GeneratedCode.Replace("\r\n", "\n");

				if (File.Exists(outputFileName))
				{
					using (var md5 = MD5.Create()) /*DevSkim: ignore DS126858*/
					using (var inputStream = File.OpenRead(outputFileName))
					{
						var codeHash = md5.ComputeHash(inputStream);
						var fileHash = md5.ComputeHash(Encoding.GetBytes(code));

						if (codeHash.SequenceEqual(fileHash))
						{
							return;
						}
					}
				}

				using (var outputStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write, FileShare.Read))
				using (var writer = new StreamWriter(outputStream, Encoding))
				{
					writer.Write(code);
				}
			}
		}
	}
}
