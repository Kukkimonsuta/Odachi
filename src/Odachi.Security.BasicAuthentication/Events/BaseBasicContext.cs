using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Security.BasicAuthentication
{
	public class BaseBasicContext : BaseControlContext
	{
		public BaseBasicContext(HttpContext context, BasicOptions options)
			: base(context)
		{
			if (options == null)
				throw new ArgumentNullException(nameof(options));

			Options = options;
		}

		public BasicOptions Options { get; }
	}
}
