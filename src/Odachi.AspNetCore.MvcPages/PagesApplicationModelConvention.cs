using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Odachi.AspNetCore.MvcPages
{
    public class PagesApplicationModelConvention : IApplicationModelConvention
    {
		private T Consume<T>(IReadOnlyList<T> array, ref int index)
		{
			return array[index++];
		}

		private T Consume<T>(IReadOnlyList<T> array, ref int index, Func<T, bool> predicate)
		{
			while (!End(array, index))
			{
				var candidate = Consume(array, ref index);

				if (predicate(candidate))
					return candidate;
			}

			return default(T);
		}

		private IEnumerable<T> ConsumeMany<T>(IReadOnlyList<T> array, ref int index)
		{
			var result = array.Skip(index).Take(int.MaxValue);
			index = array.Count;
			return result;
		}

		private bool End<T>(IReadOnlyList<T> array, int index)
		{
			return index >= array.Count;
		}

		#region IApplicationModelConvention

		public void Apply(ApplicationModel model)
        {
            foreach (var controller in model.Controllers)
            {
                var ns = controller.ControllerType.Namespace.Split('.');
				if (ns.Length < 4)
					continue;

				// expected namespace:
				// ?????(.????)?+		// prefix (ignore)
				// Areas				// string "Areas"
				// ?????				// Area name
				// App					// string "App"
				// ?????(.????)?+		// controller name

				var index = 0;

				var areas = Consume(ns, ref index, s => string.Equals(s, "areas", StringComparison.OrdinalIgnoreCase));
				if (End(ns, index))
					continue;

				var areaName = Consume(ns, ref index);
				if (End(ns, index))
					continue;

				var app = Consume(ns, ref index);
				if (End(ns, index) || !string.Equals(app, "app", StringComparison.OrdinalIgnoreCase))
					continue;

				var controllerName = string.Join("_", ConsumeMany(ns, ref index));
				var actionName = controller.ControllerName;


				controller.RouteValues.Add("area", areaName);
				controller.ControllerName = controllerName;

				foreach (var action in controller.Actions)
					action.ActionName = actionName;
			}
        }

        #endregion
    }
}