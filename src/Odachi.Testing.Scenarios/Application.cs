using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Odachi.Testing.Scenarios
{
	public abstract class Application
	{
		public List<ApplicationPlugin> Plugins { get; } = new List<ApplicationPlugin>();

		public IPrincipal DefaultPrincipal { get; set; } = new ClaimsPrincipal();

		protected virtual Task ConfigureAsync() => Task.CompletedTask;

		protected virtual Task ConfigureServicesAsync(ServiceCollection services) => Task.CompletedTask;

		protected virtual Task StartupAsync(ApplicationInstance instance) => Task.CompletedTask;

		protected virtual Task ShutdownAsync(ApplicationInstance instance) => Task.CompletedTask;

		public async Task<ApplicationInstance> StartAsync()
		{
			await ConfigureAsync();
			for (var i = 0; i < Plugins.Count; i++)
			{
				await Plugins[i].ConfigureAsync();
			}

			var serviceCollection = new ServiceCollection();
			serviceCollection.AddScoped<ApplicationPrincipalHolder>();
			serviceCollection.AddTransient(x => x.GetRequiredService<ApplicationPrincipalHolder>().Principal);
			await ConfigureServicesAsync(serviceCollection);
			for (var i = 0; i < Plugins.Count; i++)
			{
				await Plugins[i].ConfigureServicesAsync(serviceCollection);
			}
			var serviceProvider = serviceCollection.BuildServiceProvider();

			var instance = new ApplicationInstance(this, serviceProvider, async instance =>
			{
				for (var i = Plugins.Count - 1; i >= 0; i--)
				{
					await Plugins[i].ShutdownAsync(instance);
				}

				await ShutdownAsync(instance);
			});

			await StartupAsync(instance);

			for (var i = 0; i < Plugins.Count; i++)
			{
				await Plugins[i].StartupAsync(instance);
			}

			return instance;
		}
	}
}
