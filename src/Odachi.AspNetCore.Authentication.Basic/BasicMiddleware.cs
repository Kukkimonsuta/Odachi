﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;

namespace Odachi.AspNetCore.Authentication.Basic
{
    /// <summary>
    /// Middleware for basic authentication.
    /// </summary>
    public class BasicMiddleware : AuthenticationMiddleware<BasicOptions>
    {
        public BasicMiddleware(
            RequestDelegate next,
			BasicOptions options,
			ILoggerFactory loggerFactory,
            UrlEncoder encoder)
            : base(next, options, loggerFactory, encoder)
        {
			if (Options.Events == null)
				Options.Events = new BasicEvents();

            if (string.IsNullOrEmpty(Options.Realm))
                Options.Realm = BasicDefaults.Realm;

			if (Options.Credentials == null)
				Options.Credentials = new BasicCredential[0];
        }

        protected override AuthenticationHandler<BasicOptions> CreateHandler()
        {
            return new BasicHandler();
        }
    }
}