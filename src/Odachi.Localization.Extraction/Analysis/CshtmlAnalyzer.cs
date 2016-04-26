using Microsoft.AspNetCore.Razor;
using Microsoft.AspNetCore.Razor.Parser;
using Microsoft.AspNetCore.Razor.Parser.SyntaxTree;
using Microsoft.AspNetCore.Razor.Tokenizer.Symbols;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Localization.Extraction.Analysis
{
	public class CshtmlAnalyzer : IAnalyzer
	{
		private CSharpSymbol ReadSymbol(IReadOnlyList<ISymbol> symbols, ref int index, bool ignoreWhitespace = true, bool ignoreComments = true)
		{
			while (symbols.Count > index)
			{
				var symbol = (CSharpSymbol)symbols[index++];

				// ignore comments
				if (ignoreComments && symbol.Type == CSharpSymbolType.Comment)
					continue;

				// ignore whitespaces
				if (ignoreWhitespace && symbol.Type == CSharpSymbolType.WhiteSpace)
					continue;

				return symbol;
			}

			return null;
		}

		private IReadOnlyList<CSharpSymbol> ReadArgument(IReadOnlyList<ISymbol> symbols, ref int index, CSharpSymbolType closeSymbolType)
		{
			var result = new List<CSharpSymbol>();

			var depth = 0;

			while (true)
			{
				var symbol = ReadSymbol(symbols, ref index);
				if (symbol == null)
					return null;

				if (depth == 0 && (symbol.Type == CSharpSymbolType.Comma || symbol.Type == closeSymbolType))
				{
					index--;
					return result;
				}

				if (symbol.Type == CSharpSymbolType.LeftBrace || symbol.Type == CSharpSymbolType.LeftBracket || symbol.Type == CSharpSymbolType.LeftParenthesis)
					depth++;
				else if (symbol.Type == CSharpSymbolType.RightBrace || symbol.Type == CSharpSymbolType.RightBracket || symbol.Type == CSharpSymbolType.RightParenthesis)
					depth--;

				result.Add(symbol);
			}
		}

		private Invocation PeekMethodCall(IReadOnlyList<ISymbol> symbols, int index, string fileName, int lineIndex, int characterIndex)
		{
			var nameSymbol = ReadSymbol(symbols, ref index, ignoreComments: false, ignoreWhitespace: false);
			if (nameSymbol == null || nameSymbol.Type != CSharpSymbolType.Identifier)
				return null;

			CSharpSymbolType closeSymbolType;
			var openSymbol = ReadSymbol(symbols, ref index);
			if (openSymbol?.Type == CSharpSymbolType.LeftParenthesis)
				closeSymbolType = CSharpSymbolType.RightParenthesis;
			else if (openSymbol?.Type == CSharpSymbolType.LeftBracket)
				closeSymbolType = CSharpSymbolType.RightBracket;
			else
				return null;

			lineIndex += nameSymbol.Start.LineIndex;
			characterIndex = nameSymbol.Start.LineIndex == 0 ? characterIndex + nameSymbol.Start.CharacterIndex : nameSymbol.Start.CharacterIndex;

			var arguments = new List<Argument>();
			while (true)
			{
				var result = ReadArgument(symbols, ref index, closeSymbolType);
				if (result == null)
					return null;

				if (result.Count <= 0)
				{
					var closeSymbol = ReadSymbol(symbols, ref index);
					if (closeSymbol.Type != closeSymbolType)
						// sytax error?
						return null;

					return new Invocation(
						nameSymbol.Content, arguments,
						source: new SourceInfo(fileName, lineIndex, characterIndex)
					);
				}
				else if (result.Count == 1)
				{
					var symbol = result.Single();

					if (symbol.Type == CSharpSymbolType.StringLiteral)
					{
						var str = symbol.Content;

						if (str.StartsWith("@\""))
							str = str.Substring(2, str.Length - 3);
						else if (str.StartsWith("\""))
							str = str.Substring(1, str.Length - 2);
						else
							// sytax error? new string literal?
							return null;

						arguments.Add(new Argument(null, str));
					}
					else
					{
						// unsupported argument
						arguments.Add(new Argument());
					}
				}
				else
					// unsupported argument
					arguments.Add(new Argument());

				var commaOrCloseSymbol = ReadSymbol(symbols, ref index);
				if (commaOrCloseSymbol == null)
					return null;

				if (commaOrCloseSymbol.Type == closeSymbolType)
				{
					// complete the call
					return new Invocation(
						nameSymbol.Content, arguments,
						source: new SourceInfo(fileName, lineIndex, characterIndex)
					);
				}
				else if (commaOrCloseSymbol.Type == CSharpSymbolType.Comma)
					continue;

				return null;
			}
		}

		#region IExtractor

		public string DefaultExtension
		{
			get
			{
				return ".cshtml";
			}
		}

		public IEnumerable<Keyword> DefaultKeywords
		{
			get
			{
				return new[]
				{
					Keyword.Parse("GetString"),
					Keyword.Parse("GetPluralString:1,2"),
					Keyword.Parse("GetParticularString:1c,2"),
					Keyword.Parse("GetParticularPluralString:1c,2,3")
				};
			}
		}

		public IEnumerable<Invocation> GetInvocations(TextReader reader, string fileName = null)
		{
			var result = new List<Invocation>();

			var parser = new RazorParser(new CSharpCodeParser(), new HtmlMarkupParser(), tagHelperDescriptorResolver: null);

			var parserResults = parser.Parse(reader);
			var codeSpans = parserResults.Document.Flatten()
				.Where(s => s.Kind == SpanKind.Code);

			foreach (var codeSpan in codeSpans)
			{
				var symbols = codeSpan.Symbols as IReadOnlyList<ISymbol> ?? codeSpan.Symbols.ToArray();

				for (var i = 0; i < symbols.Count; i++)
				{
					var call = PeekMethodCall(symbols, i, fileName, codeSpan.Start.LineIndex, codeSpan.Start.CharacterIndex);
					if (call == null)
						continue;

					result.Add(call);
				}
			}

			return result;
		}

		#endregion
	}
}
