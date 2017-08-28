using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.CodeModel
{
	/// <summary>
	/// Represents part of module.
	/// </summary>
	public abstract class Fragment
	{
		public string Name { get; set; }

		public IDictionary<string, string> Hints { get; } = new Dictionary<string, string>();
	}

	/// <summary>
	/// Represents a fragment containing type.
	/// </summary>
	public abstract class TypeFragment : Fragment
	{
		public abstract string Kind { get; }
	}
}
