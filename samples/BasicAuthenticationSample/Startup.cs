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
		public const string BasicScheme = "Basic";
		public const string CustomBasicScheme = "CustomBasic";

		public Startup(IHostingEnvironment hostingEnvironment)
		{
			Configuration = new ConfigurationBuilder()
				.SetBasePath(hostingEnvironment.ContentRootPath)
				.AddJsonFile("config.json")
				.Build();
		}

		public IConfigurationRoot Configuration { get; set; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAuthentication()
				.AddBasic(BasicScheme, options => Configuration.GetSection("BasicAuthentication").Bind(options))
				.AddBasic(CustomBasicScheme, options =>
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

								context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));

								context.Success();
							}

							return Task.CompletedTask;
						}
					};
				});
		}

		// this example shows how to configure basic authentication using IOptions
		public void Run_Simple(IApplicationBuilder app)
		{
			app.Run(async (context) =>
			{
				var result = await context.AuthenticateAsync(BasicScheme);
				context.User = result != null && result.Succeeded ? result.Principal : new ClaimsPrincipal(new ClaimsIdentity());

				if (!context.User.Identity.IsAuthenticated)
				{
					await context.ChallengeAsync(BasicScheme);
				}
				else
				{
					await context.Response.WriteAsync($"Hello {context.User.Identity.Name}! (simple)");
				}
			});
		}

		// this example shows how to use custom authentication logic
		public void Run_CustomAuthenticationLogic(IApplicationBuilder app)
		{
			app.Run(async (context) =>
			{
				var result = await context.AuthenticateAsync(CustomBasicScheme);
				context.User = result != null && result.Succeeded ? result.Principal : new ClaimsPrincipal(new ClaimsIdentity());

				if (!context.User.Identity.IsAuthenticated)
				{
					await context.ChallengeAsync(CustomBasicScheme);
				}
				else
				{
					await context.Response.WriteAsync($"Hello {context.User.Identity.Name}! (complex)");
				}
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

			app.UseAuthentication();

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
