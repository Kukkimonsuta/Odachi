using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.Internal;
using FluentValidation.Validators;

namespace Odachi.CodeModel.Providers.FluentValidation.Description.Internal
{
	public static class FluentValidationDescriptionHelper
    {
		public static bool IsRequired(Type type, MemberInfo member)
		{
			// todo: find better way to describe..
			// todo: recursive scan for includes

			var validatorAttribute = (ValidatorAttribute)type.GetTypeInfo().GetCustomAttribute(typeof(ValidatorAttribute));
			if (validatorAttribute == null || validatorAttribute.ValidatorType == null)
				return false;

			var validator = ((IValidator)Activator.CreateInstance(validatorAttribute.ValidatorType));

			var descriptor = validator.CreateDescriptor();
			if (descriptor == null)
				return false;

			// handle direct rules
			{
				var rules = descriptor.GetRulesForMember(member.Name);
				if (rules == null)
					return false;

				var isRequired = rules
					.OfType<PropertyRule>()
					.Where(r => r.PropertyName == member.Name)
					.Any(r => r.Validators.Any(v => v.GetType() == typeof(NotEmptyValidator) || v.GetType() == typeof(NotNullValidator)));

				if (isRequired)
					return true;
			}

			// handle include rules
			{
				var isRequired = descriptor.GetRulesForMember(null)
					.OfType<IncludeRule>()
					.SelectMany(r => r.Validators.OfType<ChildValidatorAdaptor>())
					.Select(a => Activator.CreateInstance(a.ValidatorType))
					.Cast<IEnumerable<IValidationRule>>()
					.SelectMany(x => x)
					.OfType<PropertyRule>()
					.Where(r => r.PropertyName == member.Name)
					.Any(r => r.Validators.Any(v => v.GetType() == typeof(NotEmptyValidator) || v.GetType() == typeof(NotNullValidator)));

				if (isRequired)
					return true;
			}

			return false;
		}
    }
}
