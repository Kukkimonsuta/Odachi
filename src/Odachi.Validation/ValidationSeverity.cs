using System.Runtime.Serialization;

namespace Odachi.Validation
{
	/// <summary>
	/// Validation level.
	/// </summary>
	[DataContract]
	public enum ValidationSeverity
	{
		[EnumMember(Value = "warning")]
		Warning = 100,
		[EnumMember(Value = "error")]
		Error = 200,
	}
}
