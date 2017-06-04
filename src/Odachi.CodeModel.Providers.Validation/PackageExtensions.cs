using Odachi.CodeModel.Builders;
using Odachi.CodeModel.Mapping;
using Odachi.Validation;

namespace Odachi.CodeModel
{
	public static class PackageBuilderExtensions
	{
		public static PackageBuilder UseValidation(this PackageBuilder builder)
		{
			builder.Map(typeof(ValueOrState<>), new BuiltinTypeDefinition("ValueOrState", new GenericArgumentDefinition("TValue")));
			builder.Map(typeof(ValidationState), new BuiltinTypeDefinition("ValidationState"));

			return builder;
		}
	}
}
