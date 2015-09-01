using Microsoft.AspNet.Mvc.Razor;
using System.Collections.Generic;

namespace Odachi.MvcPages
{
    public class PagesViewLocationExpander : IViewLocationExpander
    {
        #region IViewLocationExpander

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            return new[]
            {
                "/{2}/App/{1}/{0}.cshtml",
                "/{2}/App/Shared/{0}.cshtml",
                "/{2}/App/{0}.cshtml",
            };
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        #endregion
    }
}