using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;
using Odachi.Security.BasicAuthentication;
using Odachi.Security.BasicAuthentication.Notifications;
using System.Security.Claims;
using Microsoft.AspNet.Diagnostics;

namespace BasicAuthenticationSample
{
	public class Startup
	{
		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAuthentication();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseErrorPage();

			app.UseBasicAuthentication(options =>
			{
				options.AutomaticAuthentication = true;
				options.Notifications = new BasicAuthenticationNotifications()
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
							context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Options.AuthenticationScheme));
						}
					}
				};
			});

			app.Run(async (context) =>
			{
				if (!context.User.Identity.IsAuthenticated)
					await context.Authentication.ChallengeAsync();
				else
					await context.Response.WriteAsync($"Hello {context.User.Identity.Name} !");
			});
		}
	}
}
