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
		public abstract IReadOnlyList<GenericArgumentDefinition> GenericArguments { get; }

		public string GetFullyQualifiedName()
		{
			return $"{(Module != null ? $"{Module}:" : "")}{Name}{(GenericArguments.Count > 0 ? $"<{string.Join(", ", GenericArguments.Select(a => a.Name))}>" : "")}";
		}

		public override string ToString()
		{
			return GetFullyQualifiedName();
		}
	}
}
