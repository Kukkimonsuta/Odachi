using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Odachi.Abstractions;
using Odachi.Extensions.Reflection;
using Odachi.JsonRpc.Common.Converters;
using Odachi.JsonRpc.Server.Model;

#pragma warning disable CS0618

namespace Odachi.JsonRpc.Server.Internal
{
	public class ReflectedJsonRpcMethod : JsonRpcMethod
	{
		public ReflectedJsonRpcMethod(string moduleName, string methodName, Type type, MethodInfo method)
			: base(moduleName, methodName)
		{
			Type = type ?? throw new ArgumentNullException(nameof(type));
			Method = method ?? throw new ArgumentNullException(nameof(method));

			_parameters = Method.GetParameters()
				.Select(p => new JsonRpcParameter(
					p.Name,
					p.ParameterType,
					p.ParameterType == typeof(JsonRpcServer) ? JsonRpcParameterSource.RpcServices : JsonRpcParameterSource.Request,
					p.IsOptional,
					p.HasDefaultValue ? p.DefaultValue : (p.ParameterType.GetTypeInfo().IsValueType ? Activator.CreateInstance(p.ParameterType) : null)
				))
				.ToArray();

			if (Method.ReturnType.IsAwaitable())
			{
				_returnType = Method.ReturnType.GetAwaitedType();
			}
			else
			{
				_returnType = Method.ReturnType;
			}
		}

		public Type Type { get; }
		public MethodInfo Method { get; }

		private readonly IReadOnlyList<JsonRpcParameter> _parameters;
		public override IReadOnlyList<JsonRpcParameter> Parameters => _parameters;

		private readonly Type _returnType;
		public override Type ReturnType => _returnType;

		private Task ReadParametersAsync(JsonRpcContext context, object[] parameters)
		{
			var request = context.Request;

			var internalParameterCount = 0;

			for (var i = 0; i < Parameters.Count; i++)
			{
				var parameter = Parameters[i];
				var parameterType = parameter.Type;

				object value = null;

				switch (parameter.Source)
				{
					case JsonRpcParameterSource.RpcServices:
						if (parameterType == typeof(JsonRpcServer))
						{
							value = context.Server;
						}
						else
						{
							throw new InvalidOperationException($"Unknown RPC service '{parameterType.FullName}'");
						}

						internalParameterCount++;
						break;

					case JsonRpcParameterSource.AppServices:
						value = context.AppServices.GetService(parameterType);
						internalParameterCount++;
						break;

					case JsonRpcParameterSource.Request:
						if (request.IsIndexed)
						{
							value = request.GetParameter(i - internalParameterCount, parameterType, Type.Missing);
						}
						else
						{
							value = request.GetParameter(parameter.Name, parameter.Type, Type.Missing);
						}
						break;

					default:
						throw new InvalidOperationException($"Undefined behavior for '{parameter.Source}'");
				}

				// if parameter couldn't be resolved, use default value (the called method is responsible for validating parameters)
				if (value == Type.Missing && !parameter.IsOptional)
				{
					value = parameter.DefaultValue;
				}

				parameters[i] = value;
			}

			return Task.CompletedTask;
		}

		public override async Task InvokeAsync(JsonRpcContext context)
		{
			object service = null;
			if (!Method.IsStatic)
			{
				service = context.AppServices.GetRequiredService(Type);
			}

			var parameters = new object[Parameters.Count];
			if (parameters.Length > 0)
			{
				await ReadParametersAsync(context, parameters);
			}

			// invoke the method
			var result = await Method.InvokeAsync(service, parameters);

			// return the result
			context.SetResponse(result);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode() ^ Method.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			var other = obj as ReflectedJsonRpcMethod;
			if (other == null)
				return false;

			return Name == other.Name && Method == other.Method;
		}
	}
}
