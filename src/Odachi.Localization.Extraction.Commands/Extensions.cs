using Odachi.Localization.Extraction.Analysis;
using Odachi.Localization.Extraction.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Localization.Extraction
{
    internal static class Extensions
    {
		public static void Add(this IDictionary<string, IAnalyzer> dictionary, IAnalyzer extractor)
		{
			dictionary.Add(extractor.DefaultExtension, extractor);
		}

		public static void Add(this IDictionary<string, IGeneratorFactory> dictionary, IGeneratorFactory generator)
		{
			dictionary.Add(generator.DefaultExtension, generator);
		}
	}
}
