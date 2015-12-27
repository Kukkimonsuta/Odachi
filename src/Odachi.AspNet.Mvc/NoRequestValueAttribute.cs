using Microsoft.AspNet.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ActionConstraints;
using Microsoft.AspNet.Routing;
using Microsoft.Extensions.Primitives;

namespace Odachi.AspNet.Mvc
{
	public class NoRequestValueAttribute : ActionMethodSelectorAttribute
	{
		public NoRequestValueAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; }

		public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
		{
			if (routeContext.RouteData.Values.ContainsKey(Name))
				return false;

			if (routeContext.RouteData.DataTokens.ContainsKey(Name))
				return false;

			if (routeContext.HttpContext.Request.Query.ContainsKey(Name))
				return false;

			return true;
		}
	}
}
