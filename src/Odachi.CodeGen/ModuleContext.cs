using Odachi.CodeGen.IO;
using Odachi.CodeModel;
using System.Collections.Generic;

namespace Odachi.CodeGen
{
	public abstract class ModuleContext<TOptions>
		where TOptions : CodeGeneratorOptions
	{
		public PackageContext<TOptions> PackageContext { get; protected set; }
		public string ModuleName { get; protected set; }
		public string FileName { get; protected set; }

		public IList<EnumFragment> Enums { get; protected set; } = new List<EnumFragment>();
		public IList<ObjectFragment> Objects { get; protected set; } = new List<ObjectFragment>();
		public IList<ServiceFragment> Services { get; protected set; } = new List<ServiceFragment>();

		public abstract bool RenderHeader(IndentedTextWriter writer);
		public abstract bool RenderBody(IndentedTextWriter writer, string body);
		public abstract bool RenderFooter(IndentedTextWriter writer);
	}
}
