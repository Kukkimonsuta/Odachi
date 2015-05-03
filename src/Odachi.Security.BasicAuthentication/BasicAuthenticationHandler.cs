using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Authentication;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Security.BasicAuthentication
{
    internal class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOptions>
    {
        public BasicAuthenticationHandler()
        {
        }

        protected override AuthenticationTicket AuthenticateCore()
        {
            return AuthenticateCoreAsync().GetAwaiter().GetResult();
        }

        protected override Task<AuthenticationTicket> AuthenticateCoreAsync()
        {
            var header = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(header))
                return Task.FromResult<AuthenticationTicket>(null);

            if (!header.StartsWith("Basic "))
                return Task.FromResult<AuthenticationTicket>(null);

            var encoded = header.Substring(6);
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));

            var index = decoded.IndexOf(':');
            if (index == -1)
                return Task.FromResult<AuthenticationTicket>(null);

            var username = decoded.Substring(0, index);
            var password = decoded.Substring(index + 1);

            var identity = Options.Authenticator.Authenticate(username, password);
            if (identity != null)
            {
                return Task.FromResult(new AuthenticationTicket(
                    new ClaimsPrincipal(identity),
					new AuthenticationProperties(),
                    Options.AuthenticationScheme
                ));
            }

            return Task.FromResult<AuthenticationTicket>(null);
        }

        protected override void ApplyResponseChallenge()
        {
            if (Response.StatusCode != 401)
                return;

            Response.Headers["WWW-Authenticate"] = "Basic realm=\"" + Options.Realm + "\"";
        }

        protected override void ApplyResponseGrant()
        {
        }
    }
}