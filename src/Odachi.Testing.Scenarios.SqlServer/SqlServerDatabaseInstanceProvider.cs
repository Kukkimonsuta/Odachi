using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace Odachi.Testing.Scenarios.Data.SqlServer
{
	public class SqlServerDatabaseInstanceProvider : DatabaseInstanceProvider
	{
		public SqlServerDatabaseInstanceProvider(string baseConnectionString)
		{
			DbaConnectionString = baseConnectionString ?? throw new ArgumentNullException(nameof(baseConnectionString));
		}

		public string DbaConnectionString { get; }

		public override async Task<DatabaseInstance> GetInstanceAsync(string name)
		{
			var connectionString = new SqlConnectionStringBuilder(DbaConnectionString)
			{
				InitialCatalog = name
			};
		
			var instance = new SqlServerDatabaseInstance(DbaConnectionString, connectionString.ConnectionString);

			await instance.CreateAsync();

			return instance;
		}
	}
}
