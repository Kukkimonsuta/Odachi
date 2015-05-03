using Microsoft.AspNet.Authentication;

namespace Odachi.Security.BasicAuthentication
{
    /// <summary>
    /// Contains the options used by the <see cref="BasicAuthenticationMiddleware"/>.
    /// </summary>
    public class BasicAuthenticationOptions : AuthenticationOptions
    {
        public BasicAuthenticationOptions()
        {
			this.AuthenticationScheme = "Basic";
			this.AutomaticAuthentication = true;
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