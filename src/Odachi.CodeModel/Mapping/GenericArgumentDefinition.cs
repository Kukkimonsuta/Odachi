using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.CodeModel.Mapping
{
	public readonly struct GenericArgumentDefinition
	{
		public GenericArgumentDefinition(string name)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
		}

		public string Name { get; }
	}
}
