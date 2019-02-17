using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Mapping;

namespace Odachi.CodeModel
{
	/// <summary>
	/// Represents an enum.
	/// </summary>
	public class EnumFragment : TypeFragment
	{
		public IList<EnumItemFragment> Items { get; } = new List<EnumItemFragment>();
	}
}
