using Microsoft.Framework.OptionsModel;
using Odachi.Security.BasicAuthentication;
using System;

namespace Microsoft.AspNet.Builder
{
    /// <summary>
    /// Basic authentication extensions for <see cref="IApplicationBuilder"/>.
    /// </summary>
    public static class BasicAuthenticationAppBuilderExtensions
    {
        /// <summary>
        /// Enables basic authentication for the current application.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> instance this method extends.</param>
        /// <returns>The <see cref="IApplicationBuilder"/> instance this method extends.</returns>
        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app, Action<BasicAuthenticationOptions> configureOptions = null, string optionsName = "")
        {
            return app.UseMiddleware<BasicAuthenticationMiddleware>(
                new ConfigureOptions<BasicAuthenticationOptions>(configureOptions ?? (o => { }))
                {
                    Name = optionsName
                }
            );
        }
    }
}