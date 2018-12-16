using System.IO;
using System.Threading.Tasks;
using Odachi.CodeGen.CSharp.Renderers;
using Odachi.CodeModel;
using Odachi.CodeGen.Rendering;
using Odachi.CodeGen.IO;

namespace Odachi.CodeGen.CSharp
{
	public class CSharpCodeGenerator : CodeGenerator<CSharpOptions, CSharpPackageContext, CSharpModuleContext>
	{
		public CSharpCodeGenerator()
		{
		}

		protected override CSharpModuleContext CreateModuleContext(CSharpPackageContext packageContext, Module module, CSharpOptions options)
		{
			return new CSharpModuleContext(packageContext.Package, module, options.Namespace);
		}

		protected override CSharpPackageContext CreatePackageContext(Package package, CSharpOptions options)
		{
			return new CSharpPackageContext(package, options.Path);
		}

		protected override IndentedTextWriter CreateWriter(TextWriter writer)
		{
			var indentedWriter = base.CreateWriter(writer);

			indentedWriter.OpenBlockOnNewLine = true;

			return indentedWriter;
		}
	}
}
