using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using Odachi.Gettext;
using Odachi.Gettext.IO;

namespace Odachi.Localization
{
	public class DefaultGettextProcessor : IGettextProcessor
	{
		public DefaultGettextProcessor(string path)
		{
			if (path == null)
				throw new ArgumentNullException(nameof(path));

			_path = path;

			_catalogs = new Dictionary<string, Catalog>();
		}

		private string _path;
		private Dictionary<string, Catalog> _catalogs;

		private Catalog GetCatalog(CultureInfo culture)
		{
			Catalog catalog;

			if (!_catalogs.TryGetValue(culture.Name, out catalog))
			{
				var path = Path.Combine(_path, culture.Name + ".mo");
				if (File.Exists(path))
				{
					using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
					using (var reader = new MoReader(stream))
						catalog = Catalog.Load(reader, culture: culture);
				}
				else
					catalog = null;

				_catalogs[culture.Name] = catalog;
			}

			return catalog;
		}

		private string Get(CultureInfo culture, Func<Catalog, string> getter)
		{
			var currentCulture = culture;

			while (true)
			{
				// check catalog for current culture
				var catalog = GetCatalog(currentCulture);
				if (catalog != null)
				{
					var result = getter(catalog);

					if (result != null)
						return result;
				}

				// move to parent culture
				if (currentCulture.Parent == currentCulture)
					break;

				currentCulture = currentCulture.Parent;
			}

			// not found
			return null;
		}

		public string GetString(CultureInfo culture, string text)
		{
			return Get(culture, c => c.GetString(text))
				?? text;
		}

		public string GetPluralString(CultureInfo culture, string text, string pluralText, long n)
		{
			return Get(culture, c => c.GetPluralString(text, pluralText, n))
				?? (PluralFormsExpression.Default.Get(n) == 0 ? text : pluralText);
		}

		public string GetParticularString(CultureInfo culture, string context, string text)
		{
			return Get(culture, c => c.GetParticularString(context, text))
				?? text;
		}

		public string GetParticularPluralString(CultureInfo culture, string context, string text, string pluralText, long n)
		{
			return Get(culture, c => c.GetParticularPluralString(context, text, pluralText, n))
				?? (PluralFormsExpression.Default.Get(n) == 0 ? text : pluralText);
		}

		public IEnumerable<LocalizedString> GetAllStrings(CultureInfo culture, bool includeAncestorCultures)
		{
			throw new NotImplementedException();
		}
	}
}
