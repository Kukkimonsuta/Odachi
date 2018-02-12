using System.IO;
using System.Threading.Tasks;

namespace Odachi.Abstractions
{
	/// <summary>
	/// Stored named binary data.
	/// </summary>
	public interface IStoredBlob : IBlob
	{
		/// <summary>
		/// Size.
		/// </summary>
		long Length { get; }

		/// <summary>
		/// Blob storage location.
		/// </summary>
		IBlobStorage Location { get; }
	}
}
