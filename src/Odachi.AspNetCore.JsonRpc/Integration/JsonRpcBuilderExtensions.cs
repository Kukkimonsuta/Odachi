using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Options;
using Odachi.AspNetCore.JsonRpc;

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

			services.Configure(configureOptions);
			services.AddJsonRpc();

			return services;
		}
	}
}
