using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Odachi.JsonRpc.Common;

namespace Odachi.AspNetCore.JsonRpc.Internal
{
	public class JsonRpcResponse
	{
		public JsonRpcResponse(object id, object result)
		{
			if (id == null)
				throw new ArgumentNullException(nameof(id));

			Id = id;
			Result = result;
		}
		public JsonRpcResponse(object id, int errorCode, object errorData = null)
			: this(id, errorCode, JsonRpcError.GetMessage(errorCode), errorData: errorData)
		{
		}
		public JsonRpcResponse(object id, int errorCode, string errorMessage, object errorData = null)
		{
			if (errorMessage == null)
				throw new ArgumentNullException(nameof(errorMessage));

			Id = id;
			if (errorData == null)
				Error = new { Code = errorCode, Message = errorMessage };
			else
				Error = new { Code = errorCode, Message = errorMessage, Data = errorData };
		}

		public object Id { get; }
		public object Result { get; }
		public object Error { get; }
	}
}
