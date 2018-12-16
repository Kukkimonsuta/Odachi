using Odachi.CodeGen.TypeScript.Renderers;
using Odachi.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Odachi.CodeGen.TypeScript
{
	public static class PackageExtensions
	{
		public static void Render_TypeScript_Default(this Package package, TypeScriptOptions options)
		{
			if (package == null)
				throw new ArgumentNullException(nameof(package));
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			var generator = new TypeScriptCodeGenerator();

			generator.FragmentRenderers.Add(new EnumRenderer());
			generator.FragmentRenderers.Add(new ObjectRenderer());
			generator.FragmentRenderers.Add(new ServiceRenderer());

			generator.Generate(package, options);
		}
	}
}
