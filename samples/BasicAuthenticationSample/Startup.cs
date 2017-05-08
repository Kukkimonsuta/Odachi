using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Odachi.AspNetCore.Authentication.Basic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;

namespace BasicAuthenticationSample
{
    public class Startup
    {
		public Startup(IHostingEnvironment hostingEnvironment)
		{
			Configuration = new ConfigurationBuilder()
				.SetBasePath(hostingEnvironment.ContentRootPath)
				.AddJsonFile("config.json")
				.Build();

			Configuration.GetSection("BasicAuthentication").Bind(BasicOptions);
		}

		public IConfigurationRoot Configuration { get; set; }
		public BasicOptions BasicOptions { get; set; } = new BasicOptions();

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
        {
			services.AddAuthentication();
		}

		// this example shows how to configure basic authentication using IOptions
		public void Run_Simple(IApplicationBuilder app)
		{
			app.UseBasicAuthentication(BasicOptions);

			app.Run(async (context) =>
			{
				if (!context.User.Identity.IsAuthenticated)
					await context.Authentication.ChallengeAsync();
				else
					await context.Response.WriteAsync($"Hello {context.User.Identity.Name}! (simple)");
			});
		}

		// this example shows how to use custom authentication logic
		public void Run_CustomAuthenticationLogic(IApplicationBuilder app)
		{
			app.UseBasicAuthentication(options =>
			{
				options.Realm = "Custom authentication logic";
				options.Events = new BasicEvents()
				{
					OnSignIn = context =>
					{
						// instead of hardcoded logic, you could also obtain your services that handle authentication
						// from the container by using `app.ApplicationServices.GetService` and use those

						if (context.Username == "admin" && context.Password == "1234")
						{
							var claims = new[]
							{
								new Claim(ClaimTypes.Name, "administrator")
							};

							// note that ClaimsIdentity is considered "authenticated" only if it has an "authenticationType"
							// returning an unauthenticated principal will in this case result in 403 Forbidden
							// returning null will act in this case as if there were no credentials submitted and user will be asked again
							context.Ticket = new AuthenticationTicket(
								new ClaimsPrincipal(new ClaimsIdentity(claims, context.Options.AuthenticationScheme)),
								new AuthenticationProperties(),
								context.Options.AuthenticationScheme
							);

							// mark response as handled
							//	AuthenticationTicket != null -> success
							//  AuthenticationTicket == null -> fail
							context.HandleResponse();
						}

						return Task.FromResult(0);
					}
				};
			});

			app.Run(async (context) =>
			{
				if (!context.User.Identity.IsAuthenticated)
					await context.Authentication.ChallengeAsync();
				else
					await context.Response.WriteAsync($"Hello {context.User.Identity.Name}! (complex)");
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

			app.UseStatusCodePages();
			if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

			app.Map("/simple", Run_Simple);
			app.Map("/custom-authentication-logic", Run_CustomAuthenticationLogic);
			app.Run(async (context) =>
			{
				await context.Response.WriteAsync(@"
					<a href=""/simple"">Simple</a>
					<a href=""/custom-authentication-logic"">Custom authentication logic</a>
				");
			});
        }
    }
}
