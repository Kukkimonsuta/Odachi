using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odachi.CodeGen.TypeScript.Internal;
using Odachi.CodeGen.TypeScript.Renderers;
using Odachi.CodeGen.IO;
using Odachi.CodeModel;
using Odachi.Extensions.Formatting;
using Odachi.CodeGen.Internal;
using Odachi.CodeGen.TypeScript.TypeHandlers;

namespace Odachi.CodeGen.TypeScript
{
	public class TypeScriptCodeGenerator : CodeGenerator<TypeScriptOptions, TypeScriptPackageContext, TypeScriptModuleContext>
	{
		public IList<ITypeHandler> TypeHandlers { get; } = new List<ITypeHandler>() { new DefaultTypeHandler() };

		protected override TypeScriptPackageContext CreatePackageContext(Package package, TypeScriptOptions options)
		{
			return new TypeScriptPackageContext(package, options);
		}

		protected override TypeScriptModuleContext CreateModuleContext(TypeScriptPackageContext packageContext, string moduleName)
		{
			return new TypeScriptModuleContext(packageContext, moduleName, TypeHandlers.ToArray() /* todo: nocopy */);
		}

		public void RenderIndex(IEnumerable<(string module, string alias)> modules, string path)
		{
			Console.WriteLine($"Generating index for '{path}'");

			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
			using (var writer = new IndentedTextWriter(new StreamWriter(stream, new UTF8Encoding(false))))
			{
				foreach (var module in modules)
				{
					if (module.alias != null)
					{
						writer.WriteIndentedLine($"import * as {module.alias} from '{module.module}';");
						writer.WriteIndentedLine($"export {{ {module.alias} }};");
					}
					else
					{
						writer.WriteIndentedLine($"export * from '{module.module}';");
					}
				}
			}
		}

		public void RenderIndexes(TypeScriptPackageContext packageContext)
		{
			var moduleNames = Array.Empty<TypeFragment>()
				.Concat(packageContext.Package.Enums)
				.Concat(packageContext.Package.Objects)
				.Concat(packageContext.Package.Services)
				.Select(m => TS.ModuleName(m.ModuleName))
				.ToArray();

			var folders = moduleNames
				.Select(m => PathTools.GetParentPath(m))
				.Distinct()
				.ToArray();

			foreach (var folder in folders)
			{
				var indexName = Path.Combine(folder, "index.ts");

				var exportedModules = folders
					.Where(f => PathTools.GetParentPath(f) == folder)
					.Select(f => (PathTools.GetRelativePath(indexName, f), Path.GetFileName(f).ToCamelInvariant()))
					.ToList();

				exportedModules.AddRange(
					moduleNames
						.Where(m => PathTools.GetParentPath(m) == folder)
						.Select(m => (PathTools.GetRelativePath(indexName, m), (string)null))
				);

				RenderIndex(exportedModules, Path.Combine(packageContext.Options.Path, indexName));
			}
		}

		public void RenderDi(TypeScriptPackageContext packageContext)
		{
			Console.WriteLine($"Generating DI module");

			var context = CreateModuleContext(packageContext, "di");
			context.Import("inversify", "interfaces");
			context.Import("inversify", "ContainerModule");

			var bodyBuilder = new StringBuilder();
			using (var writer = new IndentedTextWriter(new StringWriter(bodyBuilder)))
			{
				using (writer.WriteIndentedBlock(prefix: "const sdkModule = new ContainerModule((bind: interfaces.Bind) => ", suffix: ");"))
				{
					foreach (var serviceFragment in packageContext.Package.Services)
					{
						context.Import(TS.ModuleName(serviceFragment.ModuleName), serviceFragment.Name);

						writer.WriteIndentedLine($"bind({serviceFragment.Name}).to({serviceFragment.Name}).inSingletonScope();");
					}
				}
			}
			bodyBuilder.TrimEnd();

			context.Export($"sdkModule", @default: true);

			using (var stream = new FileStream(Path.Combine(packageContext.Options.Path, context.FileName), FileMode.Create, FileAccess.Write, FileShare.Read))
			using (var writer = new IndentedTextWriter(new StreamWriter(stream, new UTF8Encoding(false))))
			{
				context.RenderHeader(writer);
				context.RenderBody(writer, bodyBuilder.ToString());
				context.RenderFooter(writer);
			}
		}

		protected override void OnPackageFinish(TypeScriptPackageContext packageContext)
		{
			RenderIndexes(packageContext);
			if (packageContext.Options.RenderDi)
			{
				RenderDi(packageContext);
			}
		}
	}
}
