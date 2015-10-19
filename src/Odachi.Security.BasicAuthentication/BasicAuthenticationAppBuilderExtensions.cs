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
        /// Adds a basic authentication middleware to your web application pipeline.
        /// </summary>
        /// <param name="app">The IApplicationBuilder passed to your configuration method</param>
        /// <returns>The original app parameter</returns>
        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            return app.UseBasicAuthentication(new BasicAuthenticationOptions());
        }

        /// <summary>
        /// Adds a basic authentication middleware to your web application pipeline.
        /// </summary>
        /// <param name="app">The IApplicationBuilder passed to your configuration method</param>
        /// <param name="configureOptions">Used to configure the options for the middleware</param>
        /// <returns>The original app parameter</returns>
        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app, Action<BasicAuthenticationOptions> configureOptions)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            var options = new BasicAuthenticationOptions();
            if (configureOptions != null)
                configureOptions(options);

            return app.UseBasicAuthentication(options);
        }

        /// <summary>
        /// Adds a basic authentication middleware to your web application pipeline.
        /// </summary>
        /// <param name="app">The IApplicationBuilder passed to your configuration method</param>
        /// <param name="options">Used to configure the middleware</param>
        /// <returns>The original app parameter</returns>
        public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app, BasicAuthenticationOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return app.UseMiddleware<BasicAuthenticationMiddleware>(options);
        }
    }
}