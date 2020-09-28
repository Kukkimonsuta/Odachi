using System;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Odachi.Testing.Scenarios
{
	public sealed class ApplicationInstance : IAsyncDisposable
	{
		internal ApplicationInstance(Application application, ServiceProvider serviceProvider, Func<ApplicationInstance, Task>? disposer = null)
		{
			Application = application ?? throw new ArgumentNullException(nameof(application));
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			_disposer = disposer;
		}

		private ServiceProvider _serviceProvider;
		private Func<ApplicationInstance, Task>? _disposer;

		public Application Application { get; }

		internal Task<IServiceScope> CreateScopeInternalAsync()
		{
			return Task.FromResult(_serviceProvider.CreateScope());
		}

		public async Task<ApplicationScope> CreateScopeAsync(IPrincipal? principal = null)
		{
			var scope = await CreateScopeInternalAsync();

			return new ApplicationScope(this, scope, principal: principal);
		}

		#region IAsyncDisposable

		private bool _disposed = false;

		public async ValueTask DisposeAsync()
		{
			if (_disposed)
			{
				return;
			}

			if (_disposer != null)
			{
				await _disposer.Invoke(this);
			}
			await _serviceProvider.DisposeAsync();

			_disposed = true;
		}

		#endregion
	}
}
