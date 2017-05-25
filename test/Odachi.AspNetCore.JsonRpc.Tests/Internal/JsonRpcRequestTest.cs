using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Odachi.AspNetCore.JsonRpc.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;
using Odachi.Extensions.Collections;
using Odachi.Abstractions;

namespace Odachi.AspNetCore.JsonRpc.Internal
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

			var result = (string)request.GetParameter("stringProperty", JsonMappedType.FromType(typeof(string)), null);

			Assert.Equal("test", "test");
		}

		[Fact]
		public void Can_deserialize_int_from_named_params()
		{
			var request = new JsonRpcRequest("1", "TestMethod", new JObject(
				new JProperty("intProperty", 55)
			), GetSerializer());

			var result = (int)request.GetParameter("intProperty", JsonMappedType.FromType(typeof(int)), 0);

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

			var result = (PagingOptions)request.GetParameter("pagingOptions", JsonMappedType.FromType(typeof(PagingOptions)), null);

			Assert.Equal(result.Number, 1);
			Assert.Equal(result.Size, 5);
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

			var result = (Page<string>)request.GetParameter("page", JsonMappedType.FromType(typeof(Page<string>)), null);

			Assert.Equal(result, new[] { "value1", "value2" });
			Assert.Equal(result.Number, 1);
			Assert.Equal(result.Size, 5);
			Assert.Equal(result.Total, 10);
			Assert.Equal(result.Overflow, false);
		}

		[Fact]
		public void Can_deserialize_entity_reference_from_named_params()
		{
			var request = new JsonRpcRequest("1", "TestMethod", new JObject(
				new JProperty("ref", new JObject(
					new JProperty("id", 12345)
				))
			), GetSerializer());

			var result = (IEntityReference)request.GetParameter("ref", JsonMappedType.FromType(typeof(IEntityReference)), null);

			Assert.NotNull(result);
			Assert.Equal(result.Id, 12345);
		}
	}
}
