#nullable enable

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Odachi.Security
{
	public sealed class ClaimVariable
	{
		private ClaimVariable()
		{ }

		public static readonly ClaimVariable Any = new();
	}

	public class ClaimTemplate
	{
		private static readonly Regex ParameterRegex = new("\\{\\d+\\}", RegexOptions.CultureInvariant);

		private readonly string[] _parts;

		public ClaimTemplate(string template)
		{
			Template = template ?? throw new ArgumentNullException(nameof(template));
			_parts = ParameterRegex.Split(template);
		}

		public string Template { get; }
		public int VariableCount => _parts.Length - 1;

		public string Format(params object?[] variables)
		{
			var builder = new StringBuilder(Template.Length + VariableCount * 5);
			Format(builder, variables);
			return builder.ToString();
		}
		public void Format(StringBuilder builder, params object?[] variables)
		{
			ArgumentNullException.ThrowIfNull(builder);
			ArgumentNullException.ThrowIfNull(variables);
			if (variables.Length != VariableCount)
				throw new ArgumentOutOfRangeException(nameof(variables), $"Expected {VariableCount} variables, received {variables.Length}");

			static string FormatVariable(object? variable)
			{
				var result = variable?.ToString() ?? "";

				if (result.Length <= 0)
				{
					throw new InvalidOperationException("All variables need to have non-zero length value.");
				}

				return result;
			}

			for (var i = 0; i < variables.Length; i++)
			{
				builder.Append(_parts[i]);
				if (i < _parts.Length - 1)
				{
					builder.Append(FormatVariable(variables[i]));
				}
			}
		}

		public bool Matches(string value, params object?[] variables)
		{
			if (variables.Length != VariableCount)
			{
				return false;
			}

			static bool VariableMatches(ReadOnlySpan<char> actual, object? expected)
			{
				// variable is required
				if (actual.Length <= 0)
				{
					return false;
				}

				// variable can have any value
				if (expected == ClaimVariable.Any)
				{
					return true;
				}

				return actual.SequenceEqual(expected?.ToString() ?? "");
			}

			var previousEnd = -1;
			for (var i = 0; i < _parts.Length; i++)
			{
				var templatePart = _parts[i];

				var index = value.IndexOf(templatePart, previousEnd == -1 ? 0 : previousEnd);
				if (index == -1)
				{
					// part #{i} doesn't match
					return false;
				}

				if (i == 0)
				{
					// first part must match start of the value
					if (index != 0)
					{
						// start doesn't match
						return false;
					}
				}

				if (i == _parts.Length - 1)
				{
					// last part must match end of the value
					if (index + templatePart.Length != value.Length)
					{
						// unless last part is empty which means last token is a variable
						if (templatePart.Length == 0)
						{
							return VariableMatches(value.AsSpan()[index..], variables[i - 1]);
						}

						// end doesn't match
						return false;
					}
				}

				if (i != 0)
				{
					if (!VariableMatches(value.AsSpan().Slice(previousEnd, index - previousEnd), variables[i - 1]))
					{
						// variable doesn't match
						return false;
					}
				}

				previousEnd = index + templatePart.Length;
			}

			return true;
		}

		public IEnumerable<string> ReadVariables(string value)
		{
			string FormatVariable(ReadOnlySpan<char> actual)
			{
				// variable is required
				if (actual.Length <= 0)
				{
					throw new ArgumentException($"Value '{value}' doesn't match template '{Template}' (missing variable)");
				}

				return actual.ToString();
			}

			var previousEnd = -1;
			for (var i = 0; i < _parts.Length; i++)
			{
				var templatePart = _parts[i];

				var index = value.IndexOf(templatePart, previousEnd == -1 ? 0 : previousEnd);
				if (index == -1)
				{
					// part #{i} doesn't match
					throw new ArgumentException($"Value '{value}' doesn't match template '{Template}' (part #{i} doesn't match)");
				}

				// first part must match start of the value
				if (i == 0 && index != 0)
				{
					// start doesn't match
					throw new ArgumentException($"Value '{value}' doesn't match template '{Template}' (start doesn't match)");
				}

				// last part must be at end
				if (i == _parts.Length - 1 && index + templatePart.Length != value.Length)
				{
					// unless last part is empty which means last token is a variable
					if (templatePart.Length == 0)
					{
						yield return FormatVariable(value.AsSpan()[index..]);
					}
					else
					{
						// end doesn't match
						throw new ArgumentException($"Value '{value}' doesn't match template '{Template}' (end doesn't match)");
					}
				}
				else if (i != 0)
				{
					yield return FormatVariable(value.AsSpan().Slice(previousEnd, index - previousEnd));
				}

				previousEnd = index + templatePart.Length;
			}
		}
	}
}
