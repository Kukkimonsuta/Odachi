using System;
using System.Collections.Generic;
using System.Linq;
using Odachi.AspNetCore.JsonRpc.Model;
using Odachi.CodeModel.Builders;
using Odachi.CodeModel.Mapping;
using Odachi.CodeModel.Providers.JsonRpc.Description;
using Odachi.Extensions.Formatting;

namespace Odachi.CodeModel
{
	public static class PackageBuilderExtensions
	{
		public static PackageBuilder UseJsonRpc(this PackageBuilder builder)
		{
			builder.Context.MethodDescriptors.Add(new JsonRpcMethodDescriptor());
			
			return builder;
		}

		public static PackageBuilder Modules_Class_JsonRpcService(this PackageBuilder builder, IEnumerable<JsonRpcMethod> methods)
		{
			if (methods == null)
				throw new ArgumentNullException(nameof(methods));

			var modules = methods.GroupBy(m => m.ModuleName, (k, g) => new
			{
				Name = k,
				Methods = g.ToArray(),
			});

			foreach (var module in modules)
			{
				Module_Class_JsonRpcService(builder, $"{module.Name}Rpc", module.Methods);
			}

			return builder;
		}

		public static PackageBuilder Module_Class_JsonRpcService(this PackageBuilder builder, string fragmentName, IEnumerable<JsonRpcMethod> methods)
		{
			if (methods == null)
				throw new ArgumentNullException(nameof(methods));

			var moduleName = builder.Context.GlobalDescriptor.GetModuleName(builder.Context, fragmentName);

			return builder
				.Module(moduleName, module => module
					.Class(fragmentName, c =>
					{
						c.Hint("logical-kind", "jsonrpc-service");

						foreach (var method in methods)
						{
							c.Method(method.MethodName.ToPascalInvariant(), ClrTypeReference.Create(method.ReturnType?.NetType ?? typeof(void)), method, m =>
							{
								foreach (var parameter in method.Parameters.Where(p => !p.IsInternal))
								{
									m.Parameter(parameter.Name, ClrTypeReference.Create(parameter.Type.NetType), parameter);
								}
							});
						}
					})
				);
		}
	}
}
