namespace Odachi.CodeGen.TypeScript
{
	public interface ITypeScriptTemplate
	{
		void Write(TypeScriptWriter writer, TypeScriptModule module);
	}
}
