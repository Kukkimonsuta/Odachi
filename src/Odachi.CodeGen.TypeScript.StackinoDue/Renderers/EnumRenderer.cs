using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeGen.TypeScript.Internal;
using Odachi.CodeGen.IO;
using Odachi.CodeModel;
using Odachi.CodeGen.Rendering;

namespace Odachi.CodeGen.TypeScript.StackinoDue.Renderers
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

			using (writer.WriteIndentedBlock(prefix: $"enum {TS.Type(fragment.Name)} "))
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
					writer.WriteIndentedLine($"[{TS.Type(enumFragment.Name)}.{TS.Field(item.Name)}]: '{TS.Field(item.Name)}',");
				}
			}

			if (itemHasDisplayNameHint)
			{
				using (writer.WriteIndentedBlock(prefix: $"const displayNames = ", suffix: ";"))
				{
					foreach (var item in enumFragment.Items)
					{
						writer.WriteIndentedLine($"[{TS.Type(enumFragment.Name)}.{TS.Field(item.Name)}]: '{item.Hints["display-name"] ?? ""}',");
					}
				}
			}

			using (writer.WriteIndentedBlock(prefix: $"namespace {TS.Type(enumFragment.Name)} "))
			{
				if (enumFragment.Hints.TryGetValue("enum-flags", out var enumFlags) && string.Equals(enumFlags, "true", StringComparison.OrdinalIgnoreCase))
				{
					writer.WriteIndentedLine($@"export function create(value: any): {TS.Type(enumFragment.Name)} {{
    if (typeof value !== 'number') {{
        throw new Error(`Value '${{value}}' is not valid for enum {TS.Type(enumFragment.Name)}`);
    }}

    let remainder = value;
    for (let k in {TS.Type(enumFragment.Name)}) {{
        const v = {TS.Type(enumFragment.Name)}[k];
        if (!{TS.Type(enumFragment.Name)}.hasOwnProperty(v)) {{
            continue;
        }}

        remainder = remainder & ~v;
    }}

	if (remainder !== 0) {{
		throw new Error(`Remainder '${{remainder}}' of '${{value}}' is not valid for enum {TS.Type(enumFragment.Name)}`);
	}}

	return value as {TS.Type(enumFragment.Name)};
}};");
				}
				else
				{
					writer.WriteIndentedLine($@"export function create(value: any): {TS.Type(enumFragment.Name)} {{
	if (!{TS.Type(enumFragment.Name)}.hasOwnProperty(value)) {{
		throw new Error(`Value '${{value}}' is not valid for enum {TS.Type(enumFragment.Name)}`);
	}}

	return value as {TS.Type(enumFragment.Name)};
}}");
				}
				writer.WriteSeparatingLine();

				using (writer.WriteIndentedBlock(prefix: $"export function getValues(): {TS.Type(enumFragment.Name)}[] "))
				{
					using (writer.WriteIndentedBlock(prefix: "return ", open: "[", close: "]", suffix: ";"))
					{
						foreach (var item in enumFragment.Items)
						{
							writer.WriteIndentedLine($"{TS.Type(enumFragment.Name)}.{TS.Field(item.Name)},");
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

				writer.WriteIndentedLine($@"export function getName(value: {TS.Type(enumFragment.Name)}): string {{
	const name = names[value];

	if (name === undefined) {{
		throw new Error(`Cannot get name of {TS.Type(enumFragment.Name)} '${{value}}'`);
	}}

	return name;
}}");
				writer.WriteSeparatingLine();

				if (itemHasDisplayNameHint)
				{
					writer.WriteIndentedLine($@"export function getDisplayName(value: {TS.Type(enumFragment.Name)}): string {{
	const displayName = displayNames[value];

	if (displayName === undefined) {{
		throw new Error(`Cannot get display name of {TS.Type(enumFragment.Name)} '${{value}}'`);
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
