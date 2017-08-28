using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.CodeModel.Mapping
{
	public abstract class TypeDefinition
	{
		public abstract string Module { get; }
		public abstract string Name { get; }
		public abstract IReadOnlyList<GenericArgumentDefinition> GenericArgumentDefinitions { get; }

		public string GetFullyQualifiedName()
		{
			return $"{(Module != null ? $"{Module}:" : "")}{Name}{(GenericArgumentDefinitions.Count > 0 ? $"<{string.Join(", ", GenericArgumentDefinitions.Select(a => a.Name))}>" : "")}";
		}

		public override string ToString()
		{
			return GetFullyQualifiedName();
		}
	}
}
