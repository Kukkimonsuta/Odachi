using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.AspNetCore.JsonRpc.Internal
{
	public static class ExceptionExtensions
	{
		/// <summary>
		/// Remove well knows wrapper exceptions. In case of `AggregateException` return first exception.
		/// </summary>
		public static Exception Unwrap(this Exception ex)
		{
			if (ex is AggregateException aggregateException)
			{
				return aggregateException.InnerException.Unwrap();
			}
			else if (ex is TargetInvocationException targetInvocationException)
			{
				return targetInvocationException.InnerException.Unwrap();
			}

			return ex;
		}

		/// <summary>
		/// Walk `InnerException`s and try to find specific type.
		/// </summary>
		public static T Unwrap<T>(this Exception ex)
			where T : Exception
		{
			if (ex is T tException)
			{
				return tException;
			}
			else if (ex is AggregateException aggregateException)
			{
				for (var i = 0; i < aggregateException.InnerExceptions.Count; i++)
				{
					var result = aggregateException.InnerExceptions[i].Unwrap<T>();

					if (result != null)
					{
						return result;
					}
				}
			}
			else if (ex.InnerException != null)
			{
				return ex.InnerException.Unwrap<T>();
			}

			return null;
		}

		/// <summary>
		/// Formats exception to a string containing exception type, message and stack trace including any `InnerException`s.
		/// </summary>
		public static string ToDiagnosticString(this Exception ex)
		{
			var builder = new StringBuilder();

			void appendException(int level, Exception exception)
			{
				var indent = new String('\t', level);

				builder.AppendLine($"{indent}{exception.GetType().FullName}: {exception.Message}");
				builder.AppendLine($"{indent}{exception.StackTrace}");
				builder.AppendLine();

				if (exception is AggregateException aggregateException)
				{
					foreach (var innerException in aggregateException.InnerExceptions)
					{
						appendException(level + 1, innerException);
					}
				}
				else if (exception.InnerException != null)
				{
					appendException(level + 1, exception.InnerException);
				}
			}

			appendException(0, ex);

			return builder.ToString();
		}
	}
}
