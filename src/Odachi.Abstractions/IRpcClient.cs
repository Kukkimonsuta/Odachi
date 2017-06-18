using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Abstractions
{
    public interface IRpcClient
    {
		Task CallAsync<TParams>(string service, string method, TParams @params);
		Task<TResult> CallAsync<TResult, TParams>(string service, string method, TParams @params);
	}
}
