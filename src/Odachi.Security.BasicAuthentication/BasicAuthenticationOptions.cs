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
            AuthenticationScheme = BasicAuthenticationDefaults.AuthenticationScheme;
            AutomaticAuthentication = true;
            Notifications = new BasicAuthenticationNotifications();
        }

        /// <summary>
        /// The default realm used by basic authentication.
        /// </summary>
        public string Realm { get; set; } = BasicAuthenticationDefaults.Realm;

        /// <summary>
        /// The Provider may be assigned to an instance of an object created by the application at startup time. The middleware
        /// calls methods on the provider which give the application control at certain points where processing is occuring.
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        public IBasicAuthenticationNotifications Notifications { get; set; }
    }
}