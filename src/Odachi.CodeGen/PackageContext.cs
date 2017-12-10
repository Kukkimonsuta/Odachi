using Odachi.CodeModel;

namespace Odachi.CodeGen
{
	public abstract class PackageContext
	{
		public Package Package { get; protected set; }
		public string Path { get; protected set; }
	}
}
