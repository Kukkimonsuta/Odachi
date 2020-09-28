using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Builder;

namespace Odachi.AspNetCore.Authentication.ApiKey
{
	/// <summary>
	/// Contains the options used by the <see cref="ApiKeyHandler"/>.
	/// </summary>
	public class ApiKeyOptions : AuthenticationSchemeOptions
	{
		/// <summary>
		/// Allowed credentials.
		/// </summary>
		public ApiKeyCredential[] Credentials { get; set; } = new ApiKeyCredential[0];

		/// <summary>
		/// The object provided by the application to process events raised by the bearer authentication handler.
		/// The application may implement the interface fully, or it may create an instance of JwtBearerEvents
		/// and assign delegates only to the events it wants to process.
		/// </summary>
		public new ApiKeyEvents Events
		{
			get { return (ApiKeyEvents)base.Events; }
			set { base.Events = value; }
		}
	}
}
