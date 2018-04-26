using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Odachi.AspNetCore.JsonRpc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Odachi.JsonRpc.Server;

namespace Microsoft.AspNetCore.Builder
{
    public static class JsonRpcApplicationBuilderExtensions
    {
		public static IApplicationBuilder UseJsonRpc(this IApplicationBuilder builder)
		{
			var httpContextAccessor = builder.ApplicationServices.GetRequiredService<IHttpContextAccessor>();

			return builder.UseMiddleware<JsonRpcMiddleware>(httpContextAccessor);
		}

		public static IApplicationBuilder UseJsonRpc(this IApplicationBuilder builder, JsonRpcOptions options)
		{
			var httpContextAccessor = builder.ApplicationServices.GetRequiredService<IHttpContextAccessor>();

			return builder.UseMiddleware<JsonRpcMiddleware>(httpContextAccessor, Options.Create(options));
		}
	}
}
