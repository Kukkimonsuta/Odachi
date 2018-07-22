using System;
using System.IO;

namespace Odachi.Abstractions
{
	/// <summary>
	/// Represents a reference to a stream.
	/// </summary>
	[Obsolete("Use `Odachi.Abstractions.IBlob`. Will be removed in next major version.")]
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
