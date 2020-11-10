using MySqlConnector;
using System;
using System.Threading.Tasks;

namespace Odachi.Testing.Scenarios.Data.MySql
{
	public class MySqlDatabaseInstanceProvider : DatabaseInstanceProvider
	{
		public MySqlDatabaseInstanceProvider(string baseConnectionString)
		{
			DbaConnectionString = baseConnectionString ?? throw new ArgumentNullException(nameof(baseConnectionString));
		}

		public string DbaConnectionString { get; }

		public override async Task<DatabaseInstance> GetInstanceAsync(string name)
		{
			var connectionString = new MySqlConnectionStringBuilder(DbaConnectionString)
			{
				Database = name
			};

			var instance = new MySqlDatabaseInstance(DbaConnectionString, connectionString.ConnectionString);

			await instance.CreateAsync();

			return instance;
		}
	}
}
