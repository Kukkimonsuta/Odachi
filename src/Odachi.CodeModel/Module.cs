using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.CodeModel
{
	/// <summary>
	/// Represents a file in package.
	/// </summary>
	public class Module
	{
		/// <summary>
		/// Module name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Module fragments.
		/// </summary>
		public IList<TypeFragment> Fragments { get; } = new List<TypeFragment>();

		/// <summary>
		/// Hints.
		/// </summary>
		public IDictionary<string, string> Hints { get; } = new Dictionary<string, string>();
	}
}
