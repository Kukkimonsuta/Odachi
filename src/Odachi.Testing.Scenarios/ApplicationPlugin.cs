using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Odachi.Testing.Scenarios
{
	public abstract class ApplicationPlugin
	{
		public virtual Task ConfigureAsync() => Task.CompletedTask;
		public virtual Task ConfigureServicesAsync(ServiceCollection services) => Task.CompletedTask;
		public virtual Task StartupAsync(ApplicationInstance instance) => Task.CompletedTask;
		public virtual Task ShutdownAsync(ApplicationInstance instance) => Task.CompletedTask;

		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual string? GetDebuggerDisplay(ApplicationScope scope)
		{
			return null;
		}
	}
}
