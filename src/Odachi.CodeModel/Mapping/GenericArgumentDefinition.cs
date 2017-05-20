using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.CodeModel.Mapping
{
	public class GenericArgumentDefinition
	{
		public GenericArgumentDefinition(string name)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			Name = name;
		}

		public string Name { get; }
	}
}
