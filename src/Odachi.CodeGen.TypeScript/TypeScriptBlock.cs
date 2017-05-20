using System;

namespace Odachi.CodeModelGen.TypeScript
{
	public class TypeScriptBlock : IDisposable
	{
		public TypeScriptBlock(TypeScriptWriter writer, string prefix = null, string suffix = null)
		{
			if (writer == null)
				throw new ArgumentNullException(nameof(writer));

			Writer = writer;
			_suffix = suffix;

			if (prefix != null)
			{
				Writer.WriteIndented($"{prefix} {{");
			}
			else
			{
				Writer.WriteIndented("{");
			}
			Writer.Indent(1);
		}

		private string _suffix;

		protected TypeScriptWriter Writer { get; set; }

		public void Dispose()
		{
			Writer.Indent(-1);
			Writer.WriteIndented($"}}{_suffix}");
		}
	}
}
