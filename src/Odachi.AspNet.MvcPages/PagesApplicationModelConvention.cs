using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModels;

namespace Odachi.AspNet.MvcPages
{
    public class PagesApplicationModelConvention : IApplicationModelConvention
    {
        #region IApplicationModelConvention

        public void Apply(ApplicationModel model)
        {
            foreach (var controller in model.Controllers)
            {
                var ns = controller.ControllerType.Namespace.Split('.');
				if (ns.Length < 4)
					continue;

				var actionName = controller.ControllerName;
				var areas = ns[ns.Length - 4];
				var areaName = ns[ns.Length - 3];
				var app = ns[ns.Length - 2];
				var controllerName = ns[ns.Length - 1];

				if (!string.Equals(areas, "areas", System.StringComparison.OrdinalIgnoreCase) || !string.Equals(app, "app", System.StringComparison.OrdinalIgnoreCase))
					continue;

				controller.RouteConstraints.Add(new AreaAttribute(areaName));
				controller.ControllerName = controllerName;

				foreach (var action in controller.Actions)
					action.ActionName = actionName;
			}
        }

        #endregion
    }
}