using Odachi.AspNetCore.JsonRpc.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.AspNetCore.JsonRpc
{
	public abstract class JsonRpcMethod
	{
		public JsonRpcMethod(string name)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			if (name.Length <= 0)
				throw new ArgumentException("Argument must not be an empty string", nameof(name));

			Name = name;
		}

		public string Name { get; }

		public abstract Task HandleAsync(JsonRpcContext context);
	}
}
