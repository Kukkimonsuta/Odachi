using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Abstractions
{
	[Obsolete("This only causes trouble for serialization/deserialization scenarios and brings no real benefit. Will be removed in next major version.")]
	public interface IEntityReference
	{
		int Id { get; }
	}
}
