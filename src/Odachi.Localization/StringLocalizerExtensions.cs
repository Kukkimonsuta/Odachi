using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

namespace Odachi.Localization
{
	public static class StringLocalizerExtensions
	{
		public static string GetPluralString(this IStringLocalizer localizer, string singular, string plural, long n)
		{
			var gettext = localizer as GettextStringLocalizer;
			if (gettext == null)
				throw new InvalidOperationException("GetPluralString is supported only for GettextStringLocalizer");

			return gettext.GetPluralString(singular, plural, n);
		}
		public static string GetPluralString(this IStringLocalizer localizer, string singular, string plural, long n, params object[] args)
		{
			var gettext = localizer as GettextStringLocalizer;
			if (gettext == null)
				throw new InvalidOperationException("GetPluralString is supported only for GettextStringLocalizer");

			return gettext.GetPluralString(singular, plural, n, args);
		}

		public static string GetParticularString(this IStringLocalizer localizer, string context, string name)
		{
			var gettext = localizer as GettextStringLocalizer;
			if (gettext == null)
				throw new InvalidOperationException("GetParticularString is supported only for GettextStringLocalizer");

			return gettext.GetParticularString(context, name);
		}

		public static string GetParticularString(this IStringLocalizer localizer, string context, string name, params object[] args)
		{
			var gettext = localizer as GettextStringLocalizer;
			if (gettext == null)
				throw new InvalidOperationException("GetParticularString is supported only for GettextStringLocalizer");

			return gettext.GetParticularString(context, name, args);
		}

		public static string GetParticularPluralString(this IStringLocalizer localizer, string context, string singular, string plural, long n)
		{
			var gettext = localizer as GettextStringLocalizer;
			if (gettext == null)
				throw new InvalidOperationException("GetParticularPluralString is supported only for GettextStringLocalizer");

			return gettext.GetParticularPluralString(context, singular, plural, n);
		}

		public static string GetParticularPluralString(this IStringLocalizer localizer, string context, string singular, string plural, long n, params object[] args)
		{
			var gettext = localizer as GettextStringLocalizer;
			if (gettext == null)
				throw new InvalidOperationException("GetParticularPluralString is supported only for GettextStringLocalizer");

			return gettext.GetParticularPluralString(context, singular, plural, n, args);
		}
	}
}
