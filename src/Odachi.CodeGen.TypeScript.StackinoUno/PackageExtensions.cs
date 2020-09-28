using Odachi.CodeGen.TypeScript.StackinoUno.Renderers;
using Odachi.CodeGen.TypeScript.StackinoUno.TypeHandlers;
using Odachi.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Odachi.CodeGen.TypeScript.StackinoUno
{
	public static class PackageExtensions
	{
		public static void Render_TypeScript_StackinoUno(this Package package, TypeScriptOptions options)
		{
			if (package == null)
				throw new ArgumentNullException(nameof(package));
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			var generator = new TypeScriptCodeGenerator();

			generator.TypeHandlers.Insert(0, new StackinoUnoTypeHandler());

			generator.FragmentRenderers.Add(new EnumRenderer());
			generator.FragmentRenderers.Add(new ObjectRenderer());
			generator.FragmentRenderers.Add(new ServiceRenderer());

			generator.Generate(package, options);
		}
	}
}
