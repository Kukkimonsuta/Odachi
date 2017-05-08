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
		public static string[] ListMethods(JsonRpcServer server)
		{
			var methods = server.Methods
				.Select(m => m.Name)
				.ToArray();

			return methods;
		}

		[RpcMethod]
		public static DateTime Ping()
		{
			return DateTime.Now;
		}

		[RpcMethod]
		public static Task<object[]> DescribeAsync(JsonRpcServer server, bool includeInternals = false)
		{
			var modules = server.Methods
				.GroupBy(m => m.ModuleName, (k, g) => new
				{
					Name = k,
					Methods = g.Select(m => new
					{
						Name = m.MethodName,
						ReturnType = m.ReturnType.JsonType.ToString().ToLowerInvariant(),
						Parameters = m.Parameters.Where(p => includeInternals || !p.IsInternal).Select(p => new
						{
							Name = p.Name,
							Type = p.Type.JsonType.ToString().ToLowerInvariant(),
							IsOptional = p.IsOptional,
							DefaultValue = p.DefaultValue,
						}),
					}),
				})
				.Cast<object>()
				.ToArray();

			return Task.FromResult(modules);
		}
	}
}
