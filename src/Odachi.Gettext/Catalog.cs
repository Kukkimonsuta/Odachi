using Odachi.Gettext.Internal;
using Odachi.Gettext.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Gettext
{
	public class Catalog : IReadOnlyDictionary<GettextKey, Translation>
	{
		private Catalog()
		{
			_headers = new Dictionary<string, string>();
			_translations = new Dictionary<GettextKey, Translation>();
		}

		private Dictionary<string, string> _headers;
		private Dictionary<GettextKey, Translation> _translations;

		public IReadOnlyDictionary<string, string> Headers { get { return _headers; } }

		public CultureInfo Culture { get; protected set; }

		public PluralFormsExpression PluralForms { get; protected set; }

		public string GetString(string text)
		{
			var translation = default(Translation);
			if (!TryGetValue(new GettextKey(null, text, null), out translation))
				return null;

			return translation[0];
		}

		public string GetParticularString(string context, string text)
		{
			var translation = default(Translation);
			if (!TryGetValue(new GettextKey(context, text, null), out translation))
				return null;

			return translation[0];
		}

		public string GetPluralString(string singular, string plural, long n)
		{
			var translation = default(Translation);
			if (!TryGetValue(new GettextKey(null, singular, plural), out translation))
				return null;

			return translation[PluralForms.Get(n)];
		}

		public string GetParticularPluralString(string context, string singular, string plural, long n)
		{
			var translation = default(Translation);
			if (!TryGetValue(new GettextKey(context, singular, plural), out translation))
				return null;

			return translation[PluralForms.Get(n)];
		}

		#region IReadOnlyDictionary

		public IEnumerable<GettextKey> Keys
		{
			get { return _translations.Keys; }
		}

		public IEnumerable<Translation> Values
		{
			get { return _translations.Values; }
		}

		public int Count
		{
			get { return _translations.Count; }
		}

		public Translation this[GettextKey key]
		{
			get { return _translations[key]; }
		}

		public bool ContainsKey(GettextKey key)
		{
			return _translations.ContainsKey(key);
		}

		public bool TryGetValue(GettextKey key, out Translation value)
		{
			return _translations.TryGetValue(key, out value);
		}

		public IEnumerator<KeyValuePair<GettextKey, Translation>> GetEnumerator()
		{
			return _translations.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _translations.GetEnumerator();
		}

		#endregion

		#region Static members

		private const char HEADER_SEPARATOR = '\n';
		private const char HEADER_KEY_VALUE_SEPARATOR = ':';
		private static readonly char[] HEADER_KEY_VALUE_SEPARATORS = new[] { HEADER_KEY_VALUE_SEPARATOR };

		public static Catalog Load(IGettextReader reader, CultureInfo culture = null)
		{
			if (reader == null)
				throw new ArgumentNullException(nameof(reader));

			var catalog = new Catalog();

			Translation translation;
			while ((translation = reader.ReadTranslation()) != null)
			{
				if (translation.Key.IsHeader)
				{
					var headers = translation.Single().Split(HEADER_SEPARATOR);

					foreach (var header in headers)
					{
						if (header.Trim().Length <= 0)
							continue;

						var split = header.Split(HEADER_KEY_VALUE_SEPARATORS, 2);

						if (split.Length != 2)
							throw new InvalidOperationException("Failed to parse header '" + header + "'");

						var key = split[0].Trim();
						var value = split[1].Trim();

						catalog._headers.Add(key, value);

						// accept encoding
						if (string.Equals(key, "Content-Type", StringComparison.OrdinalIgnoreCase))
						{
							var contentType = new ContentType(value);

							if (contentType.CharSet != null)
							{
								reader.Encoding = Encoding.GetEncoding(contentType.CharSet);
							}
						}
						// accept culture
						else if (string.Equals(key, "Language", StringComparison.OrdinalIgnoreCase))
						{
							var cultureInfo = new CultureInfo(value);

							catalog.Culture = cultureInfo;
						}
						// accept plural forms
						else if (string.Equals(key, "Plural-Forms", StringComparison.OrdinalIgnoreCase))
						{
							var pluralForms = PluralFormsExpression.Parse(value);

							catalog.PluralForms = pluralForms;
						}
					}
				}
				else
				{
					catalog._translations.Add(translation.Key, translation);
				}
			}

			// override culture
			if (culture != null)
				catalog.Culture = culture;

			// verify integrity
			if (catalog.Culture == null)
				throw new InvalidOperationException("Catalog doesn't have 'Language' header");
			if (catalog.PluralForms == null)
				catalog.PluralForms = PluralFormsExpression.Default;

			return catalog;
		}

		#endregion
	}
}
