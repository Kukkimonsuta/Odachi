using System.Runtime.Serialization;

namespace Odachi.Validation
{
	/// <summary>
	/// Validation level.
	/// </summary>
	[DataContract]
	public enum ValidationSeverity
	{
		Warning = 100,
		Error = 200,
	}
}
