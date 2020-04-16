using System.Threading.Tasks;
using Dapper;
using Devart.Data.Oracle;

namespace Odachi.Testing.Scenarios.Data.Oracle
{
	public class OracleDatabaseInstance : DatabaseInstance
	{
		public OracleDatabaseInstance(string dbaConnectionString, string connectionString)
			: base(dbaConnectionString, connectionString)
		{
			_connectionString = new OracleConnectionStringBuilder(connectionString);
		}

		private OracleConnectionStringBuilder _connectionString;

		public override async Task CreateAsync()
		{
			using (var connection = new OracleConnection(DbaConnectionString))
			{
				await connection.OpenAsync();

				var varUsername = $"\"{_connectionString.UserId}\"";
				var varPassword = $"\"{_connectionString.Password}\"";

				await connection.ExecuteAsync($"CREATE USER {varUsername} IDENTIFIED BY {varPassword}");
				await connection.ExecuteAsync($@"GRANT ""CONNECT"" TO {varUsername}");
				await connection.ExecuteAsync($@"GRANT ""RESOURCE"" TO {varUsername}");
				await connection.ExecuteAsync($"GRANT CREATE SESSION TO {varUsername}");
				await connection.ExecuteAsync($"GRANT UNLIMITED TABLESPACE TO {varUsername}");

				await connection.CloseAsync();
			}
		}

		public override async Task DropAsync()
		{
			using (var connection = new OracleConnection(ConnectionString))
			{
				OracleConnection.ClearPool(connection);
			}

			// TODO: better way to force delete database?
			await Task.Delay(250);

			using (var connection = new OracleConnection(DbaConnectionString))
			{
				await connection.OpenAsync();

				var varUsername = $"\"{_connectionString.UserId}\"";

				await connection.ExecuteAsync($"drop user {varUsername} CASCADE");

				await connection.CloseAsync();
			}
		}
	}
}
