using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Odachi.Localization;
using System.Globalization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LocalizationServices
	{
        public static IServiceCollection AddChiLocalization(this IServiceCollection services, string path)
        {
			services.Add(new ServiceDescriptor(
				typeof(IGettextProcessor),
				sp => new DefaultGettextProcessor(path),
				ServiceLifetime.Singleton
			));
			services.Add(new ServiceDescriptor(
				typeof(IStringLocalizerFactory),
				typeof(GettextStringLocalizerFactory),
				ServiceLifetime.Singleton
			));
			services.Add(new ServiceDescriptor(
				typeof(IStringLocalizer),
				typeof(GettextStringLocalizer),
				ServiceLifetime.Transient
			));
			services.Add(new ServiceDescriptor(
				typeof(IStringLocalizer<>),
				typeof(StringLocalizer<>),
				ServiceLifetime.Transient
			));

			return services;
        }
	}
}