using System;
using Odachi.CodeModel;

namespace Odachi.CodeGen.CSharp
{
	public class CSharpPackageContext : PackageContext
	{
		public CSharpPackageContext(Package package, string path)
		{
			Package = package ?? throw new ArgumentNullException(nameof(package));
			Path = path;
		}
	}
}
