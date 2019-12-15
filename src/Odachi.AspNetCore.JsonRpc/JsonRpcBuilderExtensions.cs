using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Odachi.JsonRpc.Server;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class JsonRpcServiceCollectionExtensions
	{
		public static IServiceCollection AddJsonRpc(this IServiceCollection services)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));

			return services;
		}

		public static IServiceCollection AddJsonRpc(this IServiceCollection services, Action<JsonRpcOptions> configureOptions)
		{
			if (services == null)
				throw new ArgumentNullException(nameof(services));
			if (configureOptions == null)
				throw new ArgumentNullException(nameof(configureOptions));

			// HttpContextAccessor is required due to the way we handle files inside `StreamReferenceConverter`
			services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.Configure(configureOptions);
			services.AddJsonRpc();

			return services;
		}
	}
}
