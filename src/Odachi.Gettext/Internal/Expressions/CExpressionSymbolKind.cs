using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Gettext.Internal.Expressions
{
	internal enum CExpressionSymbolKind
	{
		IntegerLiteral = 1,
		WhiteSpace = 2,
		Modulo = 3,
		QuestionMark = 4,
		Colon = 5,
		DoubleAnd = 6,
		DoubleOr = 7,
		LeftParenthesis = 8,
		RightParenthesis = 9,
		LessThan = 10,
		LessThanEqual = 11,
		GreaterThan = 12,
		GreaterThanEqual = 13,
		Identifier = 14,
		Not,
		Equal,
		NotEqual,
	}
}
