using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Odachi.AspNetCore.JsonRpc.Model;
using Odachi.Extensions.Reflection;

namespace Odachi.AspNetCore.JsonRpc.Internal
{
	public class ReflectedJsonRpcMethod : JsonRpcMethod
	{
		public ReflectedJsonRpcMethod(string moduleName, string methodName, Type type, MethodInfo method)
			: base(moduleName, methodName)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			Type = type;
			Method = method;

			Parameters = method.GetParameters()
				.Select(p => new JsonRpcParameter(p.Name, JsonMappedType.FromType(p.ParameterType), p.IsOptional, p.DefaultValue))
				.ToArray();

			ReturnType = JsonMappedType.FromType(method.ReturnType);
		}

		public Type Type { get;}
		public MethodInfo Method { get; }

		public override IReadOnlyList<JsonRpcParameter> Parameters { get; }
		public override JsonMappedType ReturnType { get; }

		public override async Task HandleAsync(JsonRpcContext context)
		{
			var request = context.Request;

			var service = context.RequestServices.GetRequiredService(Type);

			var parameters = new object[Parameters.Count];
			for (var i = 0; i < Parameters.Count; i++)
			{
				var parameter = Parameters[i];
				var parameterType = parameter.Type;

				object value = null;

				// try resolve value from DI (only reference types)
				if (!parameterType.NetType.GetTypeInfo().IsValueType)
				{
					value = context.RequestServices.GetService(parameterType.NetType);
				}

				// if DI fails, try to resolve the value from request
				if (value == null)
				{
					if (request.IsIndexed)
						value = request.GetParameter(i, parameterType, Type.Missing);
					else
						value = request.GetParameter(parameter.Name, parameter.Type, Type.Missing);
				}

				// if parameter couldn't be resolved, use default value (the called method is responsible for validating parameters)
				if (value == Type.Missing && !parameter.IsOptional)
				{
					value = parameterType.NetType.GetTypeInfo().IsValueType ? Activator.CreateInstance(parameterType.NetType) : null;
				}

				parameters[i] = value;
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

		#region Static members

		private static object GetResult(Type type, Task task)
		{
			var resultProperty = type.GetProperty("Result");
			if (resultProperty == null)
				throw new InvalidOperationException("The task doesn't have a 'Result' property");

			return resultProperty.GetValue(task);
		}

		#endregion
	}
}
