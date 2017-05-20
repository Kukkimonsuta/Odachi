using System.Reflection;

namespace Odachi.CodeModelGen.TypeScript
{
	public class TypeScriptInterfaceBlock : TypeScriptBlock
	{
		public TypeScriptInterfaceBlock(TypeScriptWriter writer, string name, string[] extends)
			: base(writer, prefix: BuildPrefix(writer, name, extends))
		{
		}

		public void WriteMember(PropertyInfo property)
		{
			Writer.WriteInterfaceMember(property);
		}
		public void WriteMember(string name, string fqTsType)
		{
			Writer.WriteInterfaceMember(name, fqTsType);
		}

		private static string BuildPrefix(TypeScriptWriter writer, string name, string[] extends)
		{
			writer.Write($"interface {name}");

			if (extends != null && extends.Length > 0)
				writer.Write($" extends {string.Join(", ", extends)}");

			writer.Write(" ");

			return null;
		}
	}
}
