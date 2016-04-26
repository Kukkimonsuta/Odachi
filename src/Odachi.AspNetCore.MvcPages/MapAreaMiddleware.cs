using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.AspNetCore.MvcPages
{
    public class MapAreaMiddleware
    {
		public MapAreaMiddleware(RequestDelegate next, PathString prefix, RequestDelegate branch)
		{
			if (next == null)
				throw new ArgumentNullException(nameof(next));
			if (branch == null)
				throw new ArgumentNullException(nameof(branch));

			_next = next;
			_prefix = prefix;
			_branch = branch;

			_isFallback = !_prefix.HasValue || _prefix.Value == "/";
		}

		private RequestDelegate _next;
		private PathString _prefix;
		private RequestDelegate _branch;

		private bool _isFallback;

		public async Task Invoke(HttpContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			if (_isFallback || context.Request.Path.StartsWithSegments(_prefix))
				await _branch(context);
			else
				await _next(context);
		}
	}
}
