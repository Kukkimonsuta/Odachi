using System.Security.Claims;

namespace Odachi.Security.BasicAuthentication
{
    /// <summary>
    /// Interface used by <see cref="BasicAuthenticationMiddleware"/> to authenticate users.
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// Attempts to authenticate credentials and return user identity. Returns null if authentication fails.
        /// </summary>
        /// <param name="userName">User name</param>
        /// <param name="password">Password</param>
        /// <returns>An instance of <see cref="ClaimsIdentity"/> or null in case of authentication failure.</returns>
        ClaimsIdentity Authenticate(string userName, string password);
    }
}