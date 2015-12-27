using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Localization.Extraction
{
	public struct SourceInfo
	{
		public SourceInfo(string fileName, int? lineIndex, int? characterIndex)
		{
			FileName = fileName;
			LineIndex = lineIndex;
			CharacterIndex = characterIndex;
		}

		public string FileName { get; set; }
		public int? LineIndex { get; set; }
		public int? CharacterIndex { get; set; }
	}

	public class KeywordMatchBase
	{
		public KeywordMatchBase(string singular, string plural, string context)
		{
			if (singular == null)
				throw new ArgumentNullException(nameof(singular));

			Singular = singular;
			Plural = plural;
			Context = context;
		}

		public string Singular { get; private set; }
		public string Plural { get; private set; }
		public string Context { get; private set; }

		public override int GetHashCode()
		{
			return Singular.GetHashCode() ^ (Plural?.GetHashCode() ?? 0) ^ (Context?.GetHashCode() ?? 0);
		}

		public override bool Equals(object obj)
		{
			var other = obj as KeywordMatchBase;
			if (other == null)
				return false;

			return
				string.Equals(Singular, other.Singular, StringComparison.Ordinal) &&
				string.Equals(Plural, other.Plural, StringComparison.Ordinal) &&
				string.Equals(Context, other.Context, StringComparison.Ordinal);
		}
	}

	public class KeywordMatch : KeywordMatchBase
	{
		public KeywordMatch(string singular, string plural, string context, SourceInfo? source = null)
			: base(singular, plural, context)
		{
			Source = source;
		}

		public SourceInfo? Source { get; private set; }
	}

	public class KeywordMatchGroup : KeywordMatchBase
	{
		public KeywordMatchGroup(string singular, string plural, string context, IEnumerable<SourceInfo> sources = null)
			: base(singular, plural, context)
		{
			Sources = sources ?? Enumerable.Empty<SourceInfo>();
		}

		public IEnumerable<SourceInfo> Sources { get; private set; }
	}

	// http://www.gnu.org/software/gettext/manual/html_node/xgettext-Invocation.html#xgettext-Invocation
	// TODO: support comments
	// TODO: support total
	// TODO: glib sytax?
	public class Keyword
	{
		private Keyword(string id, int singular, int? plural = null, int? context = null)
		{
			if (id == null)
				throw new ArgumentNullException(nameof(id));
			if (singular <= 0)
				throw new ArgumentOutOfRangeException(nameof(singular));
			if (plural != null && plural <= 0)
				throw new ArgumentOutOfRangeException(nameof(plural));
			if (context != null && context <= 0)
				throw new ArgumentOutOfRangeException(nameof(context));

			Id = id;
			Singular = singular;
			Plural = plural;
			Context = context;

			RequiredArguments = Math.Max(Singular, Math.Max(Plural ?? 0, Context ?? 0));
		}

		public string Id { get; private set; }

		public int Singular { get; private set; }
		public int? Plural { get; private set; }
		public int? Context { get; private set; }

		public int RequiredArguments { get; private set; }

		public KeywordMatch Match(Invocation call)
		{
			if (Id != call.Name || call.Arguments.Count < RequiredArguments)
				return null;

			var singular = call.Arguments[Singular - 1];
			var plural = Plural == null ? new Argument() : call.Arguments[Plural.Value - 1];
			var context = Context == null ? new Argument() : call.Arguments[Context.Value - 1];

			if (singular.Value == null)
				return null;

			if (Plural != null && plural.Value == null)
				return null;

			if (Context != null && context.Value == null)
				return null;

			return new KeywordMatch(singular.Value, plural.Value, context.Value, source: call.Source);
		}

		public static Keyword Parse(string keyword)
		{
			if (keyword == null)
				throw new ArgumentNullException(nameof(keyword));

			var colonIndex = keyword.IndexOf(':');
			if (colonIndex == -1)
				// format 'id'
				return new Keyword(keyword, 1);

			var id = keyword.Substring(0, colonIndex);
			var args = keyword.Substring(colonIndex + 1).Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

			int? singular = null;
			int? plural = null;
			int? context = null;

			foreach (var arg in args)
			{
				if (arg.EndsWith("c", StringComparison.OrdinalIgnoreCase))
				{
					var num = int.Parse(arg.Substring(0, arg.Length - 1));

					if (context == null)
						context = num;
					else
						throw new InvalidOperationException("Both singular and plural forms are already defined for keyword '" + keyword + "'");
				}
				else
				{
					var num = int.Parse(arg);

					if (singular == null)
						singular = num;
					else if (plural == null)
						plural = num;
					else
						throw new InvalidOperationException("Both singular and plural forms are already defined for keyword '" + keyword + "'");
				}
			}

			return new Keyword(id, singular ?? 1, plural: plural, context: context);
		}
	}
}
