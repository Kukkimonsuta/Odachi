using Npgsql;
using System;
using System.Threading.Tasks;

namespace Odachi.Testing.Scenarios.Data.PostgreSQL
{
	public class PostgreSQLDatabaseInstanceProvider : DatabaseInstanceProvider
	{
		public PostgreSQLDatabaseInstanceProvider(string baseConnectionString)
		{
			DbaConnectionString = baseConnectionString ?? throw new ArgumentNullException(nameof(baseConnectionString));
		}

		public string DbaConnectionString { get; }

		public override async Task<DatabaseInstance> GetInstanceAsync(string name)
		{
			var connectionString = new NpgsqlConnectionStringBuilder(DbaConnectionString)
			{
				Database = name,
			};
		
			var instance = new PostgreSQLDatabaseInstance(DbaConnectionString, connectionString.ConnectionString);

			await instance.CreateAsync();

			return instance;
		}
	}
}
