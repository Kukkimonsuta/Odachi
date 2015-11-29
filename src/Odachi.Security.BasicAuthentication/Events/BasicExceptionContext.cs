﻿using Microsoft.AspNet.Authentication;
using Microsoft.AspNet.Http;
using System;

namespace Odachi.Security.BasicAuthentication
{
    /// <summary>
    /// Context object used to control flow of basic authentication.
    /// </summary>
    public class BasicExceptionContext : BaseControlContext
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
            BasicOptions options,
            Exception exception)
            : base(context)
        {
            Exception = exception;
        }

        /// <summary>
        /// The exception thrown.
        /// </summary>
        public Exception Exception { get; private set; }
    }
}