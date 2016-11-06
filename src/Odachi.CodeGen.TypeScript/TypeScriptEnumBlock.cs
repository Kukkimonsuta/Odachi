namespace Odachi.CodeGen.TypeScript
{
	public class TypeScriptEnumBlock : TypeScriptBlock
	{
		public TypeScriptEnumBlock(TypeScriptWriter writer, string name)
			: base(writer, prefix: $"enum {name}")
		{
		}

		public void WriteMember<T>(T value)
		{
			Writer.WriteEnumMember<T>(value);
		}
		public void WriteMember(string name, long value)
		{
			Writer.WriteEnumMember(name, value);
		}
	}
}
