using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Odachi.Abstractions;
using Odachi.JsonRpc.Common.Converters;
using Odachi.JsonRpc.Server.Builder;

namespace Odachi.AspNetCore.JsonRpc.Internal
{
	public static class JsonRpcHandlerBuilderExtensions
	{
		public static IJsonRpcHandlerBuilder UseBlobHandler(this IJsonRpcHandlerBuilder builder)
		{
			return builder.Use(async (context, next) =>
			{
				var httpContextAccessor = context.AppServices.GetRequiredService<IHttpContextAccessor>();

				IBlob HandleBlob(string path, string name)
				{
					var form = httpContextAccessor.HttpContext?.Request?.Form;
					if (form == null)
						return null;

					var file = form.Files[name];
					if (file == null)
						return null;

					return new FormFileBlob(file);
				}

				using (new BlobReadHandler(HandleBlob))
				{
					await next();
				}
			});
		}
	}
}
