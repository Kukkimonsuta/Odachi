using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;

namespace Odachi.AspNetCore.Mvc
{
	public class RequireRequestValueAttribute : ActionMethodSelectorAttribute
	{
		public RequireRequestValueAttribute(string name, string value = null)
		{
			Name = name;
			Value = value;
		}

		public string Name { get; }
		public string Value { get; }

		public StringComparison ComparisonType { get; } = StringComparison.OrdinalIgnoreCase;

		private bool ValueIsValid(object value)
		{
			return ValueIsValid(value?.ToString());
		}
		private bool ValueIsValid(string value)
		{
			if (Value == null)
			{
				// all values are accepted
				return true;
			}

			return string.Equals(value, Value, ComparisonType);
		}

		public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
		{
			var value = default(object);

			if (routeContext.RouteData.Values.TryGetValue(Name, out value) && ValueIsValid(value))
				return true;

			if (routeContext.RouteData.DataTokens.TryGetValue(Name, out value) && ValueIsValid(value))
				return true;

			if (routeContext.HttpContext.Request.Query.ContainsKey(Name))
			{
				var values = routeContext.HttpContext.Request.Query[Name];
				if (values.Count <= 0)
				{
					if (ValueIsValid(null))
						return true;
				}
				else if (values.Any(v => ValueIsValid(v)))
					return true;
			}

			return false;
		}
	}
}
