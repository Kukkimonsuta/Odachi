using Odachi.CodeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Odachi.CodeGen.TypeScript.TypeHandlers
{
	public interface ITypeHandler
	{
		/// <summary>
		/// Returns string representation valid in code of a type reference.
		/// </summary>
		string Resolve(TypeScriptModuleContext context, TypeReference type, bool includeNullability = true, bool includeGenericArguments = true);

		/// <summary>
		/// Returns string representation valid in code of a default value of given type reference.
		/// </summary>
		string ResolveDefaultValue(TypeScriptModuleContext context, TypeReference type);

		/// <summary>
		/// Returns reference to a factory for given type.
		/// </summary>
		string Factory(TypeScriptModuleContext context, TypeReference type);
	}
}
