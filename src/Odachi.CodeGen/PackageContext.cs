using Odachi.CodeModel;

namespace Odachi.CodeGen
{
	public abstract class PackageContext<TOptions>
		where TOptions : CodeGeneratorOptions
	{
		public Package Package { get; protected set; }
		public TOptions Options { get; protected set; }
	}
}
