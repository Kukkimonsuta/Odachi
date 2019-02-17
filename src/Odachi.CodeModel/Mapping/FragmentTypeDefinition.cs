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
			Module = moduleName ?? throw new ArgumentNullException(nameof(moduleName));
			Name = fragmentName ?? throw new ArgumentNullException(nameof(fragmentName));
			GenericArguments = genericArgumentDefinitions;
		}

		public override string Module { get; }
		public override string Name { get; }
		public override IReadOnlyList<GenericArgumentDefinition> GenericArguments { get; }
	}
}
