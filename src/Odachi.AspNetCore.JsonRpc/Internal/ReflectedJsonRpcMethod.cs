using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Odachi.AspNetCore.JsonRpc.Internal
{
	public class ReflectedJsonRpcMethod : JsonRpcMethod
	{
		public ReflectedJsonRpcMethod(string name, Type type, MethodInfo method)
			: base(name)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));

			Type = type;
			Method = method;
			_parameters = method.GetParameters();
		}

		private ParameterInfo[] _parameters;

		public Type Type { get;}
		public MethodInfo Method { get; }

		public override async Task HandleAsync(JsonRpcContext context)
		{
			var request = context.Request;

			var service = context.RequestServices.GetRequiredService(Type);

			var parameters = new object[_parameters.Length];
			for (var i = 0; i < _parameters.Length; i++)
			{
				var parameter = _parameters[i];
				var parameterType = parameter.ParameterType;

				object value = null;

				// try resolve value from DI (only reference types)
				if (!parameterType.GetTypeInfo().IsValueType)
				{
					value = context.RequestServices.GetService(parameterType);
				}

				// if DI fails, try to resolve the value from request
				if (value == null)
				{
					if (request.IsIndexed)
						value = request.GetParameter(i, parameterType, Type.Missing);
					else
						value = request.GetParameter(parameter.Name, parameterType, Type.Missing);
				}

				// if parameter couldn't be resolved, use default value (the called method is responsible for validating parameters)
				if (value == Type.Missing && !parameter.IsOptional)
					value = parameterType.GetTypeInfo().IsValueType ? Activator.CreateInstance(parameterType) : null;

				parameters[i] = value;
			}

			// invoke the method
			var result = Method.Invoke(service, parameters);

			// await tasks
			if (typeof(Task).IsAssignableFrom(Method.ReturnType))
			{
				var task = (Task)result;

				await task;

				if (Method.ReturnType == typeof(Task))
				{
					result = null;
				}
				else
				{
					result = GetResult(Method.ReturnType, task);
				}
			}

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
