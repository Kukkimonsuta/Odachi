using Dapper;
using MySqlConnector;
using System.Threading.Tasks;

namespace Odachi.Testing.Scenarios.Data.MySql
{
	public class MySqlDatabaseInstance : DatabaseInstance
	{
		public MySqlDatabaseInstance(string dbaConnectionString, string connectionString)
			: base(dbaConnectionString, connectionString)
		{
			_connectionString = new MySqlConnectionStringBuilder(connectionString);
		}

		private MySqlConnectionStringBuilder _connectionString;

		public override async Task CreateAsync()
		{
			using (var connection = new MySqlConnection(DbaConnectionString))
			{
				await connection.OpenAsync();

				var varDatabase = $"`{_connectionString.Database}`";
				
				await connection.ExecuteAsync($"CREATE DATABASE {varDatabase}");

				await connection.CloseAsync();
			}
		}

		public override async Task DropAsync()
		{
			using (var connection = new MySqlConnection(ConnectionString))
			{
				MySqlConnection.ClearPool(connection);
			}

			using (var connection = new MySqlConnection(DbaConnectionString))
			{
				await connection.OpenAsync();

				var varDatabase = $"`{_connectionString.Database}`";

				await connection.ExecuteAsync($"DROP DATABASE {varDatabase}");

				await connection.CloseAsync();
			}
		}
	}
}
