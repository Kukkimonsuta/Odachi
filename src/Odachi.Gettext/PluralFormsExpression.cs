using Odachi.Gettext.Internal.Expressions;
using Odachi.Gettext.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Gettext
{
	public class PluralFormsExpression
	{
		private PluralFormsExpression(string forms, int count, Func<long, int> func)
		{
			if (forms == null)
				throw new ArgumentNullException(nameof(forms));
			if (func == null)
				throw new ArgumentNullException(nameof(func));

			Forms = forms;
			Count = count;
			_func = func;
		}

		private Func<long, int> _func;

		public string Forms { get; private set; }

		public int Count { get; private set; }

		public int Get(long n)
		{
			return _func(n);
		}

		#region Static members

		public static readonly PluralFormsExpression Default = new PluralFormsExpression("nplurals=2; plural=n != 1;", 2, n => n != 1 ? 1 : 0);

		private static readonly char[] PAIR_SEPARATORS = new[] { ';' };
		private static readonly char[] KEY_VALUE_SEPARATORS = new[] { '=' };

		public static PluralFormsExpression Parse(string pluralForms)
		{
			if (pluralForms == null)
				throw new ArgumentNullException(nameof(pluralForms));

			var nplurals = default(int?);
			var plural = default(string);
			foreach (var pair in pluralForms.Split(PAIR_SEPARATORS, StringSplitOptions.RemoveEmptyEntries))
			{
				var parts = pair.Split(KEY_VALUE_SEPARATORS, 2);
				if (parts.Length != 2)
					throw new FormatException("Failed to parse plural forms");

				switch (parts[0].Trim())
				{
					case "nplurals":
						int tmp;
						if (int.TryParse(parts[1], out tmp) && tmp >= 0)
							nplurals = tmp;
						break;

					case "plural":
						plural = parts[1];
						break;
				}
			}

			if (nplurals == null)
				throw new FormatException("Failed to parse plural forms (missing nplurals)");
			if (plural == null)
				throw new FormatException("Failed to parse plural forms (missing plural)");

			var n = Expression.Parameter(typeof(long), "n");

			var parser = new CExpressionParser();
			var expression = parser.Parse(plural, n);
			if (expression.Type == typeof(bool))
				expression = Expression.Condition(expression, Expression.Constant(1, typeof(int)), Expression.Constant(0, typeof(int)));
			if (expression.Type != typeof(int))
				expression = Expression.ConvertChecked(expression, typeof(int));
			var func = Expression.Lambda<Func<long, int>>(expression, n)
				.Compile();

			return new PluralFormsExpression(pluralForms, nplurals.Value, func);
		}

		#endregion
	}
}
