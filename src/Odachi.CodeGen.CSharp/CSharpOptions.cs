namespace Odachi.CodeGen.CSharp
{
	public class CSharpOptions : CodeGeneratorOptions
	{
		public string Namespace { get; set; }
		public bool EnableNullableReferenceTypes { get; set; }
	}
}
