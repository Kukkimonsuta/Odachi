using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.CodeModel
{
	/// <summary>
	/// Represents a SDK.
	/// </summary>
	public class Package
	{
		/// <summary>
		/// Package name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Objects.
		/// </summary>
		public IList<ObjectFragment> Objects { get; } = new List<ObjectFragment>();

		/// <summary>
		/// Enums.
		/// </summary>
		public IList<EnumFragment> Enums { get; } = new List<EnumFragment>();

		/// <summary>
		/// Services.
		/// </summary>
		public IList<ServiceFragment> Services { get; } = new List<ServiceFragment>();

		/// <summary>
		/// Hints.
		/// </summary>
		public IDictionary<string, string> Hints { get; } = new Dictionary<string, string>();
	}
}
