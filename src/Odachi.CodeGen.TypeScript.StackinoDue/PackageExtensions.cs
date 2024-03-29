using Odachi.CodeGen.TypeScript.StackinoDue.Renderers;
using Odachi.CodeGen.TypeScript.StackinoDue.TypeHandlers;
using Odachi.CodeModel;
using System;

namespace Odachi.CodeGen.TypeScript.StackinoDue
{
	public static class PackageExtensions
	{
		public static void Render_TypeScript_StackinoDue(this Package package, StackinoDueOptions options)
		{
			if (package == null)
				throw new ArgumentNullException(nameof(package));
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			var generator = new TypeScriptCodeGenerator();

			generator.TypeHandlers.Insert(0, new StackinoDueTypeHandler());

			generator.FragmentRenderers.Add(new EnumRenderer());
			if (options.UseLegacyMobx5)
			{
				generator.FragmentRenderers.Add(new Mobx5ObjectRenderer());
			}
			else
			{
				generator.FragmentRenderers.Add(new ObjectRenderer());
			}
			generator.FragmentRenderers.Add(new ServiceRenderer());

			generator.Generate(package, options);
		}
	}
}
