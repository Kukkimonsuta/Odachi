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

namespace Odachi.CodeGen.TypeScript.StackinoUno.Renderers
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

			var genericSuffix = string.Join(", ", objectFragment.GenericArguments.Select(a => a.Name));

			using (writer.WriteIndentedBlock(prefix: $"class {objectFragment.Name}{(genericSuffix.Length > 0 ? $"<{genericSuffix}>" : "")} "))
			{
				if (objectFragment.Constants.Any())
				{
					foreach (var constant in objectFragment.Constants)
					{
						writer.WriteIndentedLine($"static readonly {TS.Field(constant.Name)}: {context.Resolve(constant.Type)} = {TS.Constant(constant.Value)};");
					}
					writer.WriteSeparatingLine();
				}

				foreach (var field in objectFragment.Fields)
				{
					context.Import("mobx", "observable");

					writer.WriteIndentedLine("@observable.ref");
					writer.WriteIndentedLine($"{TS.Field(field.Name)}: {context.Resolve(field.Type)} = {context.ResolveDefaultValue(field.Type)};");
					writer.WriteSeparatingLine();
				}

				if (objectFragment.GenericArguments.Any())
				{
					var genericParameters = $"<{string.Join(", ", objectFragment.GenericArguments.Select(a => a.Name))}>";
					var factoryParameters = string.Join(", ", objectFragment.GenericArguments.Select(a => $"{a.Name}_factory: {{ create(source: any): {a.Name} }}"));

					using (writer.WriteIndentedBlock(prefix: $"static create{genericParameters}({factoryParameters}): {{ create: (source: any) => {objectFragment.Name}{genericParameters} }} "))
					{
						using (writer.WriteIndentedBlock(prefix: $"return ", suffix: ";"))
						{
							using (writer.WriteIndentedBlock(prefix: $"create: (source: any) => "))
							{
								writer.WriteIndentedLine($"const result = new {objectFragment.Name}{genericParameters}();");
								foreach (var field in objectFragment.Fields)
								{
									writer.WriteIndentedLine($"result.{TS.Field(field.Name)} = {context.CreateExpression(field.Type, $"source.{TS.Field(field.Name)}")};");
								}
								writer.WriteIndentedLine("return result;");
							}
						}
					}
				}
				else
				{
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
			}

			context.Export(objectFragment.Name, @default: true);

			return true;
		}
	}
}
