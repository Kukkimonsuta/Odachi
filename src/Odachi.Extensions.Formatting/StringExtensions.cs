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
