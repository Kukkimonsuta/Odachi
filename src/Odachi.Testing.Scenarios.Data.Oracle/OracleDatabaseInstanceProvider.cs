using System;
using System.Threading.Tasks;
using Devart.Data.Oracle;

namespace Odachi.Testing.Scenarios.Data.Oracle
{
	public class OracleDatabaseInstanceProvider : DatabaseInstanceProvider
	{
		public OracleDatabaseInstanceProvider(string dbaConnectionString)
		{
			DbaConnectionString = dbaConnectionString ?? throw new ArgumentNullException(nameof(dbaConnectionString));
		}

		public string DbaConnectionString { get; }

		public override async Task<DatabaseInstance> GetInstanceAsync(string name)
		{
			var connectionString = new OracleConnectionStringBuilder(DbaConnectionString)
			{
				UserId = name.ToUpperInvariant(),
				Password = "SCENARIOTESTS"
			};

			var instance = new OracleDatabaseInstance(DbaConnectionString, connectionString.ConnectionString);

			await instance.CreateAsync();

			return instance;
		}
	}
}
