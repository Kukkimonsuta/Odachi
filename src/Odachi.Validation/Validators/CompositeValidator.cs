using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Odachi.Validation
{
	public class CompositeValidator : IValidator
	{
		public CompositeValidator(IEnumerable<IValidator> validators, bool runAllValidators = true)
		{
			if (validators == null)
				throw new ArgumentNullException(nameof(validators));

			_validators = validators;
			_runAllValidators = runAllValidators;
		}

		private IEnumerable<IValidator> _validators;
		private bool _runAllValidators;

		public async Task<bool> ValidateAsync(ValidationState builder, Type type, object obj)
		{
			var isValid = true;

			foreach (var validator in _validators)
			{
				isValid = await validator.ValidateAsync(builder, type, obj) && isValid;

				if (!isValid && !_runAllValidators)
					break;
			}

			return isValid;
		}
	}
}
