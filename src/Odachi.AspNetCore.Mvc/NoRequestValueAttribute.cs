using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace Odachi.AspNetCore.Mvc
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
