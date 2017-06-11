using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Odachi.AspNetCore.JsonRpc.Internal;
using Odachi.AspNetCore.JsonRpc.Behaviors;
using Microsoft.Extensions.Logging;
using Odachi.AspNetCore.JsonRpc.Model;

namespace Odachi.AspNetCore.JsonRpc
{
	public class JsonRpcServer
	{
		public JsonRpcServer(ILoggerFactory loggerFactory, JsonRpcMethodCollection methods, JsonRpcBehaviorCollection behaviors)
		{
			if (methods == null)
				throw new ArgumentNullException(nameof(methods));
			if (behaviors == null)
				throw new ArgumentNullException(nameof(behaviors));

			_logger = loggerFactory.CreateLogger<JsonRpcServer>();
			Methods = methods;
			Behaviors = behaviors;
		}

		private readonly ILogger _logger;

		public JsonRpcMethodCollection Methods { get; }
		public JsonRpcBehaviorCollection Behaviors { get; }

		public async Task ProcessAsync(JsonRpcContext context)
		{
			if (context == null)
				throw new ArgumentNullException(nameof(context));

			try
			{
				var request = context.Request;

				JsonRpcMethod method;

				if (!Methods.TryGetMethod(request.Method, out method))
				{
					context.SetResponse(JsonRpcError.METHOD_NOT_FOUND);
					return;
				}

				// run `Before` behaviors
				for (var i = 0; i < Behaviors.Count; i++)
				{
					await Behaviors[i].BeforeInvoke(context);
				}

				// if `Before` behavior marks request as handled, skip 'handle' phase
				if (!context.WasHandled)
				{
					try
					{
						await method.HandleAsync(context);
					}
					catch (Exception ex)
					{
						// run `Error` behaviors
						for (var i = 0; i < Behaviors.Count; i++)
						{
							await Behaviors[i].OnError(context, ex);
						}

						// if `Error` behavior marks request as handled, don't throw
						if (!context.WasHandled)
						{
							throw;
						}
					}
				}

				// run `After` behavior no matter how the request was handled
				for (var i = 0; i < Behaviors.Count; i++)
				{
					await Behaviors[i].AfterInvoke(context);
				}
			}
			catch (Exception ex)
			{
				var jsonRpcException = ex.Unwrap<JsonRpcException>();
				if (jsonRpcException != null)
				{
					if (jsonRpcException.JsonRpcCode == JsonRpcError.INTERNAL_ERROR)
					{
						_logger.LogError(JsonRpcLogEvents.InternalError, jsonRpcException, "JsonRpc call failed");
					}
					else
					{
						_logger.LogWarning(JsonRpcLogEvents.GenericError, jsonRpcException, "JsonRpc call failed");
					}

					context.SetResponse(jsonRpcException.JsonRpcCode, jsonRpcException.JsonRpcMessage, data: jsonRpcException.JsonRpcData);
					return;
				}

				_logger.LogError(JsonRpcLogEvents.InternalError, ex, "JsonRpc call crashed");

				context.SetResponse(JsonRpcError.INTERNAL_ERROR, data: ex.Unwrap().ToDiagnosticString());
				return;
			}
		}
	}
}
