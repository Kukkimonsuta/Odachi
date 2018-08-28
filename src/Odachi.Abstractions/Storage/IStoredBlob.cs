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
		/// Path to blob within storage (including name).
		/// </summary>
		string Path { get; }

		/// <summary>
		/// Size.
		/// </summary>
		long Length { get; }

		/// <summary>
		/// Owning blob storage.
		/// </summary>
		IBlobStorage Storage { get; }
	}
}
