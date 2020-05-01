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

namespace Odachi.CodeGen.TypeScript.StackinoDue.Renderers
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

			using (writer.WriteIndentedBlock(prefix: $"class {TS.Type(objectFragment.Name)}{(genericSuffix.Length > 0 ? $"<{genericSuffix}>" : "")} "))
			{
				if (objectFragment.Constants.Any())
				{
					foreach (var constant in objectFragment.Constants)
					{
						writer.WriteIndentedLine($"static readonly {TS.Field(constant.Name)}: {context.Resolve(constant.Type)} = {TS.Constant(constant.Value)};");
					}
					writer.WriteSeparatingLine();
				}

				if (objectFragment.Fields.Count > 0)
				{
					foreach (var field in objectFragment.Fields)
					{
						context.Import("mobx", "observable");

						if (field.Type.Kind == TypeKind.Array)
						{
							writer.WriteIndentedLine("@observable.shallow");
						}
						else
						{
							writer.WriteIndentedLine("@observable.ref");
						}
						writer.WriteIndentedLine($"{TS.Field(field.Name)}: {context.Resolve(field.Type)} = {context.ResolveDefaultValue(field.Type)};");
						writer.WriteSeparatingLine();
					}

					if (objectFragment.GenericArguments.Any())
					{
						var genericParameters = $"<{string.Join(", ", objectFragment.GenericArguments.Select(a => a.Name))}>";
						var factoryParameters = string.Join(", ", objectFragment.GenericArguments.Select(a => $"{a.Name}_factory: {{ create(source: any): {a.Name} }}"));

						using (writer.WriteIndentedBlock(prefix: $"static create{genericParameters}({factoryParameters}): {{ create: (source: any) => {TS.Type(objectFragment.Name)}{genericParameters} }} "))
						{
							using (writer.WriteIndentedBlock(prefix: $"return ", suffix: ";"))
							{
								using (writer.WriteIndentedBlock(prefix: $"create: (source: any) => "))
								{
									writer.WriteIndentedLine($"const result = new {TS.Type(objectFragment.Name)}{genericParameters}();");
									foreach (var field in objectFragment.Fields)
									{
										writer.WriteIndentedLine($"result.{TS.Field(field.Name)} = {context.CreateExpression(field.Type, $"source.{TS.Field(field.Name)}")};");
									}
									writer.WriteIndentedLine("return result;");
								}
							}
						}
						writer.WriteSeparatingLine();
						using (writer.WriteIndentedBlock(prefix: $"static copy{genericParameters}(source: {TS.Type(objectFragment.Name)}{genericParameters}, destination: {TS.Type(objectFragment.Name)}{genericParameters}): void "))
						{
							foreach (var field in objectFragment.Fields)
							{
								writer.WriteIndentedLine($"destination.{TS.Field(field.Name)} = source.{TS.Field(field.Name)};");
							}
						}
						writer.WriteSeparatingLine();
						using (writer.WriteIndentedBlock(prefix: $"static clone{genericParameters}(source: {TS.Type(objectFragment.Name)}{genericParameters}): {TS.Type(objectFragment.Name)}{genericParameters} "))
						{
							writer.WriteIndentedLine($"const result = new {TS.Type(objectFragment.Name)}{genericParameters}();");
							writer.WriteIndentedLine($"{TS.Type(objectFragment.Name)}.copy{genericParameters}(source, result);");
							writer.WriteIndentedLine("return result;");
						}
					}
					else
					{
						using (writer.WriteIndentedBlock(prefix: $"static create(source: any): {TS.Type(objectFragment.Name)} "))
						{
							writer.WriteIndentedLine($"const result = new {TS.Type(objectFragment.Name)}();");
							foreach (var field in objectFragment.Fields)
							{
								writer.WriteIndentedLine($"result.{TS.Field(field.Name)} = {context.CreateExpression(field.Type, $"source.{TS.Field(field.Name)}")};");
							}
							writer.WriteIndentedLine("return result;");
						}
						writer.WriteSeparatingLine();
						using (writer.WriteIndentedBlock(prefix: $"static copy(source: {TS.Type(objectFragment.Name)}, destination: {TS.Type(objectFragment.Name)}): void "))
						{
							foreach (var field in objectFragment.Fields)
							{
								writer.WriteIndentedLine($"destination.{TS.Field(field.Name)} = source.{TS.Field(field.Name)};");
							}
						}
						writer.WriteSeparatingLine();
						using (writer.WriteIndentedBlock(prefix: $"static clone(source: {TS.Type(objectFragment.Name)}): {TS.Type(objectFragment.Name)} "))
						{
							writer.WriteIndentedLine($"const result = new {TS.Type(objectFragment.Name)}();");
							writer.WriteIndentedLine($"{TS.Type(objectFragment.Name)}.copy(source, result);");
							writer.WriteIndentedLine("return result;");
						}
					}
				}
			}

			context.Export(objectFragment.Name, @default: true);

			return true;
		}
	}
}
