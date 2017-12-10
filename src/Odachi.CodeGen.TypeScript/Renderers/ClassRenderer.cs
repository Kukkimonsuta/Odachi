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
	public class ClassRenderer : IFragmentRenderer<TypeScriptModuleContext>
	{
		public bool Render(TypeScriptModuleContext context, Fragment fragment, IndentedTextWriter writer)
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

			using (writer.WriteIndentedBlock(prefix: $"class {classFragment.Name} "))
			{
				foreach (var field in classFragment.Fields)
				{
					context.Import("mobx", "observable");

					writer.WriteIndented("@observable.ref");
					writer.WriteIndented($"{TS.Field(field.Name)}: {context.Resolve(field.Type)};");
					writer.WriteLine();
				}

				using (writer.WriteIndentedBlock(prefix: $"static create(source: any): {classFragment.Name} "))
				{
					writer.WriteIndented($"const result = new {classFragment.Name}();");

					foreach (var field in classFragment.Fields)
					{
						writer.WriteIndented($"result.{TS.Field(field.Name)} = {context.CreateExpression(field.Type, $"source.{TS.Field(field.Name)}")};");
					}

					writer.WriteIndented("return result;");
				}
				writer.WriteLine();
			}
			writer.WriteLine();

			context.Export(classFragment.Name, @default: true);

			return true;
		}
	}
}
