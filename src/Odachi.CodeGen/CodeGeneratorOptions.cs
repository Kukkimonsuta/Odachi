namespace Odachi.CodeGen
{
	public abstract class CodeGeneratorOptions
	{
		/// <summary>
		/// Directory in which output files will be created.
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// Recreate output directory before generating code.
		/// </summary>
		public bool CleanOutputPath { get; set; }
	}
}
