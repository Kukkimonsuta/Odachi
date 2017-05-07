using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Odachi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Odachi.AspNetCore.JsonRpc.Internal
{
	public class JsonRpcRequestTest
	{
		[Fact]
		public void Can_deserialize_string_from_named_params()
		{
			var request = new JsonRpcRequest("1", "TestMethod", new JObject(
				new JProperty("stringProperty", "test")
			), new JsonSerializer());

			var result = (string)request.GetParameter("stringProperty", typeof(string), null);

			Assert.Equal("test", "test");
		}

		[Fact]
		public void Can_deserialize_int_from_named_params()
		{
			var request = new JsonRpcRequest("1", "TestMethod", new JObject(
				new JProperty("intProperty", 55)
			), new JsonSerializer());

			var result = (int)request.GetParameter("intProperty", typeof(int), 0);

			Assert.Equal(55, 55);
		}

		[Fact]
		public void Can_deserialize_paging_options_from_named_params()
		{
			var request = new JsonRpcRequest("1", "TestMethod", new JObject(
				new JProperty("pagingOptions", new JObject(
					new JProperty("page", 1),
					new JProperty("pageSize", 5)
				))
			), new JsonSerializer());

			var result = (PagingOptions)request.GetParameter("pagingOptions", typeof(PagingOptions), null);

			Assert.Equal(result.Page, 1);
			Assert.Equal(result.PageSize, 5);
		}
	}
}
