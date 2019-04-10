namespace Odachi.Validation
{
	/// <summary>
	/// Validation message.
	/// </summary>
	public class ValidationMessage
	{
		public string Key { get; set; }
		public ValidationSeverity Severity { get; set; }
		public string Text { get; set; }
	}
}
