using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace Odachi.CodeModel.Providers.FluentValidation.Tests.Model
{
	public class ObjectWithAmbientValidation
	{
		public int? OptionalInt { get; set; }
		public string? RequiredString { get; set; }
		public string? RequiredLengthString { get; set; }
	}

	public class ObjectWithAmbientValidationValidator : AbstractValidator<ObjectWithAmbientValidation>
	{
		public ObjectWithAmbientValidationValidator()
		{
			RuleFor(x => x.RequiredString)
				.NotEmpty();

			RuleFor(x => x.RequiredLengthString)
				.NotEmpty()
				.Length(10, 20);
		}
	}
}
