using Odachi.CodeGen.IO;
using Odachi.CodeModel;

namespace Odachi.CodeGen
{
	public abstract class ModuleContext
	{
		public Package Package { get; protected set; }
		public Module Module { get; protected set; }
		public string FileName { get; protected set; }

		public abstract bool RenderHeader(IndentedTextWriter writer);
		public abstract bool RenderBody(IndentedTextWriter writer, string body);
		public abstract bool RenderFooter(IndentedTextWriter writer);
	}
}
