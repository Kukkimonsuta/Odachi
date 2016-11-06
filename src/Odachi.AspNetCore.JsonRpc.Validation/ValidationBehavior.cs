using Odachi.AspNetCore.JsonRpc.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Odachi.Validation;
using Odachi.AspNetCore.JsonRpc.Internal;
using System.Text;

namespace Odachi.AspNetCore.JsonRpc.Validation
{
	public class ValidationBehavior : JsonRpcBehavior
	{
		public override void ConfigureServices(IServiceCollection services)
		{
			services.AddScoped<ValidationState>();
		}

		public override Task AfterInvoke(JsonRpcContext context)
		{
			var validationState = context.RequestServices.GetRequiredService<ValidationState>();

			if (validationState.HasErrors)
			{
				context.SetResponse(JsonRpcError.INVALID_PARAMS, "Invalid arguments", new
				{
					Errors = validationState.Errors.ToDictionary(e => MakeCamelCasePath(e.Key), e => e.Text)
				});
			}

			return base.AfterInvoke(context);
		}

		#region Static members

		private static string MakeCamelCasePath(string str)
		{
			var output = new char[str.Length];

			bool lower = true;
			for (var i = 0; i < str.Length; i++)
			{
				var c = str[i];

				if (!char.IsLetterOrDigit(c))
				{
					lower = true;
					output[i] = c;
				}
				else if (lower)
				{
					lower = false;
					output[i] = char.ToLowerInvariant(c);
				}
				else
				{
					output[i] = c;
				}
			}

			return new string(output);
		}

		#endregion
	}
}
