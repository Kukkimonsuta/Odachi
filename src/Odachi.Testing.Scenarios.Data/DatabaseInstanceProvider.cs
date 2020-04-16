using System.Threading.Tasks;

namespace Odachi.Testing.Scenarios.Data
{
	public abstract class DatabaseInstanceProvider
	{
		public DatabaseInstanceProvider()
		{
		}

		public abstract Task<DatabaseInstance> GetInstanceAsync(string name);
	}
}
