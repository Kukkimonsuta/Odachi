using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

namespace Odachi.Localization
{
	public class GettextStringLocalizer : IStringLocalizer
	{
		public GettextStringLocalizer(IGettextProcessor processor)
		{
			if (processor == null)
				throw new ArgumentNullException(nameof(processor));

			Processor = processor;
		}

		protected IGettextProcessor Processor { get; private set; }

		protected virtual CultureInfo Culture
		{
			get { return CultureInfo.CurrentUICulture; }
		}

		public string GetString(string name)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			return Processor.GetString(Culture, name);
		}

		public string GetString(string name, params object[] args)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			return string.Format(
				GetString(name),
				args
			);
		}

		public string GetPluralString(string singular, string plural, long n)
		{
			if (singular == null)
				throw new ArgumentNullException(nameof(singular));
			if (plural == null)
				throw new ArgumentNullException(nameof(plural));

			return Processor.GetPluralString(Culture, singular, plural, n);
		}

		public string GetPluralString(string singular, string plural, long n, params object[] args)
		{
			if (singular == null)
				throw new ArgumentNullException(nameof(singular));
			if (plural == null)
				throw new ArgumentNullException(nameof(plural));

			return string.Format(
				GetPluralString(singular, plural, n),
				args
			);
		}

		public string GetParticularString(string context, string name)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			return Processor.GetParticularString(Culture, context, name);
		}

		public string GetParticularString(string context, string name, params object[] args)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			return string.Format(
				GetParticularString(context, name),
				args
			);
		}

		public string GetParticularPluralString(string context, string singular, string plural, long n)
		{
			if (singular == null)
				throw new ArgumentNullException(nameof(singular));
			if (plural == null)
				throw new ArgumentNullException(nameof(plural));

			return Processor.GetParticularPluralString(Culture, context, singular, plural, n);
		}

		public string GetParticularPluralString(string context, string singular, string plural, long n, params object[] args)
		{
			if (singular == null)
				throw new ArgumentNullException(nameof(singular));
			if (plural == null)
				throw new ArgumentNullException(nameof(plural));

			return string.Format(
				GetParticularPluralString(context, singular, plural, n),
				args
			);
		}

		public LocalizedString this[string name]
		{
			get
			{
				if (name == null)
					throw new ArgumentNullException(nameof(name));

				var value = GetString(name);
				return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
			}
		}

		public LocalizedString this[string name, params object[] arguments]
		{
			get
			{
				if (name == null)
					throw new ArgumentNullException(nameof(name));

				var format = GetString(name);
				var value = string.Format(format ?? name, arguments);
				return new LocalizedString(name, value, resourceNotFound: format == null);
			}
		}

		public IEnumerable<LocalizedString> GetAllStrings(bool includeAncestorCultures)
		{
			return Processor.GetAllStrings(Culture, includeAncestorCultures);
		}

		public IStringLocalizer WithCulture(CultureInfo culture)
		{
			if (culture == null)
				return new GettextStringLocalizer(Processor);

			return new StaticGettextStringLocalizer(Processor, culture);
		}
	}

	public class StaticGettextStringLocalizer : GettextStringLocalizer
	{
		public StaticGettextStringLocalizer(IGettextProcessor processor, CultureInfo culture)
			: base(processor)
		{
			if (processor == null)
				throw new ArgumentNullException(nameof(processor));
			if (culture == null)
				throw new ArgumentNullException(nameof(culture));

			_culture = culture;
		}

		private CultureInfo _culture;

		protected override CultureInfo Culture
		{
			get { return _culture; }
		}
	}
}
