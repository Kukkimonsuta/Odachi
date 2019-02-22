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
				writer.WriteIndentedLine($"// source: {sourceType}");
				writer.WriteSeparatingLine();
			}

			using (writer.WriteIndentedBlock(prefix: $"class {TS.Type(serviceFragment.Name)} "))
			{
				if (serviceFragment.Constants.Any())
				{
					foreach (var constant in serviceFragment.Constants)
					{
						writer.WriteIndentedLine($"static readonly {TS.Field(constant.Name)}: {context.Resolve(constant.Type)} = {TS.Constant(constant.Value)};");
					}
					writer.WriteSeparatingLine();
				}

				context.Import("@odachi/rpc-client", "RpcClient");
				using (writer.WriteIndentedBlock(prefix: "constructor(client: RpcClient) "))
				{
					writer.WriteIndentedLine("this.client = client;");
				}
				writer.WriteSeparatingLine();

				writer.WriteIndentedLine("private client: RpcClient;");
				writer.WriteSeparatingLine();

				foreach (var method in serviceFragment.Methods)
				{
					var rpcMethodName = method.Hints["jsonrpc-name"] ?? method.Name;

					var parameters = method.Parameters
						.Select(p => $"{TS.Parameter(p.Name)}: {context.Resolve(p.Type)}")
						.ToList();

					using (writer.WriteIndentedBlock(prefix: $"async {TS.Method(method.Name)}Async({string.Join(", ", parameters)}): Promise<{context.Resolve(method.ReturnType)}> "))
					{
						if (method.ReturnType.Name == "void")
						{
							writer.WriteIndentedLine($"await this.client.callAsync('{rpcMethodName}', {{ {string.Join(", ", method.Parameters.Select(p => p.Name))} }});");
						}
						else
						{
							writer.WriteIndentedLine($"const result = await this.client.callAsync('{rpcMethodName}', {{ {string.Join(", ", method.Parameters.Select(p => p.Name))} }});");
							writer.WriteIndentedLine($"return {context.CreateExpression(method.ReturnType, "result")};");
						}
					}
					writer.WriteSeparatingLine();
				}
			}
			writer.WriteSeparatingLine();

			context.Export(serviceFragment.Name, @default: true);

			return true;
		}
	}
}
