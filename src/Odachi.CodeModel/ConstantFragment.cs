using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.CodeModel
{
	/// <summary>
	/// Represents a constant.
	/// </summary>
	public class ConstantFragment : Fragment
	{
		public TypeReference Type { get; set; }

		public object Value { get; set; }
	}
}
