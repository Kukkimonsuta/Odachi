using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Extensions.Reflection.Internal
{
	public struct Awaiter : INotifyCompletion
	{
		public Awaiter(object value)
		{
			Value = value;
			Target = null;
			_onCompletedMethod = null;
			_unsafeOnCompletedMethod = null;
			_isCompletedMethod = null;
			_getResultMethod = null;
		}
		public Awaiter(object target, MethodInfo onCompletedMethod, MethodInfo unsafeOnCompletedMethod, MethodInfo isCompletedMethod, MethodInfo getResultMethod)
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target));
			if (onCompletedMethod == null)
				throw new ArgumentNullException(nameof(onCompletedMethod));
			if (isCompletedMethod == null)
				throw new ArgumentNullException(nameof(isCompletedMethod));
			if (getResultMethod == null)
				throw new ArgumentNullException(nameof(getResultMethod));

			Value = null;
			Target = target;
			_onCompletedMethod = onCompletedMethod;
			_unsafeOnCompletedMethod = unsafeOnCompletedMethod;
			_isCompletedMethod = isCompletedMethod;
			_getResultMethod = getResultMethod;
		}

		private MethodInfo _onCompletedMethod;
		private MethodInfo _unsafeOnCompletedMethod;
		private MethodInfo _isCompletedMethod;
		private MethodInfo _getResultMethod;

		public object Target { get; }
		public object Value { get; }

		public void OnCompleted(Action continuation)
		{
			if (Target == null)
			{
				continuation();
				return;
			}

			_onCompletedMethod.Invoke(continuation, new[] { continuation });
		}

		public void UnsafeOnCompleted(Action continuation)
		{
			if (Target == null)
			{
				continuation();
				return;
			}

			// https://github.com/aspnet/Common/blob/dev/shared/Microsoft.Extensions.ObjectMethodExecutor.Sources/ObjectMethodExecutorAwaitable.cs

			if (_unsafeOnCompletedMethod != null)
			{
				_unsafeOnCompletedMethod.Invoke(continuation, new[] { continuation });
			}
			else
			{
				_onCompletedMethod.Invoke(continuation, new[] { continuation });
			}
		}

		public bool IsCompleted
		{
			get
			{
				if (Target == null)
				{
					return true;
				}

				return (bool)_isCompletedMethod.Invoke(Target, null);
			}
		}

		public object GetResult()
		{
			if (Target == null)
			{
				return Value;
			}

			return _getResultMethod.Invoke(Target, null);
		}
	}
}
