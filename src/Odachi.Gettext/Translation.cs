using System;
using System.Collections;
using System.Collections.Generic;

namespace Odachi.Gettext
{
	public class GettextKey : IEquatable<GettextKey>
	{
		public GettextKey()
		{
			Context = null;
			Singular = "";
			Plural = null;
		}
		public GettextKey(string context, string singular, string plural)
		{
			if (singular == null)
				throw new ArgumentNullException(nameof(singular));

			Context = context;
			Singular = singular;
			Plural = plural;
		}

		public string Context { get; }
		public string Singular { get; }
		public string Plural { get; }

		public bool IsHeader
		{
			get { return Context == null && Singular == "" && Plural == null; }
		}

		public override int GetHashCode()
		{
			var hash = 0x1505L;

			if (Context != null)
				hash = ((hash << 5) + hash) ^ Context.GetHashCode();
			hash = ((hash << 5) + hash) ^ Singular.GetHashCode();
			if (Plural != null)
				hash = ((hash << 5) + hash) ^ Plural.GetHashCode();

			return hash.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is GettextKey))
				return false;

			return Equals((GettextKey)obj);
		}

		public bool Equals(GettextKey other)
		{
			return
				Context == other.Context &&
				Singular == other.Singular &&
				Plural == other.Plural;
		}
	}

	public class Translation : IReadOnlyList<string>
	{
		public Translation(GettextKey key, string[] translations)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));
			if (translations == null)
				throw new ArgumentNullException(nameof(translations));

			Key = key;
			_translations = translations;
		}

		private string[] _translations;

		public GettextKey Key { get; }

		public int Count
		{
			get { return _translations.Length; }
		}

		public string this[int index]
		{
			get { return _translations[index]; }
		}

		public IEnumerator<string> GetEnumerator()
		{
			return ((IEnumerable<string>)_translations).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _translations.GetEnumerator();
		}
	}
}
