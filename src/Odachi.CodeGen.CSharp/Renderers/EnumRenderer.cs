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
				writer.WriteIndented($"// source: {sourceType}");
				writer.WriteLine();
			}

			using (writer.WriteIndentedBlock(prefix: $"public enum {fragment.Name} "))
			{
				foreach (var item in enumFragment.Items)
				{
					writer.WriteIndented($"{CS.Field(item.Name)} = {item.Value},");
				}
			}
			writer.WriteLine();

			var itemHasDisplayNameHint = enumFragment.Items.Any(i => i.Hints.ContainsKey("display-name"));

			if (itemHasDisplayNameHint)
			{
				context.Import("System");

				using (writer.WriteIndentedBlock(prefix: $"public static class {enumFragment.Name}Extensions "))
				{
					using (writer.WriteIndentedBlock(prefix: $"public static string GetDisplayName(this {enumFragment.Name} value) "))
					{
						using (writer.WriteIndentedBlock(prefix: $"switch (value) "))
						{
							foreach (var item in enumFragment.Items)
							{
								if (!item.Hints.TryGetValue("display-name", out var displayName))
								{
									continue;
								}

								writer.WriteIndented($"case {enumFragment.Name}.{CS.Field(item.Name)}:");
								writer.WriteIndented($"{writer.IndentString}return \"{displayName}\";");
							}

							writer.WriteIndented("default:");
							writer.WriteIndented($"{writer.IndentString}throw new InvalidOperationException($\"Undefined behavior for '{{value}}'\");");
						}
					}
				}
			}

			return true;
		}
	}
}
