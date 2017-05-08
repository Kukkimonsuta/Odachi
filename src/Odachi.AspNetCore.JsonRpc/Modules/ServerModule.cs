using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.AspNetCore.JsonRpc.Internal;
using Odachi.Annotations;

namespace Odachi.AspNetCore.JsonRpc.Modules
{
	public class ServerModule
	{
		[RpcMethod]
		public Task<string[]> ListMethodsAsync(JsonRpcServer server)
		{
			var methods = server.Methods
				.Select(m => m.Name)
				.ToArray();

			return Task.FromResult(methods);
		}
	}
}
