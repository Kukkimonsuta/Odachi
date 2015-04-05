using Microsoft.Framework.ConfigurationModel;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Razor;
using Odachi.MvcPages;

namespace Microsoft.Framework.DependencyInjection
{
	public static class MvcPagesServiceCollectionExtensions
	{
		public static IServiceCollection AddMvcPages(this IServiceCollection services, IConfiguration configuration = null)
		{
			services
				.AddMvc(configuration);

			services
				.Configure<MvcOptions>(o =>
				{
					o.Filters.Add(new AuthorizeAttribute());
					o.ApplicationModelConventions.Add(new PagesApplicationModelConvention());
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