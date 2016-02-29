using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.AspNetCore.MvcPages
{
	public abstract class AreaStartup
	{
		public abstract PathString Prefix { get; }

		public abstract void Configure(IApplicationBuilder app);
	}

    public static class MapAreaExtensions
    {
		public static IApplicationBuilder MapArea<T>(this IApplicationBuilder app)
			where T : AreaStartup, new()
		{
			if (app == null)
				throw new ArgumentNullException(nameof(app));

			var startup = new T();

			// create branch
			var branchBuilder = app.New();
			startup.Configure(branchBuilder);

			var branch = branchBuilder.Build();

			// put middleware in pipeline
			return app.Use(next => new MapAreaMiddleware(next, startup.Prefix, branch).Invoke);
		}
    }
}
