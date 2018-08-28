using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.CodeGen.Internal
{
	public static class StringBuilderExtensions
	{
		public static void TrimEnd(this StringBuilder builder)
		{
			if (builder.Length <= 0)
				return;

			var index = builder.Length - 1;
			while (index > 0 && char.IsWhiteSpace(builder[index]))
			{
				index--;
			}

			builder.Length = index + 1;
		}
	}
}
