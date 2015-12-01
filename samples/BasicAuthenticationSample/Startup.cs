using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Odachi.AspNet.Authentication.Basic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.OptionsModel;
using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Http.Authentication;

namespace BasicAuthenticationSample
{
	public class Startup
	{
		public Startup(IHostingEnvironment hostingEnvironment, IApplicationEnvironment applicationEnvironment)
		{
			Configuration = new ConfigurationBuilder()
				.SetBasePath(applicationEnvironment.ApplicationBasePath)
				.AddJsonFile("config.json")
				.Build();
		}

		public IConfigurationRoot Configuration { get; set; }

		// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<BasicOptions>(Configuration.GetSection("BasicAuthentication"));

			services.AddAuthentication();
		}

		public void Run_Simple(IApplicationBuilder app)
		{
			// this example shows how to configure basic authentication using IOptions

			app.UseBasicAuthentication();

			app.Run(async (context) =>
			{
				if (!context.User.Identity.IsAuthenticated)
					await context.Authentication.ChallengeAsync();
				else
					await context.Response.WriteAsync($"Hello {context.User.Identity.Name}! (simple)");
			});
		}

		public void Run_CustomAuthenticationLogic(IApplicationBuilder app)
		{
			// this example shows how to use custom authentication logic

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
							context.AuthenticationTicket = new AuthenticationTicket(
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

		public void Configure(IApplicationBuilder app)
		{
			app.UseStatusCodePages();
			app.UseDeveloperExceptionPage();
			app.UseIISPlatformHandler();

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