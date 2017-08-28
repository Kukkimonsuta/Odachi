using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Abstractions
{
    public interface IRpcClient
    {
		Task CallAsync(string service, string method, object @params);
		Task<TResult> CallAsync<TResult>(string service, string method, object @params);
	}
}
