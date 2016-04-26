using Microsoft.AspNetCore.Razor.Parser;
using Microsoft.AspNetCore.Razor.Tokenizer.Symbols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Parser.SyntaxTree;
using System.Globalization;
using Odachi.Localization.Extraction.External;
using Odachi.Localization.Extraction.Generation;
using Odachi.Localization.Extraction.Analysis;
using System.Text;

namespace Odachi.Localization.Extraction.Commands
{
	public class Program
	{
		static Program()
		{
			extractors = new Dictionary<string, IAnalyzer>();
			extractors.Add(new CsAnalyzer());
			extractors.Add(new CshtmlAnalyzer());

			generators = new Dictionary<string, IGeneratorFactory>();
			generators.Add(new PotGeneratorFactory());
			generators.Add(new ResxGeneratorFactory());
		}

		private static Dictionary<string, IAnalyzer> extractors;
		private static Dictionary<string, IGeneratorFactory> generators;

		private static void PrintUsage()
		{
			PrintUsage(null, null);
		}
		private static void PrintUsage(string message, params object[] args)
		{
			if (message != null)
			{
				Console.WriteLine(string.Format(message, args));
				Console.WriteLine("");
			}

			Console.WriteLine("Usage: chi-extract-locale [options] input [input2 [inputN]]");
			Console.WriteLine("");
			Console.WriteLine("Options:");
			Console.WriteLine("");
			Console.WriteLine("\t-l <format>, --input-format <format>\t\tUse extractor for given extension ({0})", string.Join(", ", extractors.Keys));
			Console.WriteLine("\t-k <keyword>, --keyword <keyword>\t\tKeyword specification");
			Console.WriteLine("\t-o <file>, --output <file>\t\t\tOutput to specified file instead of std out");
			Console.WriteLine("\t-f <format>, --output-format <format>\t\tOutput format ({0})", string.Join(", ", generators.Keys));
			Console.WriteLine("");
		}

		private struct Options
		{
			public List<string> InputPatterns;
			public string InputFormat;

			public List<Keyword> Keywords;

			public string OutputFile;
			public string OutputFormat;
		}

		public static int Main(string[] args)
		{
#if DNX451 || NET451
			System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
#else
			CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
#endif

			var options = new Options
			{
				OutputFile = default(string),
				OutputFormat = ".pot",
				InputFormat = default(string),
				Keywords = new List<Keyword>(),
				InputPatterns = new List<string>()
			};

			for (var i = 0; i < args.Length; i++)
			{
				var arg = args[i];

				if (arg.StartsWith("-"))
				{
					if (arg == "-o" || arg == "--output")
					{
						options.OutputFile = args[++i];
						continue;
					}

					if (arg == "-f" || arg == "--format")
					{
						options.OutputFormat = args[++i];
						continue;
					}

					if (arg == "-k" || arg == "--keyword")
					{
						options.Keywords.Add(Keyword.Parse(args[++i]));
						continue;
					}

					if (arg == "-l" || arg == "--language")
					{
						options.InputFormat = args[++i];
						continue;
					}

					PrintUsage("Unrecognized argument '{0}'", arg);
					return -1;
				}

				options.InputPatterns.Add(arg);
			}

			if (options.InputPatterns.Count <= 0)
			{
				PrintUsage();
				return -1;
			}

			if (options.InputFormat != null && !extractors.ContainsKey(options.InputFormat))
			{
				PrintUsage("Language '{0}' not found", options.InputFormat);
				return -1;
			}

			var generatorFactory = default(IGeneratorFactory);
			if (string.IsNullOrEmpty(options.OutputFormat) || !generators.TryGetValue(options.OutputFormat, out generatorFactory))
			{
				PrintUsage("Generator '{0}' not found", options.OutputFormat);
				return -1;
			}

			var files = options.InputPatterns
				.SelectMany(f => Glob.ExpandNames(f))
				.ToArray();

			var missingFiles = files.Where(f => !File.Exists(f)).ToArray();
			if (missingFiles.Length > 0)
			{
				PrintUsage("Files {0} not found", string.Join(", ", missingFiles.Select(f => $"'${f}'")));
				return -1;
			}

			if (options.InputFormat == null)
			{
				var unknownFiles = files.Where(f => !extractors.ContainsKey(Path.GetExtension(f))).ToArray();
				if (unknownFiles.Length > 0)
				{
					PrintUsage("Unknown format for files: {0}", string.Join(", ", unknownFiles.Select(f => $"'${f}'")));
					return -1;
				}
			}

			var matches = new List<KeywordMatch>();

			foreach (var file in files)
			{
				using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
				using (var reader = new StreamReader(stream))
				{
					var extension = options.InputFormat ?? Path.GetExtension(file);
					var extractor = extractors[extension];
					var effectiveKeywords = options.Keywords.Count > 0 ? options.Keywords : extractor.DefaultKeywords;

					var calls = extractor.GetInvocations(reader, fileName: file);

					foreach (var call in calls)
					{
						var match = effectiveKeywords.Select(k => k.Match(call))
							.Where(m => m != null)
							.FirstOrDefault();

						if (match == null)
							continue;

						matches.Add(match);
					}
				}
			}

			var groups = matches.GroupBy(m => m, m => m,
				(k, g) => new KeywordMatchGroup(k.Singular, k.Plural, k.Context, g.Where(i => i.Source != null).Select(i => i.Source.Value))
			);

			var hasOutputFile = !string.IsNullOrEmpty(options.OutputFile) && options.OutputFile != "-";
			using (var outputStream = hasOutputFile ? new FileStream(options.OutputFile, FileMode.Create, FileAccess.Write, FileShare.None) : Console.OpenStandardOutput())
			using (var generator = generatorFactory.Create(outputStream, new UTF8Encoding(false, true)))
			{
				generator.WriteHeader();
				foreach (var group in groups)
					generator.WriteEntry(group);
				generator.WriteFooter();
				generator.Flush();
			}

			return 0;
		}
	}
}
