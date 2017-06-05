using System;
using System.Collections.Generic;
using System.Text;

namespace Odachi.JsonRpc.Client
{
	public class JsonRpcException : Exception
	{
		public JsonRpcException(string message)
			: base(message)
		{
		}
	}
}
