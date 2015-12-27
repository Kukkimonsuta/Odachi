using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

namespace Odachi.Localization
{
	public interface IGettextProcessor
	{
		string GetString(CultureInfo culture, string text);
		string GetPluralString(CultureInfo culture, string text, string pluralText, long n);
		string GetParticularString(CultureInfo culture, string context, string text);
		string GetParticularPluralString(CultureInfo culture, string context, string text, string pluralText, long n);
		IEnumerable<LocalizedString> GetAllStrings(CultureInfo culture, bool includeAncestorCultures);
	}
}
