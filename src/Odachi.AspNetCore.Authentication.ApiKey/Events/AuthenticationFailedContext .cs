using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;

namespace Odachi.AspNetCore.Authentication.ApiKey
{
    /// <summary>
    /// Context object used to control flow of api key authentication.
    /// </summary>
    public class AuthenticationFailedContext : ResultContext<ApiKeyOptions>
    {
        /// <summary>
        /// Creates a new instance of the context object.
        /// </summary>
        public AuthenticationFailedContext(HttpContext context, AuthenticationScheme scheme, ApiKeyOptions options)
            : base(context, scheme, options)
        {
        }

        /// <summary>
        /// The exception thrown.
        /// </summary>
        public Exception Exception { get; set; }
    }
}
