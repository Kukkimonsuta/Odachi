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
                var actionName = controller.ControllerName;

                var ns = controller.ControllerType.Namespace;
                var index = ns.LastIndexOf('.');
                var controllerName = ns.Substring(index + 1, ns.Length - index - 1);

                foreach (var action in controller.Actions)
                    action.ActionName = actionName;

                controller.ControllerName = controllerName;
            }
        }

        #endregion
    }
}