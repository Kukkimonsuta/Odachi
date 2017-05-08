using Odachi.AspNetCore.JsonRpc.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Odachi.Validation;
using Odachi.AspNetCore.JsonRpc.Internal;
using System.Text;
using System.Text.RegularExpressions;

namespace Odachi.AspNetCore.JsonRpc.Validation
{
	public class ValidationBehavior : JsonRpcBehavior
	{
		public override void ConfigureRpcServices(IServiceCollection services)
		{
			services.AddScoped<ValidationState>();
		}

		public override Task AfterInvoke(JsonRpcContext context)
		{
			var validationState = context.RpcServices.GetRequiredService<ValidationState>();

			if (validationState.HasErrors)
			{
				context.SetResponse(JsonRpcError.INVALID_PARAMS, "Invalid arguments", new
				{
					Errors = validationState.Errors.ToDictionary(e => MakeCamelSnakePath(e.Key), e => e.Text)
				});
			}

			return base.AfterInvoke(context);
		}

		#region Static members

		private static readonly Regex CamelSnakeRegex = new Regex("(^|_)[A-Z]+", RegexOptions.Compiled);

		private static string MakeCamelSnakePath(string str)
		{
			return CamelSnakeRegex.Replace(str, m => m.Value.ToLowerInvariant());
		}

		#endregion
	}
}
