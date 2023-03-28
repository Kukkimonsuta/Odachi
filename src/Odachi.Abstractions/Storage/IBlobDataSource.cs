using System.IO;
using System.Threading.Tasks;

namespace Odachi.Abstractions;

/// <summary>
/// Binary data source.
/// </summary>
public interface IBlobDataSource
{
	/// <summary>
	/// Returns whether it is preferred to use async. Not respecting this value can lead to degraded performance.
	/// </summary>
	bool PreferAsync { get; }

	/// <summary>
	/// Write data to given stream.
	/// </summary>
	void WriteTo(Stream stream);

	/// <summary>
	/// Write data to given stream.
	/// </summary>
	Task WriteToAsync(Stream stream);
}
