using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Gettext.Internal.Expressions
{
	internal class CExpressionTokenizer
	{
		public CExpressionTokenizer()
		{
		}

		public IEnumerable<CExpressionSymbol> Tokenize(string input)
		{
			// this should be large enough to handle all symbols
			// todo: automatically bump size?
			var buffer = new char[256];
			var bufferIndex = 0;

			var index = 0;

			while (input.Length > index)
			{
				var ch = input[index++];

				if (char.IsDigit(ch))
				{
					bufferIndex = 0;
					buffer[bufferIndex++] = ch;

					while (input.Length > index && char.IsDigit(input[index]))
					{
						buffer[bufferIndex++] = input[index++];
					}

					yield return new CExpressionSymbol(CExpressionSymbolKind.IntegerLiteral, new string(buffer, 0, bufferIndex));
				}
				else if (ch == '?')
					yield return new CExpressionSymbol(CExpressionSymbolKind.QuestionMark, ch.ToString());
				else if (ch == ':')
					yield return new CExpressionSymbol(CExpressionSymbolKind.Colon, ch.ToString());
				else if (ch == '(')
					yield return new CExpressionSymbol(CExpressionSymbolKind.LeftParenthesis, ch.ToString());
				else if (ch == ')')
					yield return new CExpressionSymbol(CExpressionSymbolKind.RightParenthesis, ch.ToString());
				else if (ch == '%')
					yield return new CExpressionSymbol(CExpressionSymbolKind.Modulo, ch.ToString());
				else if (ch == 'n')
					yield return new CExpressionSymbol(CExpressionSymbolKind.Identifier, ch.ToString());
				else if (ch == '<')
				{
					if (input.Length > index && input[index] == '=')
					{
						index++;
						yield return new CExpressionSymbol(CExpressionSymbolKind.LessThanEqual, "<=");
					}
					else
						yield return new CExpressionSymbol(CExpressionSymbolKind.LessThan, ch.ToString());
				}
				else if (ch == '>')
				{
					if (input.Length > index && input[index] == '=')
					{
						index++;
						yield return new CExpressionSymbol(CExpressionSymbolKind.GreaterThanEqual, ">=");
					}
					else
						yield return new CExpressionSymbol(CExpressionSymbolKind.GreaterThan, ch.ToString());
				}
				else if (ch == '|' && input[index] == '|')
				{
					index++;
					yield return new CExpressionSymbol(CExpressionSymbolKind.DoubleOr, "||");
				}
				else if (ch == '&' && input[index] == '&')
				{
					index++;
					yield return new CExpressionSymbol(CExpressionSymbolKind.DoubleAnd, "&&");
				}
				else if (ch == '!')
				{
					if (input.Length > index && input[index] == '=')
					{
						index++;
						yield return new CExpressionSymbol(CExpressionSymbolKind.NotEqual, "!=");
					}
					else
						yield return new CExpressionSymbol(CExpressionSymbolKind.Not, "!");
				}
				else if (ch == '=' && input.Length > index && input[index] == '=')
				{
					index++;
					yield return new CExpressionSymbol(CExpressionSymbolKind.Equal, "==");
				}
				else if (char.IsWhiteSpace(ch))
				{
					bufferIndex = 0;
					buffer[bufferIndex++] = ch;

					while (input.Length > index && char.IsWhiteSpace(input[index]))
					{
						buffer[bufferIndex++] = input[index++];
					}

					yield return new CExpressionSymbol(CExpressionSymbolKind.WhiteSpace, new string(buffer, 0, bufferIndex));
				}
				else
					throw new FormatException("Invalid plural forms format '" + input + "'");
			}
		}
	}
}
