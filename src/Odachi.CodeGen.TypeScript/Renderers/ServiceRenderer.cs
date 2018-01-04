using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odachi.CodeGen.TypeScript.Internal;
using Odachi.CodeGen.IO;
using Odachi.CodeModel;
using Odachi.CodeGen.Rendering;

namespace Odachi.CodeGen.TypeScript.Renderers
{
	public class ServiceRenderer : IFragmentRenderer<TypeScriptModuleContext>
	{
		public bool Render(TypeScriptModuleContext context, Fragment fragment, IndentedTextWriter writer)
		{
			if (!(fragment is ServiceFragment serviceFragment))
				return false;
			
			if (serviceFragment.Hints.TryGetValue("source-type", out var sourceType))
			{
				writer.WriteIndented($"// source: {sourceType}");
				writer.WriteLine();
			}

			context.Import("inversify", "injectable");
			writer.WriteIndented("@injectable()");
			using (writer.WriteIndentedBlock(prefix: $"class {serviceFragment.Name} "))
			{
				context.Import("@stackino/uno", "net");
				using (writer.WriteIndentedBlock(prefix: "constructor(client: net.JsonRpcClient) "))
				{
					writer.WriteIndented("this.client = client;");
				}
				writer.WriteLine();

				writer.WriteIndented("private client: net.JsonRpcClient;");
				writer.WriteLine();

				foreach (var method in serviceFragment.Methods)
				{
					var rpcMethodName = method.Hints["jsonrpc-name"] ?? method.Name;

					var parameters = method.Parameters
						.Select(p => $"{p.Name}: {context.Resolve(p.Type)}")
						.ToList();

					writer.WriteIndent();
					writer.Write($"async {TS.Method(method.Name)}Async({string.Join(", ", parameters)}): Promise<{context.Resolve(method.ReturnType)}> ");
					using (writer.WriteBlock())
					{
						if (method.ReturnType.Name == "void")
						{
							writer.WriteIndented($"await this.client.callAsync('{rpcMethodName}', {{ {string.Join(", ", method.Parameters.Select(p => p.Name))} }});");
						}
						else
						{
							writer.WriteIndented($"const result = await this.client.callAsync('{rpcMethodName}', {{ {string.Join(", ", method.Parameters.Select(p => p.Name))} }});");
							writer.WriteIndented($"return {context.CreateExpression(method.ReturnType, "result")};");
						}
					}
					writer.WriteLine();

					if (method != serviceFragment.Methods.Last())
						writer.WriteLine();
				}
			}
			writer.WriteLine();

			context.Export(serviceFragment.Name, @default: true);

			return true;
		}
	}
}
