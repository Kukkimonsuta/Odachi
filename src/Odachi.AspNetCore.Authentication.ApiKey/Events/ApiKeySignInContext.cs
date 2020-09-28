using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Odachi.AspNetCore.Authentication.ApiKey
{
    /// <summary>
    /// Context object used to control flow of api key authentication.
    /// </summary>
    public class ApiKeySignInContext : ResultContext<ApiKeyOptions>
    {
        /// <summary>
        /// Creates a new instance of the context object.
        /// </summary>
        public ApiKeySignInContext(HttpContext context, AuthenticationScheme scheme, ApiKeyOptions options)
            : base(context, scheme, options)
        {
        }

        /// <summary>
        /// Contains the api key used in api key authentication.
        /// </summary>
        public string ApiKey { get; set; }
    }
}
