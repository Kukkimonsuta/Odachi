using FluentValidation;
using FluentValidation.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace Odachi.CodeModel.Providers.FluentValidation.Tests.Model
{
	public class ObjectWithAmbientNestedValidation : ObjectWithAmbientValidation
	{
	}

	public class ObjectWithAmbientNestedValidationValidator : AbstractValidator<ObjectWithAmbientNestedValidation>
	{
		public ObjectWithAmbientNestedValidationValidator()
		{
			Include(new ObjectWithAmbientValidationValidator());
		}
	}
}
