using System.IO;
using System.Threading.Tasks;

namespace Odachi.Abstractions
{
	/// <summary>
	/// Named binary data.
	/// </summary>
	public interface IBlob
	{
		/// <summary>
		/// Name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Returns whether it is preffered to use async. Not respecting this value can lead to degraded perfromance.
		/// </summary>
		bool PreferAsync { get; }

		/// <summary>
		/// Opens stream for reading.
		/// </summary>
		Stream OpenRead();

		/// <summary>
		/// Opens stream for reading.
		/// </summary>
		Task<Stream> OpenReadAsync();
	}
}
