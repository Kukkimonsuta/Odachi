using System;
using Odachi.CodeModel;

namespace Odachi.CodeGen.TypeScript
{
	public class TypeScriptPackageContext : PackageContext<TypeScriptOptions>
	{
		public TypeScriptPackageContext(Package package, TypeScriptOptions options)
		{
			Package = package ?? throw new ArgumentNullException(nameof(package));
			Options = options ?? throw new ArgumentNullException(nameof(options));
		}
	}
}
