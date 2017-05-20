namespace Odachi.CodeGen.TypeScript
{
	public interface ITypeScriptTemplateResolver
	{
		ITypeScriptTemplate GetTemplate(TypeScriptModule module);
	}
}
