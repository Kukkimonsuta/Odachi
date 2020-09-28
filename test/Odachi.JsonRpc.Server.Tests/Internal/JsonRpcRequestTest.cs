using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Odachi.Extensions.Collections;
using Odachi.Abstractions;
using Odachi.JsonRpc.Server;

namespace Odachi.JsonRpc.Server.Internal
{
	public class JsonRpcRequestTest
	{
		private JsonSerializer GetSerializer()
		{
			var options = new JsonRpcOptions();

			return JsonSerializer.Create(options.JsonSerializerSettings);
		}

		[Fact]
		public void Can_deserialize_string_from_named_params()
		{
			var request = new JsonRpcRequest("1", "TestMethod", new JObject(
				new JProperty("stringProperty", "test")
			), GetSerializer());

			var result = (string)request.GetParameter("stringProperty", typeof(string), null);

			Assert.Equal("test", "test");
		}

		[Fact]
		public void Can_deserialize_int_from_named_params()
		{
			var request = new JsonRpcRequest("1", "TestMethod", new JObject(
				new JProperty("intProperty", 55)
			), GetSerializer());

			var result = (int)request.GetParameter("intProperty", typeof(int), 0);

			Assert.Equal(55, 55);
		}

		[Fact]
		public void Can_deserialize_paging_options_from_named_params()
		{
			var request = new JsonRpcRequest("1", "TestMethod", new JObject(
				new JProperty("pagingOptions", new JObject(
					new JProperty("number", 1),
					new JProperty("size", 5)
				))
			), GetSerializer());

			var result = (PagingOptions)request.GetParameter("pagingOptions", typeof(PagingOptions), null);

			Assert.Equal(1, result.Number);
			Assert.Equal(5, result.Size);
		}

		[Fact]
		public void Can_deserialize_page_from_named_params()
		{
			var request = new JsonRpcRequest("1", "TestMethod", new JObject(
				new JProperty("page", new JObject(
					new JProperty("data", new JArray(new[]
					{
						"value1",
						"value2",
					})),
					new JProperty("number", 1),
					new JProperty("size", 5),
					new JProperty("total", 10)
				))
			), GetSerializer());

			var result = (Page<string>)request.GetParameter("page", typeof(Page<string>), null);

			Assert.Equal(result, new[] { "value1", "value2" });
			Assert.Equal(1, result.Number);
			Assert.Equal(5, result.Size);
			Assert.Equal(10, result.Total);
			Assert.False(result.Overflow);
		}
	}
}
