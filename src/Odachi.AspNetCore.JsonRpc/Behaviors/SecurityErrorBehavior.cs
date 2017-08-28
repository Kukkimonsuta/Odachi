using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Odachi.AspNetCore.JsonRpc.Internal;
using Odachi.JsonRpc.Common;

namespace Odachi.AspNetCore.JsonRpc.Behaviors
{
	public class SecurityErrorBehavior : JsonRpcBehavior
	{
		public override Task OnError(JsonRpcContext context, Exception exception)
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

			return Task.CompletedTask;
		}
	}
}
