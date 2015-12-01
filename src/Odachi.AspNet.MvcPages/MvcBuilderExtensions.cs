using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Odachi.AspNet.MvcPages;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcBuilderExtensions
	{
        public static IMvcBuilder AddPages(this IMvcBuilder mvcBuilder)
        {
			mvcBuilder.Services
                .Configure<MvcOptions>(o =>
                {
                    o.Conventions.Add(new PagesApplicationModelConvention());
                })
                .Configure<RazorViewEngineOptions>(o =>
                {
                    o.ViewLocationExpanders.Add(new PagesViewLocationExpander());
                });

            return mvcBuilder;
        }
    }
}