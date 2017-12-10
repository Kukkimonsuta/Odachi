using System.IO;
using System.Threading.Tasks;
using Odachi.CodeGen.CSharp.Renderers;
using Odachi.CodeModel;
using Odachi.CodeGen.Rendering;

namespace Odachi.CodeGen.CSharp
{
	public class CSharpCodeGenerator : CodeGenerator<CSharpOptions, CSharpPackageContext, CSharpModuleContext>
	{
		public CSharpCodeGenerator()
		{
			Renderers.Add(new EnumRenderer());
			Renderers.Add(new ClassRenderer());
			Renderers.Add(new JsonRpcServiceRenderer());
		}

		protected override CSharpModuleContext CreateModuleContext(CSharpPackageContext packageContext, Module module, CSharpOptions options)
		{
			return new CSharpModuleContext(packageContext.Package, module, options.Namespace);
		}

		protected override CSharpPackageContext CreatePackageContext(Package package, CSharpOptions options)
		{
			return new CSharpPackageContext(package, options.Path);
		}
	}
}
