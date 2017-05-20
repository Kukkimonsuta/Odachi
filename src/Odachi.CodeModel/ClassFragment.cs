using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Mapping;

namespace Odachi.CodeModel
{
	/// <summary>
	/// Represents a class.
	/// </summary>
	public class ClassFragment : TypeFragment
	{
		public override string Kind => "class";

		public IList<FieldFragment> Fields { get; } = new List<FieldFragment>();

		public IList<MethodFragment> Methods { get; } = new List<MethodFragment>();
	}
}
