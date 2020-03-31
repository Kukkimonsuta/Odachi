using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace Odachi.CodeModel.Providers.FluentValidation.Tests.Model
{
	[Validator(typeof(ObjectWithAttributeValidationValidator))]
	public class ObjectWithAttributeValidation
	{
		public int? OptionalInt { get; set; }
		public string? RequiredString { get; set; }
		public string? RequiredLengthString { get; set; }
	}

	public class ObjectWithAttributeValidationValidator : AbstractValidator<ObjectWithAttributeValidation>
	{
		public ObjectWithAttributeValidationValidator()
		{
			RuleFor(x => x.RequiredString)
				.NotEmpty();

			RuleFor(x => x.RequiredLengthString)
				.NotEmpty()
				.Length(10, 20);
		}
	}
}
