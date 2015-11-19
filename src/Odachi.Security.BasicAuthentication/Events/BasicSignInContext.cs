using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Http;
using System.Security.Claims;

namespace Odachi.Security.BasicAuthentication
{
    /// <summary>
    /// Context object used to control flow of basic authentication.
    /// </summary>
    public class BasicSignInContext : BaseBasicContext
    {
        /// <summary>
        /// Creates a new instance of the context object.
        /// </summary>
        /// <param name="context">The HTTP request context</param>
        /// <param name="options">The middleware options</param>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        public BasicSignInContext(
            HttpContext context,
            BasicOptions options,
            string username,
            string password
        )
            : base(context, options)
        {
            Username = username;
            Password = password;
        }

        /// <summary>
        /// Contains the username used in basic authentication.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Contains the password used in basic authentication.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Contains the principal that will be returned to the application.
        /// </summary>
        public ClaimsPrincipal Principal { get; set; }
    }
}