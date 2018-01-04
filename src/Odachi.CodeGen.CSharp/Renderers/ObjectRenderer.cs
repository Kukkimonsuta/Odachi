using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odachi.CodeGen.CSharp.Internal;
using Odachi.CodeModel;
using Odachi.CodeGen.IO;
using Odachi.CodeGen.Rendering;

namespace Odachi.CodeGen.CSharp.Renderers
{
	public class ObjectRenderer : IFragmentRenderer<CSharpModuleContext>
	{
		public bool Render(CSharpModuleContext context, Fragment fragment, IndentedTextWriter writer)
		{
			if (!(fragment is ObjectFragment objectFragment))
				return false;
			
			if (objectFragment.Hints.TryGetValue("source-type", out var sourceType))
			{
				writer.WriteIndented($"// source: {sourceType}");
				writer.WriteLine();
			}

			using (writer.WriteIndentedBlock(prefix: $"public class {objectFragment.Name} "))
			{
				foreach (var field in objectFragment.Fields)
				{
					writer.WriteIndented($"public {context.Resolve(field.Type)} {CS.Field(field.Name)} {{ get; set; }}");
				}
			}

			return true;
		}
	}
}
