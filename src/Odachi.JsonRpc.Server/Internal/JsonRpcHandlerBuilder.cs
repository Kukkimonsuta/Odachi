using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.JsonRpc.Common;
using Odachi.JsonRpc.Server.Builder;

namespace Odachi.JsonRpc.Server.Internal
{
	public class JsonRpcHandlerBuilder : IJsonRpcHandlerBuilder
	{
		private readonly IList<Func<JsonRpcRequestDelegate, JsonRpcRequestDelegate>> _components = new List<Func<JsonRpcRequestDelegate, JsonRpcRequestDelegate>>();

		public IJsonRpcHandlerBuilder Use(Func<JsonRpcRequestDelegate, JsonRpcRequestDelegate> middleware)
		{
			_components.Add(middleware);
			return this;
		}

		public JsonRpcRequestDelegate Build()
		{
			JsonRpcRequestDelegate result = async (context) =>
			{
				if (context.Method == null)
				{
					context.SetResponse(JsonRpcError.METHOD_NOT_FOUND);
					return;
				}

				await context.Method.InvokeAsync(context);
			};

			foreach (var component in _components.Reverse())
			{
				result = component(result);
			}

			return result;
		}
	}
}
