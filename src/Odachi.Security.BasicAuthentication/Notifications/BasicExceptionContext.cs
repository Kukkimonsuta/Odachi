using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Authentication.Notifications;
using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.Security.BasicAuthentication.Notifications
{
	/// <summary>
	/// Context object used to control flow of basic authentication.
	/// </summary>     
	public class BasicExceptionContext : BaseContext<BasicAuthenticationOptions>
	{
		/// <summary>
		/// Creates a new instance of the context object.
		/// </summary>
		/// <param name="context">The HTTP request context</param>
		/// <param name="options">The middleware options</param>
		/// <param name="exception">The exception thrown.</param>
		/// <param name="ticket">The current ticket, if any.</param>
		public BasicExceptionContext(
			HttpContext context,
			BasicAuthenticationOptions options,
			Exception exception,
			AuthenticationTicket ticket)
			: base(context, options)
		{
			Exception = exception;
			Rethrow = true;
			Ticket = ticket;
		}
		
		/// <summary>
		/// The exception thrown.
		/// </summary>
		public Exception Exception { get; private set; }

		/// <summary>
		/// True if the exception should be re-thrown (default), false if it should be suppressed. 
		/// </summary>
		public bool Rethrow { get; set; }

		/// <summary>
		/// The current authentication ticket, if any.
		/// In the AuthenticateAsync code path, if the given exception is not re-thrown then this ticket
		/// will be returned to the application. The ticket may be replaced if needed.
		/// </summary>
		public AuthenticationTicket Ticket { get; set; }
	}
}
