using System.IO;

namespace Odachi.Abstractions
{
	/// <summary>
	/// Represents a reference to a stream.
	/// </summary>
	public interface IStreamReference
	{
		/// <summary>
		/// Name.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Opens stream for reading.
		/// </summary>
		Stream OpenReadStream();
	}
}
