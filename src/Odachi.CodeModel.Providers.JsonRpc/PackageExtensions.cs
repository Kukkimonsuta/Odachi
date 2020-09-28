using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.CodeModel.Builders;
using Odachi.CodeModel.Mapping;
using Odachi.CodeModel.Providers.JsonRpc.Description;
using Odachi.Extensions.Formatting;
using Odachi.Extensions.Reflection;
using Odachi.JsonRpc.Server.Internal;
using Odachi.JsonRpc.Server.Model;

namespace Odachi.CodeModel
{
	public static class PackageBuilderExtensions
	{
		public static PackageBuilder UseJsonRpc(this PackageBuilder builder)
		{
			builder.Context.MethodDescriptors.Add(new JsonRpcMethodDescriptor());
			builder.Context.TypeReferenceDescriptors.Add(new JsonRpcTypeReferenceDescriptor());

			return builder;
		}

		public static PackageBuilder Modules_Service_JsonRpc(this PackageBuilder builder, IEnumerable<JsonRpcMethod> methods)
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
				Module_Service_JsonRpc(builder, $"{module.Name}Rpc", module.Methods);
			}

			return builder;
		}

		public static PackageBuilder Module_Service_JsonRpc(this PackageBuilder builder, string fragmentName, IEnumerable<JsonRpcMethod> methods)
		{
			if (methods == null)
				throw new ArgumentNullException(nameof(methods));

			return builder
				.Service(fragmentName, service =>
				{
					foreach (var method in methods)
					{
						var isNullable = !method.ReturnType.IsNonNullableValueType();

						// TODO: jsonrpc server should probably have only reflected methods
						if (method is ReflectedJsonRpcMethod reflectedJsonRpcMethod)
						{
							var returnType = reflectedJsonRpcMethod.Method.ReturnType;

							if (returnType.IsGenericType && (returnType.GetGenericTypeDefinition() == typeof(Task<>) || returnType.GetGenericTypeDefinition() == typeof(ValueTask<>)))
							{
								isNullable = !reflectedJsonRpcMethod.Method.ReturnParameter.IsGenericArgumentNonNullable(0);
							}
							else
							{
								isNullable = !reflectedJsonRpcMethod.Method.ReturnParameter.IsNonNullable();
							}
						}

						service.Method(method.MethodName.ToPascalInvariant(), method.ReturnType ?? typeof(void), method, m =>
						{
							m.ReturnType.IsNullable = isNullable;

							foreach (var parameter in method.Parameters.Where(p => p.Source == JsonRpcParameterSource.Request))
							{
								m.Parameter(parameter.Name, parameter.Type, parameter);
							}
						});
					}
				});
		}
	}
}
