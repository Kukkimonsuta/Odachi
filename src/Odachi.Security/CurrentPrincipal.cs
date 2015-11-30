using System;
using System.Security.Claims;
#if NET451
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
#else
using System.Threading;
#endif

namespace Odachi.Security
{
    public sealed class CurrentPrincipalFix
    {
#if NET451
        private static string KeyIsSet = typeof(CurrentPrincipalFix).FullName + ":IsSet";
		private static string KeyCurrent = typeof(CurrentPrincipalFix).FullName + ":Current";

		private static bool IsSetStore
		{
            get
            {
                var handle = CallContext.LogicalGetData(KeyIsSet) as ObjectHandle;
                return handle != null ? (bool)handle.Unwrap() : false;
            }
            set
            {
                CallContext.LogicalSetData(KeyIsSet, new ObjectHandle(value));
            }
        }

		private static ClaimsPrincipal CurrentStore
		{
			get
			{
				var handle = CallContext.LogicalGetData(KeyCurrent) as ObjectHandle;
				return handle != null ? (ClaimsPrincipal)handle.Unwrap() : null;
			}
			set
			{
				CallContext.LogicalSetData(KeyCurrent, new ObjectHandle(value));
			}
		}
#else
		private static AsyncLocal<bool> _isSet = new AsyncLocal<bool>();
		private static AsyncLocal<ClaimsPrincipal> _current = new AsyncLocal<ClaimsPrincipal>();

		private static bool IsSetStore
		{
			get { return _isSet.Value; }
			set { _isSet.Value = value; }
		}

		private static ClaimsPrincipal CurrentStore
		{
			get { return _current.Value; }
			set { _current.Value = value; }
		}
#endif

		public static ClaimsPrincipal Current
        {
            get
            {
                if (!IsSetStore)
                    throw new InvalidOperationException("Current principal is not available");

                return CurrentStore;
            }
        }

        public static void Capture(ClaimsPrincipal principal)
        {
            if (IsSetStore)
                throw new InvalidOperationException("Current principal is already captured");

			IsSetStore = true;
			CurrentStore = principal;
        }

        public static void Replace(ClaimsPrincipal principal)
        {
            if (!IsSetStore)
                throw new InvalidOperationException("Current principal is not captured");

			CurrentStore = principal;
        }

        public static void Release()
        {
            if (!IsSetStore)
                throw new InvalidOperationException("Current principal is not captured");

			IsSetStore = false;
			CurrentStore = null;
        }
    }
}
