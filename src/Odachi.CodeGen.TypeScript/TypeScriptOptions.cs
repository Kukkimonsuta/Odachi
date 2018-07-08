namespace Odachi.CodeGen.TypeScript
{
	public class TypeScriptOptions : CodeGeneratorOptions
	{
		/// <summary>
		/// Render `export default` if instructed by renderer. Default `true`.
		/// </summary>
		public bool AllowDefaultExports { get; set; } = true;
	}
}
