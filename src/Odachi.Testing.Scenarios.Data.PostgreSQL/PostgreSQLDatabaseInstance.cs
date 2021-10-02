using Dapper;
using System.Threading.Tasks;
using Npgsql;

namespace Odachi.Testing.Scenarios.Data.PostgreSQL
{
	public class PostgreSQLDatabaseInstance : DatabaseInstance
	{
		public PostgreSQLDatabaseInstance(string dbaConnectionString, string connectionString)
			: base(dbaConnectionString, connectionString)
		{
			_connectionString = new NpgsqlConnectionStringBuilder(connectionString);
		}

		private NpgsqlConnectionStringBuilder _connectionString;

		public override async Task CreateAsync()
		{
			using (var connection = new NpgsqlConnection(DbaConnectionString))
			{
				await connection.OpenAsync();

				var varDatabase = $"\"{_connectionString.Database}\"";
				
				await connection.ExecuteAsync($"CREATE DATABASE {varDatabase}");

				await connection.CloseAsync();
			}
		}

		public override async Task DropAsync()
		{
			using (var connection = new NpgsqlConnection(ConnectionString))
			{
				NpgsqlConnection.ClearPool(connection);
			}
			
			using (var connection = new NpgsqlConnection(DbaConnectionString))
			{
				await connection.OpenAsync();

				var varDatabase = $"[{_connectionString.Database}]";

				await connection.ExecuteAsync($"DROP DATABASE {varDatabase}");

				await connection.CloseAsync();
			}
		}
	}
}
