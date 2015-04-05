using System;
using System.Security.Claims;

namespace Odachi.Security.BasicAuthentication
{
    public class GenericAuthenticator : IAuthenticator
    {
        public GenericAuthenticator(Func<string, string, ClaimsIdentity> authenticate)
        {
            if (authenticate == null)
                throw new ArgumentNullException(nameof(authenticate));
            
            _authenticate = authenticate;
        }

        private Func<string, string, ClaimsIdentity> _authenticate;

        public ClaimsIdentity Authenticate(string userName, string password)
        {
            return _authenticate(userName, password);
        }
    }
}