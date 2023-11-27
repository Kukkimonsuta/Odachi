using Microsoft.AspNetCore.Authentication;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Odachi.AspNetCore.Authentication.ApiKey
{
	internal class ApiKeyHandler : AuthenticationHandler<ApiKeyOptions>
	{
		public const string RequestHeaderPrefix = "ApiKey ";

		public ApiKeyHandler(IOptionsMonitor<ApiKeyOptions> options, ILoggerFactory logger, UrlEncoder encoder)
			: base(options, logger, encoder)
		{
		}

		/// <summary>
		/// The handler calls methods on the events which give the application control at certain points where processing is occurring.
		/// If it is not provided a default instance is supplied which does nothing when the methods are called.
		/// </summary>
		protected new ApiKeyEvents Events
		{
			get { return (ApiKeyEvents)base.Events; }
			set { base.Events = value; }
		}

		protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new ApiKeyEvents());

		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			try
			{
				// retrieve authorization header
				string authorization = Request.Headers[HeaderNames.Authorization];

				if (string.IsNullOrEmpty(authorization))
				{
					return AuthenticateResult.NoResult();
				}

				if (!authorization.StartsWith(RequestHeaderPrefix, StringComparison.OrdinalIgnoreCase))
				{
					return AuthenticateResult.NoResult();
				}

				// retrieve credentials from header
				var apiKey = authorization.Substring(RequestHeaderPrefix.Length);

				// invoke sign in event
				var signInContext = new ApiKeySignInContext(Context, Scheme, Options)
				{
					ApiKey = apiKey,
				};
				await Events.SignIn(signInContext);

				if (signInContext.Result != null)
				{
					return signInContext.Result;
				}

				// allow sign in event to modify received credentials
				apiKey = signInContext.ApiKey;

				// verify credentials against options
				ApiKeyCredential credentials = null;
				for (var i = 0; i < Options.Credentials.Length; i++)
				{
					var currentCredentials = Options.Credentials[i];

					if (currentCredentials.ApiKey == apiKey)
					{
						credentials = currentCredentials;
						break;
					}
				}
				if (credentials == null)
				{
					return AuthenticateResult.Fail("Invalid api key authentication credentials.");
				}

				var claims = new Claim[credentials.Claims.Length + 1];
				claims[0] = new Claim(ClaimsIdentity.DefaultNameClaimType, credentials.ApiKey);
				for (var i = 0; i < credentials.Claims.Length; i++)
				{
					var currentClaim = credentials.Claims[i];

					claims[i + 1] = new Claim(currentClaim.Type, currentClaim.Value);
				}

				var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));

				var ticket = new AuthenticationTicket(principal, new AuthenticationProperties(), Scheme.Name);

				return AuthenticateResult.Success(ticket);
			}
			catch (Exception ex)
			{
				var authenticationFailedContext = new AuthenticationFailedContext(Context, Scheme, Options)
				{
					Exception = ex,
				};

				await Events.AuthenticationFailed(authenticationFailedContext);
				if (authenticationFailedContext.Result != null)
				{
					return authenticationFailedContext.Result;
				}

				throw;
			}
		}

		protected override Task HandleChallengeAsync(AuthenticationProperties properties)
		{
			Response.StatusCode = 401;
			Response.Headers.Append(HeaderNames.WWWAuthenticate, $"ApiKey");

			return Task.CompletedTask;
		}
	}
}
