using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.CodeModel
{
	/// <summary>
	/// Represents root folder.
	/// </summary>
	public class Package
	{
		/// <summary>
		/// Package name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Package modules.
		/// </summary>
		public IList<Module> Modules { get; } = new List<Module>();
		
		/// <summary>
		/// Hints.
		/// </summary>
		public IDictionary<string, string> Hints { get; } = new Dictionary<string, string>();
	}
}
