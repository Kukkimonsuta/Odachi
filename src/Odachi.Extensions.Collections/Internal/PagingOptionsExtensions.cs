using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Extensions.Collections.Internal;

public static class PagingOptionsExtensions
{
	public static int GetSkipCount(this PagingOptions options)
	{
		return Math.Max(0, options.Number * options.Size - options.Offset);
	}

	public static int GetTakeCount(this PagingOptions options)
	{
		// get offset on selected page
		var offset = Math.Max(0, options.Offset - options.Number * options.Size);

		return Math.Max(0, options.Size - offset);
	}
}
