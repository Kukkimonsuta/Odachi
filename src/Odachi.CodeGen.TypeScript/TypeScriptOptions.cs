namespace Odachi.CodeGen.TypeScript
{
	public class TypeScriptOptions : CodeGeneratorOptions
	{
		/// <summary>
		/// Render `export default` if instructed by renderer. Default `true`.
		/// </summary>
		public bool AllowDefaultExports { get; set; } = true;

		/// <summary>
		/// Render `di.ts`.
		/// </summary>
		public bool RenderDi { get; set; } = false;

		/// <summary>
		/// Allow temporal using @js-temporal/polyfill
		/// </summary>
		public bool UseTemporal { get; set; } = false;
	}
}
