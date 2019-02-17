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

namespace Odachi.CodeGen.TypeScript.StackinoDue.Renderers
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

			context.Import("@stackino/due", "Tag");
			writer.WriteIndentedLine($"export const {TS.Type(serviceFragment.Name)}Tag = new Tag<{TS.Type(serviceFragment.Name)}>('{context.PackageContext.Package.Name} {TS.Type(serviceFragment.Name)}');");
			writer.WriteSeparatingLine();

			context.Import("@stackino/due", "injectable");
			writer.WriteIndentedLine($"@injectable({TS.Type(serviceFragment.Name)}Tag)");
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

				context.Import("@stackino/due", "inject");
				context.Import("@stackino/due-plugin-odachirpcclient", "RpcClientTag");
				context.Import("@odachi/rpc-client", "RpcClient");
				writer.WriteIndentedLine("@inject(RpcClientTag)");
				writer.WriteIndentedLine("private readonly client!: RpcClient;");
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
							writer.WriteIndentedLine($"await this.client.callAsync('{rpcMethodName}', {{ {string.Join(", ", parameters)} }});");
						}
						else
						{
							writer.WriteIndentedLine($"const result = await this.client.callAsync('{rpcMethodName}', {{ {string.Join(", ", parameters)} }});");
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
