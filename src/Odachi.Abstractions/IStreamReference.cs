using System.IO;

namespace Odachi.Abstractions
{
	public interface IStreamReference
	{
		string Name { get; }
		Stream OpenReadStream();
	}
}
