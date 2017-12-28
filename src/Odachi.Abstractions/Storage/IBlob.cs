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
		/// Returns whether the preferred method of opening is asynchronous.
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
