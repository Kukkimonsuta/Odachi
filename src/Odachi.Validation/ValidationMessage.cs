namespace Odachi.Validation
{
	/// <summary>
	/// Validation message.
	/// </summary>
	public struct ValidationMessage
	{
		public ValidationLevel Level { get; set; }
		public string Message { get; set; }
	}
}
