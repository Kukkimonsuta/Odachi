namespace Odachi.AspNetCore.Authentication.ApiKey
{
	/// <summary>
	/// Default values used by <see cref="ApiKeyHandler"/> when not defined in <see cref="ApiKeyOptions"/>.
	/// </summary>
	public static class ApiKeyDefaults
    {
        /// <summary>
        /// The default authentication scheme used by api key authentication.
        /// </summary>
        public const string AuthenticationScheme = "ApiKey";
    }
}
