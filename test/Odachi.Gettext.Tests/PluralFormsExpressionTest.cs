using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Odachi.Gettext
{
	public class PluralFormsTest
	{
		[Fact]
		public void Can_parse_czech()
		{
			var format = "nplurals=3; plural=(n==1) ? 0 : (n>=2 && n<=4) ? 1 : 2;";

			var expression = PluralFormsExpression.Parse(format);

			Assert.Equal(3, expression.Count);

			Assert.Equal(0, expression.Get(1));

			Assert.Equal(1, expression.Get(2));
			Assert.Equal(1, expression.Get(3));
			Assert.Equal(1, expression.Get(4));

			Assert.Equal(2, expression.Get(0));
			Assert.Equal(2, expression.Get(5));
			Assert.Equal(2, expression.Get(6));
			Assert.Equal(2, expression.Get(7));
			Assert.Equal(2, expression.Get(8));
			Assert.Equal(2, expression.Get(9));
			Assert.Equal(2, expression.Get(10));
			Assert.Equal(2, expression.Get(11));
			Assert.Equal(2, expression.Get(12));
			Assert.Equal(2, expression.Get(13));
			Assert.Equal(2, expression.Get(14));
			Assert.Equal(2, expression.Get(15));
		}

		[Fact]
		public void Can_parse_slovenian()
		{
			var format = @"nplurals=4; plural=n%100==1 ? 0 : n%100==2 ? 1 : n%100==3 || n%100==4 ? 2 : 3;";

			var expression = PluralFormsExpression.Parse(format);

			Assert.Equal(4, expression.Count);

			Assert.Equal(0, expression.Get(1));
			Assert.Equal(0, expression.Get(101));
			Assert.Equal(0, expression.Get(201));
			Assert.Equal(0, expression.Get(301));
			Assert.Equal(0, expression.Get(1001));

			Assert.Equal(1, expression.Get(2));
			Assert.Equal(1, expression.Get(102));
			Assert.Equal(1, expression.Get(202));
			Assert.Equal(1, expression.Get(302));
			Assert.Equal(1, expression.Get(1002));

			Assert.Equal(2, expression.Get(3));
			Assert.Equal(2, expression.Get(103));
			Assert.Equal(2, expression.Get(203));
			Assert.Equal(2, expression.Get(303));
			Assert.Equal(2, expression.Get(1003));
			Assert.Equal(2, expression.Get(4));
			Assert.Equal(2, expression.Get(104));
			Assert.Equal(2, expression.Get(204));
			Assert.Equal(2, expression.Get(304));
			Assert.Equal(2, expression.Get(1004));

			Assert.Equal(3, expression.Get(0));
			Assert.Equal(3, expression.Get(5));
			Assert.Equal(3, expression.Get(6));
			Assert.Equal(3, expression.Get(7));
			Assert.Equal(3, expression.Get(8));
			Assert.Equal(3, expression.Get(9));
			Assert.Equal(3, expression.Get(10));
			Assert.Equal(3, expression.Get(11));
			Assert.Equal(3, expression.Get(12));
			Assert.Equal(3, expression.Get(13));
			Assert.Equal(3, expression.Get(14));
			Assert.Equal(3, expression.Get(15));
			Assert.Equal(3, expression.Get(16));
			Assert.Equal(3, expression.Get(105));
			Assert.Equal(3, expression.Get(205));
			Assert.Equal(3, expression.Get(305));
			Assert.Equal(3, expression.Get(1005));
		}

		[Fact]
		public void Can_parse_english()
		{
			var format = "nplurals=2; plural=n != 1;";

			var expression = PluralFormsExpression.Parse(format);

			Assert.Equal(2, expression.Count);

			Assert.Equal(0, expression.Get(1));

			Assert.Equal(1, expression.Get(0));
			Assert.Equal(1, expression.Get(2));
			Assert.Equal(1, expression.Get(3));
			Assert.Equal(1, expression.Get(4));
			Assert.Equal(1, expression.Get(5));
			Assert.Equal(1, expression.Get(6));
			Assert.Equal(1, expression.Get(7));
			Assert.Equal(1, expression.Get(8));
			Assert.Equal(1, expression.Get(9));
			Assert.Equal(1, expression.Get(10));
			Assert.Equal(1, expression.Get(11));
			Assert.Equal(1, expression.Get(12));
			Assert.Equal(1, expression.Get(13));
			Assert.Equal(1, expression.Get(14));
			Assert.Equal(1, expression.Get(15));
		}
	}
}
