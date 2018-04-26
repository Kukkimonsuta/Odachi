using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.JsonRpc.Server.Internal
{
    public class JsonRpcLogEvents
    {
		public static readonly EventId ParseError = new EventId(1, name: "Parse error");
		public static readonly EventId InternalError = new EventId(2, name: "Internal error");

		public static readonly EventId GenericError = new EventId(10, name: "Generic error");

		public static readonly EventId RequestStarting = new EventId(20, name: "Request starting");
		public static readonly EventId RequestFinished = new EventId(30, name: "Request finished");
	}
}
