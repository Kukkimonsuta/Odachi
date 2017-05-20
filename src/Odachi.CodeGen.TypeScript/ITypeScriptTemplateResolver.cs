namespace Odachi.CodeModelGen.TypeScript
{
	public interface ITypeScriptTemplateResolver
	{
		ITypeScriptTemplate GetTemplate(TypeScriptModule module);
	}
}
