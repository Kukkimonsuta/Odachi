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
	public class ClassRenderer : IFragmentRenderer<CSharpModuleContext>
	{
		public bool Render(CSharpModuleContext context, Fragment fragment, IndentedTextWriter writer)
		{
			if (!(fragment is ClassFragment classFragment))
				return false;

			if (classFragment.Hints["logical-kind"] != "class")
				return false;

			if (classFragment.Methods.Count > 0)
				throw new NotSupportedException("Methods on classes are not supported");

			if (classFragment.Hints.TryGetValue("source-type", out var sourceType))
			{
				writer.WriteIndented($"// source: {sourceType}");
				writer.WriteLine();
			}

			using (writer.WriteIndentedBlock(prefix: $"public class {classFragment.Name} "))
			{
				foreach (var field in classFragment.Fields)
				{
					writer.WriteIndented($"public {context.Resolve(field.Type)} {CS.Field(field.Name)} {{ get; set; }}");
				}
			}

			return true;
		}
	}
}
