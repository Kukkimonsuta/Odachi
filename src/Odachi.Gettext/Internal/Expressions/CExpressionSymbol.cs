using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Gettext.Internal.Expressions
{
	internal class CExpressionSymbol
	{
		public CExpressionSymbol(CExpressionSymbolKind kind, string text)
		{
			Kind = kind;
			Text = text;
		}

		public CExpressionSymbolKind Kind { get; set; }

		public string Text { get; set; }

		public override string ToString()
		{
			return "Symbol '" + Text + "' (" + Kind + ")";
		}
	}
}
