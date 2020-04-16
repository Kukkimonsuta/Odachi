using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Odachi.Testing.Scenarios
{
	[DebuggerDisplay("{DebuggerDisplay,nq}")]
	public class ApplicationScope : IDisposable
	{
		internal ApplicationScope(ApplicationInstance instance, IServiceScope scope, IPrincipal? principal = null)
		{
			Instance = instance ?? throw new ArgumentNullException(nameof(instance));
			Scope = scope ?? throw new ArgumentNullException(nameof(scope));

			scope.ServiceProvider.GetRequiredService<ApplicationPrincipalHolder>().Principal = principal ?? instance.Application.DefaultPrincipal;
		}

		public ApplicationInstance Instance { get; }
		public IServiceScope Scope { get; }

		public IServiceProvider Services => Scope.ServiceProvider;

		#region Debugger display

		private string DebuggerDisplay => string.Join(
			" | ",
			new[] { "ApplicationScope" }.Concat(
				Instance.Application.Plugins
					.Select(p => p.GetDebuggerDisplay(this))
					.Where(p => p != null)
			)
		);

		#endregion

		#region IDisposable

		private bool _disposed = false;

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (Scope != null)
					{
						Scope.Dispose();
					}
				}

				_disposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		#endregion
	}
}
