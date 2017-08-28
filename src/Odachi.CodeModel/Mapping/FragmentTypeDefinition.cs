using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.CodeModel.Mapping
{
	public class FragmentTypeDefinition : TypeDefinition
	{
		public FragmentTypeDefinition(string moduleName, string fragmentName, params GenericArgumentDefinition[] genericArgumentDefinitions)
		{
			if (moduleName == null)
				throw new ArgumentNullException(nameof(moduleName));
			if (fragmentName == null)
				throw new ArgumentNullException(nameof(fragmentName));

			Module = moduleName;
			Name = fragmentName;
			GenericArgumentDefinitions = genericArgumentDefinitions;
		}

		public override string Module { get; }
		public override string Name { get; }
		public override IReadOnlyList<GenericArgumentDefinition> GenericArgumentDefinitions { get; }
	}
}
