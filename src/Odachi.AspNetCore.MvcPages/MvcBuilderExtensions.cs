using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Odachi.AspNetCore.MvcPages;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcBuilderExtensions
	{
        public static IMvcBuilder AddMvcPages(this IMvcBuilder mvcBuilder)
        {
			mvcBuilder
				.AddMvcOptions(o =>
                {
                    o.Conventions.Add(new PagesApplicationModelConvention());
                })
                .AddRazorOptions(o =>
                {
                    o.ViewLocationExpanders.Add(new PagesViewLocationExpander());
                });

            return mvcBuilder;
        }
	}
}