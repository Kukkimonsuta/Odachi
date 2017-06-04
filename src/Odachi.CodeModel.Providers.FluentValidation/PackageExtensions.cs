using Odachi.CodeModel.Builders;
using Odachi.CodeModel.Providers.FluentValidation.Description;

namespace Odachi.CodeModel
{
	public static class PackageBuilderExtensions
	{
		public static PackageBuilder UseFluentValidation(this PackageBuilder builder)
		{
			builder.Context.FieldDescriptors.Add(new FluentValidationFieldDescriptor());

			return builder;
		}
    }
}
