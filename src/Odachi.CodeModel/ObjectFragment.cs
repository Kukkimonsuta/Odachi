using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Mapping;

namespace Odachi.CodeModel
{
	/// <summary>
	/// Represents an object.
	/// </summary>
	public class ObjectFragment : TypeFragment
	{
		public override string Kind => "object";

		public IReadOnlyList<GenericArgumentDefinition> GenericArguments { get; set; } = Array.Empty<GenericArgumentDefinition>();

		public IList<ConstantFragment> Constants { get; } = new List<ConstantFragment>();
		public IList<FieldFragment> Fields { get; } = new List<FieldFragment>();
	}
}
