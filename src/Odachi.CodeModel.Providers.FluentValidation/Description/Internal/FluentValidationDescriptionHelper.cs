using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;

#nullable enable

namespace Odachi.CodeModel.Providers.FluentValidation.Description.Internal
{
	public static class FluentValidationDescriptionHelper
	{
		private static HashSet<Assembly> _scannedAssemblies = new HashSet<Assembly>();
		private static Dictionary<Type, Type> _scannedValidators = new Dictionary<Type, Type>();

		private static Type? GetValidatorTypeForType(Type type)
		{
			// todo: is this the best way to find matching validators?
			if (_scannedValidators.TryGetValue(type, out var validatorType))
			{
				return validatorType;
			}

			// try scan types assembly for matching validator
			if (!_scannedAssemblies.Contains(type.Assembly))
			{
				var scanResults = AssemblyScanner
					.FindValidatorsInAssembly(type.Assembly)
					.ToArray();

				foreach (var scanResult in scanResults)
				{
					_scannedValidators.Add(scanResult.InterfaceType.GetGenericArguments().Single(), scanResult.ValidatorType);
				}

				_scannedAssemblies.Add(type.Assembly);

				if (_scannedValidators.TryGetValue(type, out validatorType))
				{
					return validatorType;
				}
			}

			return null;
		}

		private static IEnumerable<IPropertyValidator> GetPropertyValidators(IValidatorDescriptor descriptor, string propertyName, Func<IPropertyValidator, bool> predicate)
		{
			// todo: recursive scan for includes?

			var propertyRules = descriptor.GetRulesForMember(propertyName);
			if (propertyRules != null)
			{
				foreach (var propertyRule in propertyRules)
				{
					foreach (var validator in propertyRule.Components.Select(c => c.Validator))
					{
						if (predicate(validator))
						{
							yield return validator;
						}
					}
				}
			}

			var rules = descriptor.GetRulesForMember(null);
			if (rules != null)
			{
				foreach (var includeRule in rules.Where(r => typeof(IIncludeRule).IsAssignableFrom(r.GetType())))
				{
					var includedPropertyRules = includeRule.Components.Select(c => c.Validator).OfType<IChildValidatorAdaptor>()
						.Select(a => Activator.CreateInstance(a.ValidatorType))
						.Cast<IEnumerable<IValidationRule>>()
						.SelectMany(x => x)
						.ToArray();

					foreach (var includedPropertyRule in includedPropertyRules)
					{
						if (includedPropertyRule.Member.Name != propertyName)
						{
							continue;
						}

						foreach (var validator in includedPropertyRule.Components.Select(c => c.Validator))
						{
							if (predicate(validator))
							{
								yield return validator;
							}
						}
					}
				}
			}
		}

		public static bool IsRequired(Type type, MemberInfo member)
		{
			var validatorType = GetValidatorTypeForType(type);
			if (validatorType == null)
			{
				return false;
			}

			var validator = (IValidator?)Activator.CreateInstance(validatorType);
			if (validator == null)
			{
				return false;
			}

			var descriptor = validator.CreateDescriptor();
			if (descriptor == null)
			{
				return false;
			}

			return GetPropertyValidators(descriptor, member.Name, v => v is INotEmptyValidator or INotNullValidator).Any();
		}

		public static (int min, int max) Length(Type type, MemberInfo member)
		{
			var validatorType = GetValidatorTypeForType(type);
			if (validatorType == null)
			{
				return (-1, -1);
			}

			var validator = (IValidator?)Activator.CreateInstance(validatorType);
			if (validator == null)
			{
				return (-1, -1);
			}

			var descriptor = validator.CreateDescriptor();
			if (descriptor == null)
			{
				return (-1, -1);
			}

			var relevantValidators = GetPropertyValidators(descriptor, member.Name, v => v is ILengthValidator);

			var min = -1;
			var max = -1;
			foreach (var lengthValidator in relevantValidators.Cast<ILengthValidator>())
			{
				min = lengthValidator.Min;
				max = lengthValidator.Max;
			}

			return (min, max);
		}
	}
}
