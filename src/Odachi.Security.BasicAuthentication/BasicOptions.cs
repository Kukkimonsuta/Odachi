using Microsoft.AspNet.Authentication;

namespace Odachi.Security.BasicAuthentication
{
	/// <summary>
	/// Contains the options used by the <see cref="BasicMiddleware"/>.
	/// </summary>
	public class BasicOptions : AuthenticationOptions
	{
		public BasicOptions()
		{
			AuthenticationScheme = BasicDefaults.AuthenticationScheme;
			AutomaticAuthenticate = true;
			AutomaticChallenge = true;
		}

		/// <summary>
		/// The default realm used by basic authentication.
		/// </summary>
		public string Realm { get; set; } = BasicDefaults.Realm;

		/// <summary>
		/// The Provider may be assigned to an instance of an object created by the application at startup time. The middleware
		/// calls methods on the provider which give the application control at certain points where processing is occuring.
		/// If it is not provided a default instance is supplied which does nothing when the methods are called.
		/// </summary>
		public IBasicEvents Events { get; set; } = new BasicEvents();
	}
}