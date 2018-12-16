using Odachi.CodeGen.CSharp.Renderers;
using Odachi.CodeModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Odachi.CodeGen.CSharp
{
	public static class PackageExtensions
	{
		public static void Render_CSharp_Default(this Package package, CSharpOptions options)
		{
			if (package == null)
				throw new ArgumentNullException(nameof(package));
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			var generator = new CSharpCodeGenerator();

			generator.FragmentRenderers.Add(new EnumRenderer());
			generator.FragmentRenderers.Add(new ObjectRenderer());
			generator.FragmentRenderers.Add(new ServiceRenderer());

			generator.Generate(package, options);
		}
	}
}
