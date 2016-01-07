using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Localization.Extraction.Analysis
{
	public interface IAnalyzer
	{
		string DefaultExtension { get; }

		IEnumerable<Keyword> DefaultKeywords { get; }

		IEnumerable<Invocation> GetInvocations(TextReader reader, string fileName = null);
	}

    public static class AnalyzerExtensions
    {
		public static IEnumerable<Invocation> GetInvocations(this IAnalyzer extractor, string text, string fileName = null)
		{
			using (var reader = new StringReader(text))
			{
				return extractor.GetInvocations(reader, fileName: fileName);
			}
		}
    }
}
