using System;
using System.Threading.Tasks;

namespace Odachi.Validation
{
	public interface IValidator
	{
		Task<bool> ValidateAsync(ValidationState state, Type type, object obj);
	}
}
