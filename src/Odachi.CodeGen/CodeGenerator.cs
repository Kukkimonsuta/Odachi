using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odachi.CodeGen.IO;
using Odachi.CodeModel;
using Odachi.CodeGen.Rendering;
using Odachi.CodeGen.Internal;

namespace Odachi.CodeGen
{
	public abstract class CodeGenerator<TOptions, TPackageContext, TModuleContext>
		where TOptions : CodeGeneratorOptions
		where TPackageContext : PackageContext<TOptions>
		where TModuleContext : ModuleContext<TOptions>
	{
		public CodeGenerator()
		{
		}

		public IList<IFragmentRenderer<TModuleContext>> FragmentRenderers { get; } = new List<IFragmentRenderer<TModuleContext>>();

		protected abstract TPackageContext CreatePackageContext(Package package, TOptions options);

		protected abstract TModuleContext CreateModuleContext(TPackageContext packageContext, string moduleName);

		protected virtual IndentedTextWriter CreateWriter(TextWriter writer)
		{
			return new IndentedTextWriter(writer);
		}

		protected virtual void OnPackageStart(TPackageContext packageContext) { }

		protected virtual void OnPackageFinish(TPackageContext packageContext) { }

		private void RenderTypeFragment(TModuleContext context, TypeFragment fragment, IndentedTextWriter writer)
		{
			Console.WriteLine($"\t\t- fragment '{fragment.ModuleName}'");

			foreach (var renderer in FragmentRenderers)
			{
				if (renderer.Render(context, fragment, writer))
				{
					return;
				}
			}

			throw new NotSupportedException($"Type fragment '{fragment.ModuleName}' has no renderer");
		}

		private void RenderModule(TModuleContext context)
		{
			Console.WriteLine($"\t- module '{context.ModuleName}'");

			var path = Path.Combine(context.PackageContext.Options.Path, context.FileName);

			var directory = Path.GetDirectoryName(path);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
			using (var writer = CreateWriter(new StreamWriter(stream, new UTF8Encoding(false))))
			{
				var bodyBuilder = new StringBuilder();
				using (var bodyWriter = CreateWriter(new StringWriter(bodyBuilder)))
				{
					foreach (var @enum in context.Enums)
					{
						RenderTypeFragment(context, @enum, bodyWriter);
					}
					foreach (var @object in context.Objects)
					{
						RenderTypeFragment(context, @object, bodyWriter);
					}
					foreach (var service in context.Services)
					{
						RenderTypeFragment(context, service, bodyWriter);
					}
				}
				bodyBuilder.TrimEnd();

				context.RenderHeader(writer);
				context.RenderBody(writer, bodyBuilder.ToString());
				context.RenderFooter(writer);
			}
		}

		public void Generate(Package package, TOptions options)
		{
			if (package == null)
				throw new ArgumentNullException(nameof(package));
			if (options == null)
				throw new ArgumentNullException(nameof(options));
			if (string.IsNullOrWhiteSpace(options.Path))
				throw new ArgumentException("Output path is required", nameof(options));

			Console.WriteLine($"Generating package '{package.Name}'");

			var packageContext = CreatePackageContext(package, options);

			if (options.CleanOutputPath && Directory.Exists(options.Path))
			{
				Directory.Delete(options.Path, true);
			}

			if (!Directory.Exists(options.Path))
			{
				Directory.CreateDirectory(options.Path);
			}

			OnPackageStart(packageContext);

			var moduleContexts = new Dictionary<string, TModuleContext>();

			TModuleContext GetOrCreateModuleContext(string moduleName)
			{
				if (!moduleContexts.TryGetValue(moduleName, out var moduleContext))
				{
					moduleContext = CreateModuleContext(packageContext, moduleName);

					moduleContexts.Add(moduleName, moduleContext);
				}

				return moduleContext;
			}

			foreach (var @enum in package.Enums)
			{
				var moduleContext = GetOrCreateModuleContext(@enum.ModuleName);

				moduleContext.Enums.Add(@enum);
			}
			foreach (var @object in package.Objects)
			{
				var moduleContext = GetOrCreateModuleContext(@object.ModuleName);

				moduleContext.Objects.Add(@object);
			}
			foreach (var service in package.Services)
			{
				var moduleContext = GetOrCreateModuleContext(service.ModuleName);

				moduleContext.Services.Add(service);
			}

			foreach (var moduleContext in moduleContexts.Values)
			{
				RenderModule(moduleContext);
			}

			OnPackageFinish(packageContext);
		}
	}
}
