using Microsoft.AspNet.Security;
using System;
using System.Security.Claims;

namespace Odachi.Security.BasicAuthentication
{
    /// <summary>
    /// Contains the options used by the <see cref="BasicAuthenticationMiddleware"/>.
    /// </summary>
    public class BasicAuthenticationOptions : AuthenticationOptions
    {
        public BasicAuthenticationOptions()
        {
            AuthenticationMode = AuthenticationMode.Active;
            AuthenticationType = "Basic";
        }

        /// <summary>
        /// The default realm used by basic authentication.
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        /// And instance of <see cref="IAuthenticator"/> that should be used for user authentication.
        /// </summary>
        public IAuthenticator Authenticator { get; set; }
    }
}