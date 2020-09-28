using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Odachi.Testing.Scenarios;
using Odachi.Testing.Scenarios.Data;

namespace Odachi.Testing.Scenarios.Data
{
	// current implementation is simply for debugger display
	public class DatabasePlugin : ApplicationPlugin
	{
		public DatabasePlugin(string connectionString)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
		}

		private string _connectionString;


		public override string GetDebuggerDisplay(ApplicationScope scope)
		{
			return $"database:{_connectionString}";
		}
	}
}
