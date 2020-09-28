using System;
using Odachi.CodeModel;

namespace Odachi.CodeGen.CSharp
{
	public class CSharpPackageContext : PackageContext<CSharpOptions>
	{
		public CSharpPackageContext(Package package, CSharpOptions options)
		{
			Package = package ?? throw new ArgumentNullException(nameof(package));
			Options = options;
		}
	}
}
