using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.CodeModel
{
	/// <summary>
	/// Represents a method.
	/// </summary>
	public class MethodFragment : Fragment
	{
		public TypeReference ReturnType { get; set; }

		public IList<ParameterFragment> Parameters { get; } = new List<ParameterFragment>();
	}
}
