namespace Odachi.Security.BasicAuthentication
{
    /// <summary>
    /// Specifies callback methods which the <see cref="BasicAuthenticationMiddleware"></see> invokes to enable developer control over the authentication process.
    /// </summary>
    public interface IBasicAuthenticationNotifications
    {
        /// <summary>
        /// Called when a request came with basic authentication credentials. By implementing this method the credentials can be converted to
        /// a principal.
        /// </summary>
        /// <param name="context">Contains information about the sign in request.</param>
        void SignIn(BasicSignInContext context);

        /// <summary>
        /// Called when an exception occurs during request or response processing.
        /// </summary>
        /// <param name="context">Contains information about the exception that occurred</param>
        void Exception(BasicExceptionContext context);
    }
}