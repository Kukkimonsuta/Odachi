using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Security.Infrastructure;
using Microsoft.Framework.OptionsModel;
using System;

namespace Odachi.Security.BasicAuthentication
{
    /// <summary>
    /// Middleware for basic authentication.
    /// </summary>
    public class BasicAuthenticationMiddleware : AuthenticationMiddleware<BasicAuthenticationOptions>
    {
        public BasicAuthenticationMiddleware(
            RequestDelegate next,
            IServiceProvider services,
            IOptions<BasicAuthenticationOptions> options,
            ConfigureOptions<BasicAuthenticationOptions> configureOptions)
            : base(next, services, options, configureOptions)
        {
            if (string.IsNullOrEmpty(Options.Realm))
                Options.Realm = BasicAuthenticationDefaults.Realm;

            if (Options.Authenticator == null)
                throw new ArgumentException("Options must contain an authenticator");
        }

        protected override AuthenticationHandler<BasicAuthenticationOptions> CreateHandler()
        {
            return new BasicAuthenticationHandler();
        }
    }
}