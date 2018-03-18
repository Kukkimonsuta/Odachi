using Odachi.AspNetCore.JsonRpc.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.AspNetCore.JsonRpc.Model
{
	public abstract class JsonRpcMethod
	{
		public static readonly IReadOnlyList<JsonRpcParameter> NoParameters = new JsonRpcParameter[0];

		public JsonRpcMethod(string moduleName, string methodName)
		{
			if (methodName == null)
				throw new ArgumentNullException(nameof(methodName));
			if (moduleName != null && moduleName.Length <= 0)
				throw new ArgumentException("Argument must not be an empty string", nameof(moduleName));
			if (methodName.Length <= 0)
				throw new ArgumentException("Argument must not be an empty string", nameof(methodName));

			ModuleName = moduleName;
			MethodName = methodName;
			Name = ModuleName != null ? $"{ModuleName}.{MethodName}" : MethodName;
		}

		/// <summary>
		/// Optional prefix of method name.
		/// </summary>
		public string ModuleName { get; }

		/// <summary>
		/// Method name.
		/// </summary>
		public string MethodName { get; }

		/// <summary>
		/// Full name of json rpc method.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Method parameters.
		/// </summary>
		public virtual IReadOnlyList<JsonRpcParameter> Parameters => NoParameters;

		/// <summary>
		/// The return type of method, `null` means `void`.
		/// </summary>
		public virtual Type ReturnType => null;

		public virtual void Analyze(JsonRpcServer server, Type[] internalTypes)
		{
		}

		public abstract Task HandleAsync(JsonRpcContext context);
	}
}
