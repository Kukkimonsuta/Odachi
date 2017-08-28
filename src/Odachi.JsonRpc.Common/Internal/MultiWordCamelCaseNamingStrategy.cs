using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odachi.Extensions.Formatting;
using Newtonsoft.Json.Serialization;

namespace Odachi.JsonRpc.Common.Internal
{
	public class MultiWordCamelCaseNamingStrategy : NamingStrategy
	{
		public MultiWordCamelCaseNamingStrategy()
		{
		}
		public MultiWordCamelCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames)
		{
			ProcessDictionaryKeys = processDictionaryKeys;
			OverrideSpecifiedNames = overrideSpecifiedNames;
		}

		/// <summary>
		/// Resolves the specified property name.
		/// </summary>
		/// <param name="name">The property name to resolve.</param>
		/// <returns>The resolved property name.</returns>
		protected override string ResolvePropertyName(string name)
		{
			return name.ToCamelInvariant();
		}
	}
}
