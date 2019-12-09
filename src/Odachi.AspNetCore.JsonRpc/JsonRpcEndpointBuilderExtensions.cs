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
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace Microsoft.AspNetCore.Builder
{
	public static class JsonRpcEndpointBuilderExtensions
	{
		public static IEndpointConventionBuilder MapJsonRpc(this IEndpointRouteBuilder endpoints, string pattern)
		{
			if (endpoints == null)
			{
				throw new ArgumentNullException(nameof(endpoints));
			}

			return MapJsonRpc(endpoints, RoutePatternFactory.Parse(pattern));
		}
		public static IEndpointConventionBuilder MapJsonRpc(this IEndpointRouteBuilder endpoints, RoutePattern pattern)
		{
			if (endpoints == null)
			{
				throw new ArgumentNullException(nameof(endpoints));
			}

			var pipeline = endpoints.CreateApplicationBuilder()
				.UseJsonRpc()
				.Build();

			return endpoints.Map(pattern, pipeline);
		}

		public static IEndpointConventionBuilder MapJsonRpc(this IEndpointRouteBuilder endpoints, string pattern, JsonRpcOptions options)
		{
			if (endpoints == null)
			{
				throw new ArgumentNullException(nameof(endpoints));
			}

			return MapJsonRpc(endpoints, RoutePatternFactory.Parse(pattern), options);
		}
		public static IEndpointConventionBuilder MapJsonRpc(this IEndpointRouteBuilder endpoints, RoutePattern pattern, JsonRpcOptions options)
		{
			if (endpoints == null)
			{
				throw new ArgumentNullException(nameof(endpoints));
			}

			var pipeline = endpoints.CreateApplicationBuilder()
				.UseJsonRpc(options)
				.Build();

			return endpoints.Map(pattern, pipeline);
		}
	}
}
