using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeGen.IO;
using Odachi.CodeModel;

namespace Odachi.CodeGen.Rendering
{
	public interface IFragmentRenderer<TModuleContext>
	{
		bool Render(TModuleContext context, Fragment fragment, IndentedTextWriter writer);
	}
}
