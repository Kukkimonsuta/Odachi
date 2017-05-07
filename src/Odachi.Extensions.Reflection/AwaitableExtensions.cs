using Odachi.Extensions.Reflection.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Odachi.Extensions.Reflection
{
	public static class AwaitableExtensions
	{
		private static Dictionary<Type, AwaiterEntry> _awaiterCache = new Dictionary<Type, AwaiterEntry>();
		private static Dictionary<Type, AwaitableEntry> _awaitableCache = new Dictionary<Type, AwaitableEntry>();

		private static AwaiterEntry GetAwaiterEntry(Type type)
		{
			if (_awaiterCache.TryGetValue(type, out var result))
			{
				return result;
			}

			lock (_awaiterCache)
			{
				if (_awaiterCache.TryGetValue(type, out result))
				{
					return result;
				}

				var interfaces = type.GetInterfaces();

				// must implement INotifyCompletion
				if (!interfaces.Any(t => t == typeof(INotifyCompletion)))
				{
					return _awaiterCache[type] = null;
				}

				var notifyCompetitionMap = type.GetTypeInfo().GetRuntimeInterfaceMap(typeof(INotifyCompletion));
				var onCompletedMethod = notifyCompetitionMap.InterfaceMethods.Single(m =>
					m.Name.Equals("OnCompleted", StringComparison.OrdinalIgnoreCase) &&
					m.ReturnType == typeof(void) &&
					m.GetParameters().Length == 1 &&
					m.GetParameters()[0].ParameterType == typeof(Action)
				);

				// may implement ICriticalNotifyCompletion
				var unsafeOnCompletedMethod = default(MethodInfo);
				if (interfaces.Any(t => t == typeof(ICriticalNotifyCompletion)))
				{
					var criticalNotifyCompetitionMap = type.GetTypeInfo().GetRuntimeInterfaceMap(typeof(ICriticalNotifyCompletion));
					unsafeOnCompletedMethod = criticalNotifyCompetitionMap.InterfaceMethods.Single(m =>
						m.Name.Equals("UnsafeOnCompleted", StringComparison.OrdinalIgnoreCase) &&
						m.ReturnType == typeof(void) &&
						m.GetParameters().Length == 1 &&
						m.GetParameters()[0].ParameterType == typeof(Action)
					);
				}

				// must have `bool IsCompleted { get; }` property
				var isCompletedProperty = type.GetRuntimeProperty("IsCompleted");
				if (isCompletedProperty == null || isCompletedProperty.PropertyType != typeof(bool))
				{
					return _awaiterCache[type] = null;
				}

				var isCompletedMethod = isCompletedProperty.GetGetMethod();
				if (isCompletedMethod == null || isCompletedMethod.ReturnType != typeof(bool))
				{
					return _awaiterCache[type] = null;
				}

				// must have `void | T GetResult()` method
				var getResultMethod = type.GetRuntimeMethod("GetResult", Array.Empty<Type>());
				if (getResultMethod == null || getResultMethod.GetParameters().Length > 0)
				{
					return _awaiterCache[type] = null;
				}

				return _awaiterCache[type] = new AwaiterEntry()
				{
					OnCompletedMethod = onCompletedMethod,
					UnsafeOnCompletedMethod = unsafeOnCompletedMethod,
					IsCompletedMethod = isCompletedMethod,
					GetResultMethod = getResultMethod,
				};
			}
		}

		private static AwaitableEntry GetAwaitableEntry(Type type)
		{
			if (_awaitableCache.TryGetValue(type, out var result))
			{
				return result;
			}

			lock (_awaitableCache)
			{
				if (_awaitableCache.TryGetValue(type, out result))
				{
					return result;
				}

				var getAwaiterMethod = type.GetMethod("GetAwaiter", BindingFlags.Public | BindingFlags.Instance);
				if (getAwaiterMethod == null || getAwaiterMethod.GetParameters().Length > 0)
				{
					return _awaitableCache[type] = null;
				}

				if (!IsAwaiter(getAwaiterMethod.ReturnType))
				{
					return _awaitableCache[type] = null;
				}

				return _awaitableCache[type] = new AwaitableEntry()
				{
					GetAwaiterMethod = getAwaiterMethod,
				};
			}
		}

		/// <summary>
		/// Returns whether type is an awaiter.
		/// </summary>
		public static bool IsAwaiter(this Type type)
		{
			return GetAwaiterEntry(type) != null;
		}

		/// <summary>
		/// Returns an awaiter for given target.
		/// </summary>
		public static Awaiter GetAwaiter(this Type type, object target)
		{
			var entry = GetAwaiterEntry(type);
			if (entry == null)
				throw new InvalidOperationException($"Type '{type.FullName}' is not an awaiter");

			return new Awaiter(target, entry.OnCompletedMethod, entry.UnsafeOnCompletedMethod, entry.IsCompletedMethod, entry.GetResultMethod);
		}

		/// <summary>
		/// Returns whether type is an awaitable. Note that this doesn't currently support extensions implementing `GetAwaiter`.
		/// </summary>
		public static bool IsAwaitable(this Type type)
		{
			return GetAwaitableEntry(type) != null;
		}

		/// <summary>
		/// Returns an awaiter for given target.
		/// </summary>
		public static Awaitable GetAwaitable(this Type type, object target)
		{
			var entry = GetAwaitableEntry(type);
			if (entry == null)
				throw new InvalidOperationException($"Type '{type.FullName}' is not an awaitable");

			return new Awaitable(target, entry.GetAwaiterMethod);
		}

		public static Awaitable InvokeAsync(this MethodInfo method, object target, object[] parameters)
		{
			var result = method.Invoke(target, parameters);
			if (result == null)
				return default(Awaitable);

			var resultType = result.GetType();
			if (!resultType.IsAwaitable())
				return Awaitable.FromValue(result);

			return resultType.GetAwaitable(result);
		}

		#region Nested type: AwaiterEntry

		private class AwaiterEntry
		{
			public MethodInfo OnCompletedMethod { get; set; }
			public MethodInfo UnsafeOnCompletedMethod { get; set; }
			public MethodInfo IsCompletedMethod { get; set; }
			public MethodInfo GetResultMethod { get; set; }
		}

		#endregion

		#region Nested type: AwaitableEntry

		private class AwaitableEntry
		{
			public MethodInfo GetAwaiterMethod { get; set; }
		}

		#endregion
	}
}
