using System;
using System.Security;
using System.Threading.Tasks;
using Odachi.JsonRpc.Common;
using Odachi.JsonRpc.Common.Internal;

namespace Odachi.JsonRpc.Server.Builder
{
	public static class JsonRpcHandlerBuilderExtensions
	{
		public static IJsonRpcHandlerBuilder Use(this IJsonRpcHandlerBuilder builder, Func<JsonRpcContext, Func<Task>, Task> middleware)
		{
			return builder.Use(next =>
			{
				return context =>
				{
					return middleware(context, () => next(context));
				};
			});
		}

		public static IJsonRpcHandlerBuilder UseSecurityExceptionHandler(this IJsonRpcHandlerBuilder builder)
		{
			return builder.Use(async (context, next) =>
			{
				try
				{
					await next();
				}
				catch (Exception exception)
				{
					var securityException = exception.Unwrap<SecurityException>();
					if (securityException != null)
					{
						// This is wrong:
						// 1) we are using exceptions for flow control
						// 2) sematically `SecurityException` means more `Forbidden` than `Unauthorized`
						// However - this get's the job done and I can't think of any better alternative without
						// introducing ton of boilerplate code.

						context.SetResponse(JsonRpcError.UNAUTHORIZED, $"{JsonRpcError.GetMessage(JsonRpcError.UNAUTHORIZED)} - {securityException.Message}");
					}
				}
			});
		}

		public static void Run(this IJsonRpcHandlerBuilder builder, JsonRpcRequestDelegate handler)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (handler == null)
				throw new ArgumentNullException(nameof(handler));

			builder.Use(_ => handler);
		}
	}
}
