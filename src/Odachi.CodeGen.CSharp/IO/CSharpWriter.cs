using Odachi.CodeGen.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.CodeGen.CSharp.IO
{
	public class CSharpWriter : IndentWriter
	{
		public CSharpWriter()
			: base(new StringWriter(CultureInfo.InvariantCulture))
		{
		}

		public void WriteTo(TextWriter writer)
		{
			writer.Write(
				((StringWriter)Writer).GetStringBuilder().ToString()
			);
		}
	}
}
