using System.Threading.Tasks;

namespace Odachi.AspNetCore.Authentication.ApiKey
{
    /// <summary>
    /// Specifies callback methods which the <see cref="ApiKeyHandler"></see> invokes to enable developer control over the authentication process.
    /// </summary>
    public interface IApiKeyEvents
    {
		/// <summary>
		/// Called when an exception occurs during request or response processing.
		/// </summary>
		/// <param name="context">Contains information about the exception that occurred</param>
		Task AuthenticationFailed(AuthenticationFailedContext context);

		/// <summary>
		/// Called when a request came with api key authentication credentials. By implementing this method the api key can be converted to
		/// a principal.
		/// </summary>
		/// <param name="context">Contains information about the sign in request.</param>
		Task SignIn(ApiKeySignInContext context);
	}
}
