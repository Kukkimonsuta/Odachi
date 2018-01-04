using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odachi.CodeGen.CSharp.Internal;
using Odachi.CodeModel;
using Odachi.CodeGen.IO;
using Odachi.CodeGen.Rendering;

namespace Odachi.CodeGen.CSharp.Renderers
{
	public class ServiceRenderer : IFragmentRenderer<CSharpModuleContext>
	{
		public bool Render(CSharpModuleContext context, Fragment fragment, IndentedTextWriter writer)
		{
			if (!(fragment is ServiceFragment classFragment))
				return false;

			if (classFragment.Hints.TryGetValue("source-type", out var sourceType))
			{
				writer.WriteIndented($"// source: {sourceType}");
				writer.WriteLine();
			}

			context.Import("Odachi.Abstractions");

			using (writer.WriteIndentedBlock(prefix: $"public class {classFragment.Name} "))
			{
				using (writer.WriteIndentedBlock(prefix: $"public {classFragment.Name}(IRpcClient client) "))
				{
					writer.WriteIndented("this._client = client;");
				}
				writer.WriteLine();

				writer.WriteIndented("private IRpcClient _client;");
				writer.WriteLine();

				foreach (var method in classFragment.Methods)
				{
					context.Import("System.Threading.Tasks");

					var rpcModuleName = method.Hints["jsonrpc-module-name"];
					var rpcMethodName = method.Hints["jsonrpc-method-name"] ?? method.Name;

					writer.WriteIndent();
					if (method.ReturnType.Name == "void")
					{
						writer.Write($"public Task ");
					}
					else
					{
						writer.Write($"public Task<{context.Resolve(method.ReturnType)}> ");
					}
					writer.Write($"{CS.Method(method.Name)}Async({string.Join(", ", method.Parameters.Select(p => $"{context.Resolve(p.Type)} {p.Name}"))})");
					using (writer.WriteBlock())
					{
						if (method.ReturnType.Name == "void")
						{
							writer.WriteIndented($"return _client.CallAsync(\"{rpcModuleName}\", \"{rpcMethodName}\", new {{ {string.Join(", ", method.Parameters.Select(p => p.Name))} }});");
						}
						else
						{
							writer.WriteIndented($"return _client.CallAsync<{context.Resolve(method.ReturnType)}>(\"{rpcModuleName}\", \"{rpcMethodName}\", new {{ {string.Join(", ", method.Parameters.Select(p => p.Name))} }});");
						}
					}
					writer.WriteLine();

					if (method != classFragment.Methods.Last())
						writer.WriteLine();
				}
			}

			return true;
		}
	}
}
