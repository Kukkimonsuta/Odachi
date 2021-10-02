using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Extensions.Reflection.Internal
{
	public struct Awaitable
	{
		public Awaitable(object? value)
		{
			Value = value;
			Target = null;
			_getAwaiterMethod = null;
		}
		public Awaitable(object target, MethodInfo getAwaiterMethod)
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target));
			if (getAwaiterMethod == null)
				throw new ArgumentNullException(nameof(getAwaiterMethod));

			Value = null;
			Target = target;
			_getAwaiterMethod = getAwaiterMethod;
		}

		private MethodInfo? _getAwaiterMethod;

		public object? Target { get; }
		public object? Value { get; }

		public Awaiter GetAwaiter()
		{
			if (Target == null)
			{
				return new Awaiter(Value);
			}

			var result = _getAwaiterMethod!.Invoke(Target, null);
			if (result == null)
			{
				return default(Awaiter);
			}

			return _getAwaiterMethod.ReturnType.GetAwaiter(result);
		}

		#region Static members

		public static readonly Awaitable Completed = FromValue(null);

		public static Awaitable FromValue(object? value)
		{
			return new Awaitable(value);
		}

		#endregion
	}
}
