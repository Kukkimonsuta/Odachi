using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Odachi.CodeModel.Mapping
{
	/// <summary>
	/// Represents a type reference.
	/// </summary>
	public interface ITypeReference
	{
		bool IsNullable { get; set; }

		TypeReference Resolve(TypeMapper mapper);
	}
}
