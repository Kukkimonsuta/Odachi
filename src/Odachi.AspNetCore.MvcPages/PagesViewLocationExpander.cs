using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;

namespace Odachi.AspNetCore.MvcPages
{
    public class PagesViewLocationExpander : IViewLocationExpander
    {
        #region IViewLocationExpander

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
			return new[]
			{
				"/Areas/{2}/App/{1}/{0}.cshtml",
				"/Areas/{2}/App/{0}.cshtml",
			};
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        #endregion
    }
}