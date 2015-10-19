using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Http.Features.Authentication;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Odachi.Security.BasicAuthentication
{
    internal class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        public const string RequestHeader = "Authorization";
        public const string ChallengeHeader = "WWW-Authenticate";

        public BasicAuthenticationHandler()
        {
        }

        protected override Task<AuthenticationTicket> HandleAuthenticateAsync()
        {
            AuthenticationTicket ticket = null;
            try
            {
                var headers = Request.Headers[RequestHeader];
                if (headers.Count <= 0)
                    return Task.FromResult<AuthenticationTicket>(null);

                var header = headers.Where(h => h.StartsWith("Basic ")).FirstOrDefault();

                if (header == null)
                    return Task.FromResult<AuthenticationTicket>(null);

                var encoded = header.Substring(6);
                var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));

                var index = decoded.IndexOf(':');
                if (index == -1)
                    return Task.FromResult<AuthenticationTicket>(null);

                var username = decoded.Substring(0, index);
                var password = decoded.Substring(index + 1);

                var signInContext = new BasicSignInContext(Context, Options, username, password);
                Options.Events.SignIn(signInContext);
                if (signInContext.Principal == null)
                    return Task.FromResult<AuthenticationTicket>(null);

                ticket = new AuthenticationTicket(signInContext.Principal, new AuthenticationProperties(), Options.AuthenticationScheme);

                return Task.FromResult(ticket);
            }
            catch (Exception ex)
            {
                var exceptionContext = new BasicExceptionContext(Context, Options, ex, ticket);
                Options.Events.Exception(exceptionContext);
                if (exceptionContext.Rethrow)
                {
                    throw;
                }
                return Task.FromResult(exceptionContext.Ticket);
            }
        }

        protected override Task<bool> HandleUnauthorizedAsync(ChallengeContext context)
        {
            Response.Headers[ChallengeHeader] = "Basic realm=\"" + Options.Realm + "\"";

            return base.HandleUnauthorizedAsync(context);
        }
    }
}