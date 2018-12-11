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
				writer.WriteIndentedLine($"// source: {sourceType}");
				writer.WriteSeparatingLine();
			}

			var genericSuffix = string.Join(", ", objectFragment.GenericArguments.Select(a => a.Name));

			using (writer.WriteIndentedBlock(prefix: $"public class {objectFragment.Name}{(genericSuffix.Length > 0 ? $"<{genericSuffix}>" : "")} "))
			{
				if (objectFragment.Constants.Any())
				{
					foreach (var constant in objectFragment.Constants)
					{
						writer.WriteIndentedLine($"public const {context.Resolve(constant.Type)} {CS.Field(constant.Name)} = {CS.Constant(constant.Value)};");
					}
					writer.WriteSeparatingLine();
				}

				foreach (var field in objectFragment.Fields)
				{
					writer.WriteIndentedLine($"public {context.Resolve(field.Type)} {CS.Field(field.Name)} {{ get; set; }}");
				}
			}

			return true;
		}
	}
}
