using Odachi.Localization.Extraction.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Odachi.Localization.Extraction.Infrastructure
{
	public class KeywordTest
	{
		[Theory]
		[InlineData("GetString")]
		[InlineData("gettext")]
		public void Can_parse_format_id(string data)
		{
			var keyword = Keyword.Parse(data);

			Assert.NotNull(keyword);
			Assert.Equal(data, keyword.Id);
			Assert.Equal(1, keyword.Singular);
			Assert.Equal(null, keyword.Plural);
			Assert.Equal(null, keyword.Context);
		}

		[Theory]
		[InlineData("GetString:1", "GetString", 1)]
		[InlineData("gettext:10", "gettext", 10)]
		public void Can_parse_format_id_singular(string data, string id, int singular)
		{
			var keyword = Keyword.Parse(data);

			Assert.NotNull(keyword);
			Assert.Equal(id, keyword.Id);
			Assert.Equal(singular, keyword.Singular);
			Assert.Equal(null, keyword.Plural);
			Assert.Equal(null, keyword.Context);
		}

		[Theory]
		[InlineData("GetString:1c,2", "GetString", 1, 2)]
		[InlineData("gettext:10c,11", "gettext", 10, 11)]
		[InlineData("GetString:2,1c", "GetString", 1, 2)]
		[InlineData("gettext:11,10c", "gettext", 10, 11)]
		public void Can_parse_format_id_context_singular(string data, string id, int context, int singular)
		{
			var keyword = Keyword.Parse(data);

			Assert.NotNull(keyword);
			Assert.Equal(id, keyword.Id);
			Assert.Equal(singular, keyword.Singular);
			Assert.Equal(null, keyword.Plural);
			Assert.Equal(context, keyword.Context);
		}

		[Theory]
		[InlineData("GetString:1c,2,3", "GetString", 1, 2, 3)]
		[InlineData("gettext:10c,11,12", "gettext", 10, 11, 12)]
		[InlineData("GetString:2,1c,3", "GetString", 1, 2, 3)]
		[InlineData("gettext:11,10c,12", "gettext", 10, 11, 12)]
		[InlineData("GetString:2,3,1c", "GetString", 1, 2, 3)]
		[InlineData("gettext:11,12,10c", "gettext", 10, 11, 12)]
		public void Can_parse_format_id_context_singular_plural(string data, string id, int context, int singular, int plural)
		{
			var keyword = Keyword.Parse(data);

			Assert.NotNull(keyword);
			Assert.Equal(id, keyword.Id);
			Assert.Equal(singular, keyword.Singular);
			Assert.Equal(plural, keyword.Plural);
			Assert.Equal(context, keyword.Context);
		}
	}
}
