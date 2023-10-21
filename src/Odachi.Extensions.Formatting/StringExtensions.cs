using System;
using System.Collections.Generic;

namespace Odachi.Extensions.Formatting
{
	[Flags]
	public enum WordSplitOptions
	{
		None = 0,
		/// <summary>
		/// Consider upper letters to be word boundary.
		/// </summary>
		SplitOnUpperLetter = 1,
	}

	public static class StringExtensions
	{
		private static readonly string[] LineSeparators = { "\r\n", "\n" };

		/// <summary>
		/// Returns `start` (inclusive) and `end` (exclusive) indexes of all parts delimited by separators in given string.
		/// </summary>
		public static IEnumerable<(int start, int end)> GetPartBoundaries(this string source, params string[] separators)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (separators == null)
				throw new ArgumentNullException(nameof(separators));
			if (separators.Length <= 0)
				throw new ArgumentException("At least one separator is required", nameof(separators));

			var startIndex = 0;
			for (var i = 0; i <= source.Length; i++)
			{
				var remaining = source.Length - i;
				var isBreaking = false;
				var breakLength = -1;

				for (var si = 0; si < separators.Length; si++)
				{
					var separator = separators[si];

					if (separator.Length > remaining)
						continue;

					var isMatch = true;

					for (var sci = 0; sci < separator.Length; sci++)
					{
						if (separator[sci] != source[i + sci])
						{
							isMatch = false;
							break;
						}
					}

					if (isMatch)
					{
						isBreaking = true;
						breakLength = separator.Length;
						break;
					}
				}

				if (isBreaking)
				{
					yield return (startIndex, i);

					startIndex = i + breakLength;

					// skip any additional breaking characters
					if (breakLength > 1)
					{
						i += breakLength - 1;
					}
				}
			}

			if (startIndex <= source.Length)
			{
				yield return (startIndex, source.Length);
			}
		}

		/// <summary>
		/// Returns all parts of given string delimited by separators.
		/// </summary>
		public static IEnumerable<string> GetParts(this string source, params string[] separators)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (separators == null)
				throw new ArgumentNullException(nameof(separators));
			if (separators.Length <= 0)
				throw new ArgumentException("At least one separator is required", nameof(separators));

			foreach (var (start, end) in GetPartBoundaries(source, separators))
			{
				yield return source.Substring(start, end - start);
			}
		}

		/// <summary>
		/// Returns `start` (inclusive) and `end` (exclusive) indexes of all lines in given string.
		/// </summary>
		public static IEnumerable<(int start, int ent)> GetLineBoundaries(this string source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			return GetPartBoundaries(source, LineSeparators);
		}

		/// <summary>
		/// Returns all lines of given string.
		/// </summary>
		public static IEnumerable<string> GetLines(this string source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			foreach (var (start, end) in GetLineBoundaries(source))
			{
				yield return source.Substring(start, end - start);
			}
		}

		/// <summary>
		/// Returns `start` (inclusive) and `end` (exclusive) indexes of all words in given string. A 'word' is considered to be any sequence of letters or digits.
		/// </summary>
		public static IEnumerable<(int start, int end)> GetWordBoundaries(this string source, WordSplitOptions options = WordSplitOptions.None)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			var splitOnUpperLetter = options.HasFlag(WordSplitOptions.SplitOnUpperLetter);

			// todo: I'm pretty confident that this can fail due to some unicode shenanigans

			var startIndex = -1;
			for (var i = 0; i < source.Length; i++)
			{
				var isLetterOrDigit = char.IsLetterOrDigit(source, i);

				if (startIndex != -1)
				{
					if (!isLetterOrDigit)
					{
						yield return (startIndex, i);
						startIndex = -1;
					}
					else if (splitOnUpperLetter)
					{
						if (char.IsUpper(source, i) && (i == 0 || !char.IsUpper(source, i - 1)))
						{
							yield return (startIndex, i);
							startIndex = i;
						}
					}
				}
				else
				{
					if (isLetterOrDigit)
					{
						startIndex = i;
					}
				}
			}

			if (startIndex != -1)
			{
				yield return (startIndex, source.Length);
			}
		}

		/// <summary>
		/// Returns all words in given string. A 'word' is considered to be any sequence of letters or digits.
		/// </summary>
		public static IEnumerable<string> GetWords(this string source, WordSplitOptions options = WordSplitOptions.None)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			foreach (var (start, end) in GetWordBoundaries(source, options: options))
			{
				yield return source.Substring(start, end - start);
			}
		}

		/// <summary>
		/// Convert casing convention of all words to PascalCase.
		/// </summary>
		public static string ToPascalInvariant(this string source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			var chars = source.ToCharArray();
			foreach (var (start, _) in source.GetWordBoundaries())
			{
				var ch = chars[start];

				if (char.IsLower(ch))
				{
					chars[start] = char.ToUpperInvariant(ch);
				}
			}
			return new string(chars);
		}

		/// <summary>
		/// Convert casing convention of all words to camelCase.
		/// </summary>
		public static string ToCamelInvariant(this string source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));

			var chars = source.ToCharArray();
			foreach (var (start, end) in source.GetWordBoundaries())
			{
				for (var i = start; i < end; i++)
				{
					var ch = chars[i];

					if (char.IsLower(ch))
					{
						break;
					}

					chars[i] = char.ToLowerInvariant(ch);
				}
			}
			return new string(chars);
		}
	}
}
