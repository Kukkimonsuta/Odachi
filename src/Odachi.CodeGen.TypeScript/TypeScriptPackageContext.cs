using System;
using Odachi.CodeModel;

namespace Odachi.CodeGen.TypeScript
{
	public class TypeScriptPackageContext : PackageContext
	{
		public TypeScriptPackageContext(Package package, string path)
		{
			Package = package ?? throw new ArgumentNullException(nameof(package));
			Path = path;
		}
	}
}
