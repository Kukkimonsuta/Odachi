using System.Threading.Tasks;

namespace Odachi.Testing.Scenarios.Data
{
	public abstract class Dataset
	{
		/// <summary>
		/// Version of dataset. This value is used to determinate whether cached data are still valid.
		/// </summary>
		public abstract string Version { get; }

		/// <summary>
		/// Called when initializing new database instance to insert initial data.
		/// </summary>
		public abstract Task InstallAsync();
	}
}
