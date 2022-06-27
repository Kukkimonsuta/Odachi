using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeGen.CSharp.Internal;
using Odachi.CodeModel;
using Odachi.CodeGen.IO;
using Odachi.CodeGen.Rendering;

namespace Odachi.CodeGen.CSharp.Renderers
{
	public class EnumRenderer : IFragmentRenderer<CSharpModuleContext>
	{
		public bool Render(CSharpModuleContext context, Fragment fragment, IndentedTextWriter writer)
		{
			if (!(fragment is EnumFragment enumFragment))
				return false;

			if (enumFragment.Hints.TryGetValue("source-type", out var sourceType))
			{
				writer.WriteIndentedLine($"// source: {sourceType}");
				writer.WriteSeparatingLine();
			}

			if (enumFragment.Hints.TryGetValue("enum-flags", out var enumFlags) && string.Equals(enumFlags, "true", StringComparison.OrdinalIgnoreCase))
			{
				context.Import("System");

				writer.WriteIndentedLine($"[Flags]");
			}
			using (writer.WriteIndentedBlock(prefix: $"public enum {CS.Type(fragment.Name)} "))
			{
				foreach (var item in enumFragment.Items)
				{
					writer.WriteIndentedLine($"{CS.Field(item.Name)} = {item.Value},");
				}
			}

			var itemHasDisplayNameHint = enumFragment.Items.Any(i => i.Hints.ContainsKey("display-name"));

			if (itemHasDisplayNameHint)
			{
				context.Import("System");

				using (writer.WriteIndentedBlock(prefix: $"public static class {CS.Type(enumFragment.Name)}Extensions "))
				{
					using (writer.WriteIndentedBlock(prefix: $"public static string GetDisplayName(this {CS.Type(enumFragment.Name)} value) "))
					{
						using (writer.WriteIndentedBlock(prefix: $"switch (value) "))
						{
							foreach (var item in enumFragment.Items)
							{
								if (!item.Hints.TryGetValue("display-name", out var displayName))
								{
									continue;
								}

								writer.WriteIndentedLine($"case {CS.Type(enumFragment.Name)}.{CS.Field(item.Name)}:");
								writer.WriteIndentedLine($"{writer.IndentString}return \"{displayName}\";");
							}

							writer.WriteIndentedLine("default:");
							writer.WriteIndentedLine($"{writer.IndentString}throw new InvalidOperationException($\"Undefined behavior for '{{value}}'\");");
						}
					}
				}
			}

			return true;
		}
	}
}
