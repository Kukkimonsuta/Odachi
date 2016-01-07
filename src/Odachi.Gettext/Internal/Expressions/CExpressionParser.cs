using Odachi.Gettext.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Odachi.Gettext.Internal.Expressions
{
	/// <summary>
	/// Basic c expression parser. The parser is pretty simple and incomplete, ought to be used only for parsing of gettext plural expressions.
	/// </summary>
	internal class CExpressionParser
	{
		public Expression Parse(string expression, params ParameterExpression[] identifiers)
		{
			if (expression == null)
				throw new ArgumentNullException(nameof(expression));
			if (identifiers == null)
				throw new ArgumentNullException(nameof(identifiers));

			return Parse(expression, name =>
				identifiers.Where(i => i.Name == name).FirstOrDefault()
			);
		}
		public Expression Parse(string expression, Func<string, Expression> identifierHandler)
		{
			if (expression == null)
				throw new ArgumentNullException(nameof(expression));
			if (identifierHandler == null)
				throw new ArgumentNullException(nameof(identifierHandler));

			var tokenizer = new CExpressionTokenizer();
			var symbols = tokenizer.Tokenize(expression);

			return Parse(symbols, identifierHandler);
		}
		public Expression Parse(IEnumerable<CExpressionSymbol> symbols, Func<string, Expression> identifierHandler)
		{
			if (symbols == null)
				throw new ArgumentNullException(nameof(symbols));
			if (identifierHandler == null)
				throw new ArgumentNullException(nameof(identifierHandler));

			var relevantSymbols = symbols
				.Where(s => s.Kind != CExpressionSymbolKind.WhiteSpace)
				.Cast<object>()
				.ToArray();

			return Parse(relevantSymbols, identifierHandler);
		}

		private Expression Parse(IReadOnlyList<object> symbols, Func<string, Expression> identifierHandler)
		{
			var tree = new List<object>(symbols);

			CExpressionSymbol symbol;

			// handle constants
			while ((symbol = tree
				.OfType<CExpressionSymbol>()
				.Where(s => s.Kind == CExpressionSymbolKind.IntegerLiteral)
				.FirstOrDefault()) != null)
			{
				var index = tree.IndexOf(symbol);

				var expression = Expression.Constant(long.Parse(symbol.Text), typeof(long));

				tree.RemoveAt(index);
				tree.Insert(index, expression);
			}

			// handle identifiers
			while ((symbol = tree
				.OfType<CExpressionSymbol>()
				.Where(s => s.Kind == CExpressionSymbolKind.Identifier)
				.FirstOrDefault()) != null)
			{
				var index = tree.IndexOf(symbol);
				tree.RemoveAt(index);

				var expression = identifierHandler(symbol.Text);
				if (expression == null)
					throw new InvalidOperationException("Unhandled identifier '" + symbol.Text + "'");

				tree.Insert(index, expression);
			}

			// handle parenthesis
			while ((symbol = tree
				.OfType<CExpressionSymbol>()
				.Where(s => s.Kind == CExpressionSymbolKind.LeftParenthesis)
				.LastOrDefault()) != null)
			{
				var index = tree.IndexOf(symbol);
				var length = -1;
				for (var i = 0; i < tree.Count - index; i++)
				{
					var s = tree[index + 1 + i] as CExpressionSymbol;
					if (s == null)
						continue;

					if (s.Kind == CExpressionSymbolKind.RightParenthesis)
					{
						length = i;
						break;
					}
				}
				if (length < 0)
					throw new FormatException("Couldn't find match for left brace");

				var expression = Parse(new ReadOnlyListSegment<object>(tree, index + 1, length), identifierHandler);

				tree.RemoveRange(index, length + 2);
				tree.Insert(index, expression);
			}

			// handle negation (right associative)
			while ((symbol = tree
				.OfType<CExpressionSymbol>()
				.Where(s => s.Kind == CExpressionSymbolKind.Not)
				.LastOrDefault()) != null)
			{
				var index = tree.IndexOf(symbol);
				var what = tree[index + 1] as Expression;
				if (what == null)
					throw new InvalidOperationException("Cannot negate '" + what + "'");

				var expression = Expression.Not(what);

				tree.RemoveRange(index, 2);
				tree.Insert(index, expression);
			}

			// handle modulo
			while ((symbol = tree
				.OfType<CExpressionSymbol>()
				.Where(s => s.Kind == CExpressionSymbolKind.Modulo)
				.FirstOrDefault()) != null)
			{
				var index = tree.IndexOf(symbol);

				var left = tree[index - 1] as Expression;
				if (left == null)
					throw new InvalidOperationException("Cannot mod left '" + left + "'");

				var right = tree[index + 1] as Expression;
				if (right == null)
					throw new InvalidOperationException("Cannot mod right '" + right + "'");

				var expression = Expression.Modulo(left, right);

				tree.RemoveRange(index - 1, 3);
				tree.Insert(index - 1, expression);
			}

			// handle comparisons
			while ((symbol = tree
				.OfType<CExpressionSymbol>()
				.Where(s =>
					s.Kind == CExpressionSymbolKind.Equal || s.Kind == CExpressionSymbolKind.NotEqual ||
					s.Kind == CExpressionSymbolKind.LessThan || s.Kind == CExpressionSymbolKind.LessThanEqual ||
					s.Kind == CExpressionSymbolKind.GreaterThan || s.Kind == CExpressionSymbolKind.GreaterThanEqual
				)
				.FirstOrDefault()) != null)
			{
				var index = tree.IndexOf(symbol);

				var left = tree[index - 1] as Expression;
				if (left == null)
					throw new InvalidOperationException("Cannot compare left '" + left + "'");

				var right = tree[index + 1] as Expression;
				if (right == null)
					throw new InvalidOperationException("Cannot compare right '" + right + "'");

				Expression expression;
				switch (symbol.Kind)
				{
					case CExpressionSymbolKind.Equal:
						expression = Expression.Equal(left, right);
						break;

					case CExpressionSymbolKind.NotEqual:
						expression = Expression.NotEqual(left, right);
						break;

					case CExpressionSymbolKind.LessThan:
						expression = Expression.LessThan(left, right);
						break;

					case CExpressionSymbolKind.LessThanEqual:
						expression = Expression.LessThanOrEqual(left, right);
						break;

					case CExpressionSymbolKind.GreaterThan:
						expression = Expression.GreaterThan(left, right);
						break;

					case CExpressionSymbolKind.GreaterThanEqual:
						expression = Expression.GreaterThanOrEqual(left, right);
						break;

					default:
						throw new InvalidOperationException("Undefined behavior for '" + symbol.Kind + "'");
				}

				tree.RemoveRange(index - 1, 3);
				tree.Insert(index - 1, expression);
			}

			// handle and
			while ((symbol = tree
				.OfType<CExpressionSymbol>()
				.Where(s => s.Kind == CExpressionSymbolKind.DoubleAnd)
				.FirstOrDefault()) != null)
			{
				var index = tree.IndexOf(symbol);

				var left = tree[index - 1] as Expression;
				if (left == null)
					throw new InvalidOperationException("Cannot and left '" + left + "'");

				var right = tree[index + 1] as Expression;
				if (right == null)
					throw new InvalidOperationException("Cannot and right '" + right + "'");

				var expression = Expression.AndAlso(left, right);

				tree.RemoveRange(index - 1, 3);
				tree.Insert(index - 1, expression);
			}

			// handle or
			while ((symbol = tree
				.OfType<CExpressionSymbol>()
				.Where(s => s.Kind == CExpressionSymbolKind.DoubleOr)
				.FirstOrDefault()) != null)
			{
				var index = tree.IndexOf(symbol);

				var left = tree[index - 1] as Expression;
				if (left == null)
					throw new InvalidOperationException("Cannot or left '" + left + "'");

				var right = tree[index + 1] as Expression;
				if (right == null)
					throw new InvalidOperationException("Cannot or right '" + right + "'");

				var expression = Expression.OrElse(left, right);

				tree.RemoveRange(index - 1, 3);
				tree.Insert(index - 1, expression);
			}

			// handle ternary (right associative)
			while ((symbol = tree
				.OfType<CExpressionSymbol>()
				.Where(s => s.Kind == CExpressionSymbolKind.QuestionMark)
				.LastOrDefault()) != null)
			{
				var index = tree.IndexOf(symbol);

				var condition = tree[index - 1] as Expression;
				if (condition == null)
					throw new InvalidOperationException("Cannot use ternary condition '" + condition + "'");

				Expression ifTrue;
				var ifTrueLength = -1;
				{
					for (var i = 0; i < tree.Count - index; i++)
					{
						var s = tree[index + 1 + i] as CExpressionSymbol;
						if (s == null)
							continue;

						if (s.Kind == CExpressionSymbolKind.Colon)
						{
							ifTrueLength = i;
							break;
						}
					}
					if (ifTrueLength < 0)
						throw new FormatException("Couldn't find ternary true");

					ifTrue = Parse(new ReadOnlyListSegment<object>(tree, index + 1, ifTrueLength), identifierHandler);
				}

				Expression ifFalse;
				var ifFalseLength = tree.Count - index - 2 - ifTrueLength;
				ifFalse = Parse(new ReadOnlyListSegment<object>(tree, index + 2 + ifTrueLength, ifFalseLength), identifierHandler);

				var expression = Expression.Condition(condition, ifTrue, ifFalse);

				tree.RemoveRange(index - 1, 3 + ifTrueLength + ifFalseLength);
				tree.Insert(index - 1, expression);
			}

			Expression result;

			if (tree.Count != 1 || (result = tree[0] as Expression) == null)
				throw new InvalidOperationException("Couldn't build expression from input");

			return result;
		}
	}
}
