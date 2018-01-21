using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeGen.TypeScript.Internal;
using Odachi.CodeGen.IO;
using Odachi.CodeModel;
using Odachi.CodeGen.Rendering;

namespace Odachi.CodeGen.TypeScript.Renderers
{
	public class EnumRenderer : IFragmentRenderer<TypeScriptModuleContext>
	{
		public bool Render(TypeScriptModuleContext context, Fragment fragment, IndentedTextWriter writer)
		{
			if (!(fragment is EnumFragment enumFragment))
				return false;

			if (enumFragment.Hints.TryGetValue("source-type", out var sourceType))
			{
				writer.WriteIndentedLine($"// source: {sourceType}");
				writer.WriteSeparatingLine();
			}

			using (writer.WriteIndentedBlock(prefix: $"enum {fragment.Name} "))
			{
				foreach (var item in enumFragment.Items)
				{
					writer.WriteIndentedLine($"{TS.Field(item.Name)} = {item.Value},");
				}
			}

			var itemHasDisplayNameHint = enumFragment.Items.Any(i => i.Hints.ContainsKey("display-name"));

			using (writer.WriteIndentedBlock(prefix: $"const names = ", suffix: ";"))
			{
				foreach (var item in enumFragment.Items)
				{
					writer.WriteIndentedLine($"[{enumFragment.Name}.{TS.Field(item.Name)}]: '{TS.Field(item.Name)}',");
				}
			}

			if (itemHasDisplayNameHint)
			{
				using (writer.WriteIndentedBlock(prefix: $"const displayNames = ", suffix: ";"))
				{
					foreach (var item in enumFragment.Items)
					{
						writer.WriteIndentedLine($"[{enumFragment.Name}.{TS.Field(item.Name)}]: '{item.Hints["display-name"] ?? ""}',");
					}
				}
			}

			using (writer.WriteIndentedBlock(prefix: $"namespace {enumFragment.Name} "))
			{
				writer.WriteIndentedLine($@"export function create(value: any): {enumFragment.Name} {{
	if (!{enumFragment.Name}.hasOwnProperty(value)) {{
		throw new Error(`Value '${{value}}' is not valid for enum {enumFragment.Name}`);
	}}

	return value as {enumFragment.Name};
}}");
				writer.WriteSeparatingLine();

				using (writer.WriteIndentedBlock(prefix: $"export function getValues(): {enumFragment.Name}[] "))
				{
					using (writer.WriteIndentedBlock(prefix: "return ", open: "[", close: "]", suffix: ";"))
					{
						foreach (var item in enumFragment.Items)
						{
							writer.WriteIndentedLine($"{enumFragment.Name}.{TS.Field(item.Name)},");
						}
					}
				}

				using (writer.WriteIndentedBlock(prefix: $"export function getNames(): string[] "))
				{
					using (writer.WriteIndentedBlock(prefix: "return ", open: "[", close: "]", suffix: ";"))
					{
						foreach (var item in enumFragment.Items)
						{
							writer.WriteIndentedLine($"'{TS.Field(item.Name)}',");
						}
					}
				}

				if (itemHasDisplayNameHint)
				{
					writer.WriteIndentedLine($@"export function getDisplayName(value: {enumFragment.Name}): string {{
	const displayName = displayNames[value];

	if (displayName === undefined) {{
		throw new Error(`Cannot get display name of {enumFragment.Name} '${{value}}'`);
	}}

	return displayName;
}}");
					writer.WriteSeparatingLine();
				}
			}

			context.Export(enumFragment.Name, @default: true);

			return true;
		}
	}
}
