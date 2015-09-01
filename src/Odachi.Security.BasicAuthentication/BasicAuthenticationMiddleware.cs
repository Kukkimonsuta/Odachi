using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Builder;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.WebEncoders;

namespace Odachi.Security.BasicAuthentication
{
    /// <summary>
    /// Middleware for basic authentication.
    /// </summary>
    public class BasicAuthenticationMiddleware : AuthenticationMiddleware<BasicAuthenticationOptions>
    {
        public BasicAuthenticationMiddleware(
            RequestDelegate next,
            IOptions<BasicAuthenticationOptions> options,
            ILoggerFactory loggerFactory,
            IUrlEncoder encoder,
            ConfigureOptions<BasicAuthenticationOptions> configureOptions)
            : base(next, options, loggerFactory, encoder, configureOptions)
        {
            if (string.IsNullOrEmpty(Options.Realm))
                Options.Realm = BasicAuthenticationDefaults.Realm;
        }

        protected override AuthenticationHandler<BasicAuthenticationOptions> CreateHandler()
        {
            return new BasicAuthenticationHandler();
        }
    }
}