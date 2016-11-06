using System.Linq;
using System.Reflection;

namespace Odachi.CodeGen.TypeScript
{
	public class TypeScriptClassBlock : TypeScriptBlock
	{
		public TypeScriptClassBlock(TypeScriptWriter writer, string name, string[] extends, string[] implements, string[] decorators)
			: base(writer, prefix: BuildPrefix(writer, name, extends, implements, decorators))
		{
		}

		public void WriteMember(PropertyInfo property, string value = null, string[] decorators = null)
		{
			Writer.WriteClassMember(property, value: value, decorators: decorators);
		}
		public void WriteMember(string name, string fqTsType, string value = null, string[] decorators = null)
		{
			Writer.WriteClassMember(name, fqTsType, value: value, decorators: decorators);
		}

		private static string BuildPrefix(TypeScriptWriter writer, string name, string[] extends, string[] implements, string[] decorators)
		{
			if (decorators != null && decorators.Any())
			{
				foreach (var decorator in decorators)
				{
					writer.WriteLine($"@{decorator}");
					writer.WriteIndent();
				}
			}

			writer.Write($"class {name}");

			if (extends != null && extends.Length > 0)
				writer.Write($" extends {string.Join(", ", extends)}");

			if (implements != null && implements.Length > 0)
				writer.Write($" implements {string.Join(", ", implements)}");

			writer.Write(" ");

			return null;
		}
	}
}
