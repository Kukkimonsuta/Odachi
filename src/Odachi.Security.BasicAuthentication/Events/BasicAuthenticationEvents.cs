using System;

namespace Odachi.Security.BasicAuthentication
{
    /// <summary>
    /// This default implementation of the <see cref="IBasicAuthenticationEvents"/> may be used if the
    /// application only needs to override a few of the interface methods. This may be used as a base class
    /// or may be instantiated directly.
    /// </summary>
    public class BasicAuthenticationEvents : IBasicAuthenticationEvents
    {
        /// <summary>
        /// Create a new instance of the default notifications.
        /// </summary>
        public BasicAuthenticationEvents()
        {
            OnSignIn = context => { };
            OnException = context => { };
        }

        /// <summary>
        /// A delegate assigned to this property will be invoked when the related method is called
        /// </summary>
        public Action<BasicSignInContext> OnSignIn { get; set; }

        /// <summary>
        /// A delegate assigned to this property will be invoked when the related method is called
        /// </summary>
        public Action<BasicExceptionContext> OnException { get; set; }

        /// <summary>
        /// Implements the interface method by invoking the related delegate method
        /// </summary>
        /// <param name="context"></param>
        public virtual void SignIn(BasicSignInContext context)
        {
            OnSignIn.Invoke(context);
        }

        /// <summary>
        /// Implements the interface method by invoking the related delegate method
        /// </summary>
        /// <param name="context">Contains information about the event</param>
        public virtual void Exception(BasicExceptionContext context)
        {
            OnException.Invoke(context);
        }
    }
}