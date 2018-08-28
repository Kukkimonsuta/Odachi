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
		where TPackageContext : PackageContext
		where TModuleContext : ModuleContext
	{
		public CodeGenerator()
		{
		}

		public IList<IFragmentRenderer<TModuleContext>> Renderers { get; } = new List<IFragmentRenderer<TModuleContext>>();

		protected abstract TPackageContext CreatePackageContext(Package package, TOptions options);

		protected abstract TModuleContext CreateModuleContext(TPackageContext packageContext, Module module, TOptions options);

		protected virtual IndentedTextWriter CreateWriter(TextWriter writer)
		{
			return new IndentedTextWriter(writer);
		}

		protected virtual void OnPackageStart(TPackageContext packageContext) { }

		protected virtual void OnModuleStart(TModuleContext moduleContext) { }

		protected virtual void OnModuleFinish(TModuleContext moduleContext) { }

		protected virtual void OnPackageFinish(TPackageContext packageContext) { }

		private void RenderTypeFragment(TModuleContext context, TypeFragment fragment, IndentedTextWriter writer)
		{
			Console.WriteLine($"\t\t- fragment '{fragment.Name}'");

			foreach (var renderer in Renderers)
			{
				if (renderer.Render(context, fragment, writer))
				{
					return;
				}
			}

			throw new NotSupportedException($"Type fragment '{fragment.Kind}' is not supported");
		}

		private void RenderModule(TModuleContext context, string path)
		{
			Console.WriteLine($"\t- module '{context.Module.Name}'");

			var directory = Path.GetDirectoryName(path);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);

			using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
			using (var writer = CreateWriter(new StreamWriter(stream, new UTF8Encoding(false))))
			{
				var bodyBuilder = new StringBuilder();
				using (var bodyWriter = CreateWriter(new StringWriter(bodyBuilder)))
				{
					foreach (var fragment in context.Module.Fragments)
					{
						RenderTypeFragment(context, fragment, bodyWriter);
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
			Console.WriteLine($"Generating package '{package.Name}'");

			var packageContext = CreatePackageContext(package, options);

			if (!Directory.Exists(packageContext.Path))
				Directory.CreateDirectory(packageContext.Path);

			OnPackageStart(packageContext);

			foreach (var module in package.Modules)
			{
				var moduleContext = CreateModuleContext(packageContext, module, options);

				OnModuleStart(moduleContext);

				RenderModule(moduleContext, Path.Combine(packageContext.Path, moduleContext.FileName));

				OnModuleFinish(moduleContext);
			}

			OnPackageFinish(packageContext);
		}
	}
}
