using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odachi.CodeGen.TypeScript.Internal;
using Odachi.CodeGen.IO;
using Odachi.CodeModel;
using Odachi.CodeGen.Rendering;

namespace Odachi.CodeGen.TypeScript.Renderers
{
	public class ObjectRenderer : IFragmentRenderer<TypeScriptModuleContext>
	{
		public bool Render(TypeScriptModuleContext context, Fragment fragment, IndentedTextWriter writer)
		{
			if (!(fragment is ObjectFragment objectFragment))
				return false;
			
			if (objectFragment.Hints.TryGetValue("source-type", out var sourceType))
			{
				writer.WriteIndentedLine($"// source: {sourceType}");
				writer.WriteSeparatingLine();
			}

			using (writer.WriteIndentedBlock(prefix: $"class {objectFragment.Name} "))
			{
				foreach (var field in objectFragment.Fields)
				{
					context.Import("mobx", "observable");

					writer.WriteIndentedLine("@observable.ref");
					writer.WriteIndentedLine($"{TS.Field(field.Name)}: {context.Resolve(field.Type)};");
					writer.WriteSeparatingLine();
				}

				using (writer.WriteIndentedBlock(prefix: $"static create(source: any): {objectFragment.Name} "))
				{
					writer.WriteIndentedLine($"const result = new {objectFragment.Name}();");

					foreach (var field in objectFragment.Fields)
					{
						writer.WriteIndentedLine($"result.{TS.Field(field.Name)} = {context.CreateExpression(field.Type, $"source.{TS.Field(field.Name)}")};");
					}

					writer.WriteIndentedLine("return result;");
				}
			}

			context.Export(objectFragment.Name, @default: true);

			return true;
		}
	}
}
