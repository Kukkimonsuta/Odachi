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

namespace Odachi.CodeGen.TypeScript
{
	public class TypeScriptCodeGenerator : CodeGenerator<TypeScriptOptions, TypeScriptPackageContext, TypeScriptModuleContext>
	{
		public TypeScriptCodeGenerator()
		{
			Renderers.Add(new EnumRenderer());
			Renderers.Add(new ObjectRenderer());
			Renderers.Add(new ServiceRenderer());
		}

		protected override TypeScriptPackageContext CreatePackageContext(Package package, TypeScriptOptions options)
		{
			return new TypeScriptPackageContext(package, options);
		}

		protected override TypeScriptModuleContext CreateModuleContext(TypeScriptPackageContext packageContext, Module module, TypeScriptOptions options)
		{
			return new TypeScriptModuleContext(packageContext.Package, module, options);
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
			var moduleNames = packageContext.Package.Modules
				.Select(m => TS.ModuleName(m.Name))
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

				RenderIndex(exportedModules, Path.Combine(packageContext.Path, indexName));
			}
		}

		public void RenderDi(TypeScriptPackageContext packageContext)
		{
			Console.WriteLine($"Generating DI module");

			var context = new TypeScriptModuleContext(packageContext.Package, new Module() { Name = "./di.tsx" }, packageContext.Options);
			context.Import("inversify", "interfaces");
			context.Import("inversify", "ContainerModule");

			var bodyBuilder = new StringBuilder();
			using (var writer = new IndentedTextWriter(new StringWriter(bodyBuilder)))
			{
				using (writer.WriteIndentedBlock(prefix: "const sdkModule = new ContainerModule((bind: interfaces.Bind) => ", suffix: ");"))
				{
					foreach (var module in packageContext.Package.Modules)
					{
						var serviceFragments = module.Fragments.OfType<ServiceFragment>()
							.ToArray();

						if (serviceFragments.Length <= 0)
							continue;

						foreach (var rpcServiceFragment in serviceFragments)
						{
							context.Import(TS.ModuleName(module.Name), rpcServiceFragment.Name);

							writer.WriteIndentedLine($"bind({rpcServiceFragment.Name}).to({rpcServiceFragment.Name}).inSingletonScope();");
						}
					}
				}
			}
			bodyBuilder.TrimEnd();

			context.Export($"sdkModule", @default: true);

			using (var stream = new FileStream(Path.Combine(packageContext.Path, context.Module.Name), FileMode.Create, FileAccess.Write, FileShare.Read))
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
			RenderDi(packageContext);
		}
	}
}
