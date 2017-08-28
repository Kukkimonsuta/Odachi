using Microsoft.Extensions.DependencyInjection;
using Odachi.AspNetCore.JsonRpc.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.AspNetCore.JsonRpc.Behaviors
{
    public abstract class JsonRpcBehavior
    {
		public virtual void ConfigureRpcServices(IServiceCollection services)
		{
		}

		public virtual Task BeforeInvoke(JsonRpcContext context)
		{
			return Task.CompletedTask;
		}

		public virtual Task OnError(JsonRpcContext context, Exception exception)
		{
			return Task.CompletedTask;
		}

		public virtual Task AfterInvoke(JsonRpcContext context)
		{
			return Task.CompletedTask;
		}
	}
}
