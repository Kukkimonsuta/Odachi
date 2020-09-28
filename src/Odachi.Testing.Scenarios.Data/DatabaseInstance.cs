using System;
using System.Threading.Tasks;

namespace Odachi.Testing.Scenarios.Data
{
	public abstract class DatabaseInstance
	{
		public DatabaseInstance(
			string dbaConnectionString,
			string connectionString
		)
		{
			DbaConnectionString = dbaConnectionString ?? throw new ArgumentNullException(nameof(dbaConnectionString));
			ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
		}

		public string DbaConnectionString { get; }
		public string ConnectionString { get; }

		public abstract Task CreateAsync();

		// todo: backup
		// todo: restore

		public abstract Task DropAsync();
	}
}
