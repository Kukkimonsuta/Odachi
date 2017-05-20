using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odachi.CodeModel.Mapping;

namespace Odachi.CodeModel
{
	public class TypeReference
	{
		public TypeReference(string module, string name, bool isNullable, params TypeReference[] genericArguments)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			if (genericArguments == null)
				throw new ArgumentNullException(nameof(genericArguments));

			Module = module;
			Name = name;
			IsNullable = isNullable;
			GenericArguments = genericArguments;
		}
		
		public string Module { get; }
		public string Name { get; }
		public bool IsNullable { get; set; }
		public TypeReference[] GenericArguments { get; }
		
		public override string ToString()
		{
			return $"{Name}{(GenericArguments.Length > 0 ? $"<{string.Join(", ", GenericArguments.Select(a => a.ToString()))}>" : "")}{(IsNullable ? "?" : "")}";
		}
	}
}
