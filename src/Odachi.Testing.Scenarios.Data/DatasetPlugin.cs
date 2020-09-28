using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Odachi.Testing.Scenarios;
using Odachi.Testing.Scenarios.Data;

namespace Odachi.Testing.Scenarios.Data
{
	public class DatasetPlugin<TDataset> : ApplicationPlugin
		where TDataset : Dataset
	{
		public override Task ConfigureServicesAsync(ServiceCollection services)
		{
			services.AddSingleton<TDataset>();

			return Task.CompletedTask;
		}

		public override async Task StartupAsync(ApplicationInstance instance)
		{
			using (var scope = await instance.CreateScopeAsync())
			{
				var dataset = scope.Scope.ServiceProvider.GetRequiredService<TDataset>();

				await dataset.InstallAsync();
			}
		}

		public override string GetDebuggerDisplay(ApplicationScope scope)
		{
			return $"dataset:{typeof(TDataset).Name}";
		}
	}
}
