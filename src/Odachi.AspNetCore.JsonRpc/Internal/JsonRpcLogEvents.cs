using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.AspNetCore.JsonRpc.Internal
{
    public class JsonRpcLogEvents
    {
		public static readonly EventId ParseError = new EventId(1, name: "Parse error");
		public static readonly EventId InternalError = new EventId(2, name: "Internal error");

		public static readonly EventId GenericError = new EventId(10, name: "Generic error");
	}
}
