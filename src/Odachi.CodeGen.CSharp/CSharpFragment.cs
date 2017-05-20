using Odachi.CodeGen.CSharp.IO;
using Odachi.CodeGen.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Odachi.CodeGen.CSharp
{
	public abstract class CSharpFragment
	{
		public CSharpFragment(string name)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			Name = name;
		}

		public string Name { get; set; }

		public virtual IEnumerable<CSharpMapping> CreateMappings(CSharpContext context, CSharpPackage package, CSharpModule module)
		{
			yield break;
		}

		public abstract void WriteTo(CSharpContext context, CSharpPackage package, CSharpModule module, CSharpWriter writer);
	}
}
