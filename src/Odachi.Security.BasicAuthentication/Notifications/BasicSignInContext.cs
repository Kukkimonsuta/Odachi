using Microsoft.AspNet.Authentication.Notifications;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Odachi.Security.BasicAuthentication.Notifications
{
	/// <summary>
	/// Context object used to control flow of basic authentication.
	/// </summary>    
	public class BasicSignInContext : BaseContext<BasicAuthenticationOptions>
	{
		/// <summary>
		/// Creates a new instance of the context object.
		/// </summary>
		/// <param name="context">The HTTP request context</param>
		/// <param name="options">The middleware options</param>
		/// <param name="username">The username</param>
		/// <param name="password">The password</param>
		public BasicSignInContext(
			HttpContext context,
			BasicAuthenticationOptions options,
			string username,
			string password
		)
			: base(context, options)
		{
			Username = username;
			Password = password;
		}

		/// <summary>
		/// Contains the username used in basic authentication.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		/// Contains the password used in basic authentication.
		/// </summary>
		public string Password { get; set; }
		
		/// <summary>
		/// Contains the principal that will be returned to the application.
		/// </summary>
		public ClaimsPrincipal Principal { get; set; }
	}
}
