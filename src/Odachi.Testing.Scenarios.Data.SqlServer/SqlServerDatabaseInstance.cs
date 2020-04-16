using Dapper;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace Odachi.Testing.Scenarios.Data.SqlServer
{
	public class SqlServerDatabaseInstance : DatabaseInstance
	{
		public SqlServerDatabaseInstance(string dbaConnectionString, string connectionString)
			: base(dbaConnectionString, connectionString)
		{
			_connectionString = new SqlConnectionStringBuilder(connectionString);
		}

		private SqlConnectionStringBuilder _connectionString;

		public override async Task CreateAsync()
		{
			using (var connection = new SqlConnection(DbaConnectionString))
			{
				await connection.OpenAsync();

				var varDatabase = $"[{_connectionString.InitialCatalog}]";
				
				await connection.ExecuteAsync($"CREATE DATABASE {varDatabase}");

				await connection.CloseAsync();
			}
		}

		public override async Task DropAsync()
		{
			using (var connection = new SqlConnection(ConnectionString))
			{
				SqlConnection.ClearPool(connection);
			}
			
			using (var connection = new SqlConnection(DbaConnectionString))
			{
				await connection.OpenAsync();

				var varDatabase = $"[{_connectionString.InitialCatalog}]";

				await connection.ExecuteAsync($"DROP DATABASE {varDatabase}");

				await connection.CloseAsync();
			}
		}
	}
}
