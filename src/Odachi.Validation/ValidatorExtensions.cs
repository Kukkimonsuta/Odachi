using System.Threading.Tasks;

namespace Odachi.Validation
{
	public static class ValidatorExtensions
	{
		public static Task<bool> ValidateAsync<T>(this IValidator validator, ValidationState state, T obj)
		{
			return validator.ValidateAsync(state, typeof(T), obj);
		}
	}
}
