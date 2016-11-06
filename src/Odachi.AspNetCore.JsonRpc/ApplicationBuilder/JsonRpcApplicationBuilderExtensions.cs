using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Odachi.AspNetCore.JsonRpc;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class JsonRpcApplicationBuilderExtensions
    {
		public static IApplicationBuilder UseJsonRpc(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<JsonRpcMiddleware>();
		}

		public static IApplicationBuilder UseJsonRpc(this IApplicationBuilder builder, JsonRpcOptions options)
		{
			return builder.UseMiddleware<JsonRpcMiddleware>(Options.Create(options));
		}
	}
}
