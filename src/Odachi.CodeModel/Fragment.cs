using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.CodeModel
{
	/// <summary>
	/// Base class for all schema building blocks.
	/// </summary>
	public abstract class Fragment
	{
		public string Name { get; set; }

		public IDictionary<string, string> Hints { get; } = new Dictionary<string, string>();
	}

	/// <summary>
	/// Base class for all types (enums, objects, services).
	/// </summary>
	public abstract class TypeFragment : Fragment
	{
		public string ModuleName { get; set; }
	}
}
