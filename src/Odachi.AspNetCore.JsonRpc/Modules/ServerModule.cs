using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.AspNetCore.JsonRpc.Internal;

namespace Odachi.AspNetCore.JsonRpc.Modules
{
	public class ServerModule
	{
		public class ListMethods : JsonRpcMethod
		{
			public ListMethods()
				: base("Server.listMethods")
			{
			}

			public override Task HandleAsync(JsonRpcContext context)
			{
				var methods = context.Server.Methods
					.Select(m => m.Name)
					.ToArray();

				context.SetResponse(methods);

				return Task.WhenAll();
			}
		}
	}
}
