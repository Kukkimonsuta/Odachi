using System.IO;
using System.Threading.Tasks;

namespace Odachi.Abstractions;

/// <summary>
/// Named binary data.
/// </summary>
public interface IBlob : IBlobDataSource
{
	/// <summary>
	/// Name.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Opens stream for reading.
	/// </summary>
	Stream OpenRead();

	/// <summary>
	/// Opens stream for reading.
	/// </summary>
	Task<Stream> OpenReadAsync();
}
