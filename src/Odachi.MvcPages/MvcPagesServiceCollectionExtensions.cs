using Microsoft.Framework.ConfigurationModel;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Razor;
using Odachi.MvcPages;

namespace Microsoft.Framework.DependencyInjection
{
	public static class MvcPagesServiceCollectionExtensions
	{
		public static IServiceCollection AddMvcPages(this IServiceCollection services)
		{
			services
				.AddMvc();

			services
				.Configure<MvcOptions>(o =>
				{
					o.Conventions.Add(new PagesApplicationModelConvention());
				});

			services
				.Configure<RazorViewEngineOptions>(o =>
				{
					o.ViewLocationExpanders.Add(new PagesViewLocationExpander());
				});

			return services;
		}
	}
}