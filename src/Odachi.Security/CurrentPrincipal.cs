using System;
using System.Security.Claims;
using System.Threading;

namespace Odachi.Security
{
    public sealed class CurrentPrincipalFix
    {
        private static AsyncLocal<bool> _isSet = new AsyncLocal<bool>();
        private static AsyncLocal<ClaimsPrincipal> _current = new AsyncLocal<ClaimsPrincipal>();

        public static ClaimsPrincipal Current
        {
            get
            {
                if (!_isSet.Value)
                    throw new InvalidOperationException("Current principal is not available");

                return _current.Value;
            }
        }

        public static void Capture(ClaimsPrincipal principal)
        {
            if (_isSet.Value)
                throw new InvalidOperationException("Current principal is already captured");

            _isSet.Value = true;
            _current.Value = principal;
        }

        public static void Replace(ClaimsPrincipal principal)
        {
            if (!_isSet.Value)
                throw new InvalidOperationException("Current principal is not captured");

            _current.Value = principal;
        }

        public static void Release()
        {
            if (!_isSet.Value)
                throw new InvalidOperationException("Current principal is not captured");

            _isSet.Value = false;
            _current.Value = null;
        }
    }
}
