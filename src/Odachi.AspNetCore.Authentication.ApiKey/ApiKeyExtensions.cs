using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Odachi.AspNetCore.Authentication.ApiKey;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ApiKeyExtensions
	{
		public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder)
			=> builder.AddApiKey(ApiKeyDefaults.AuthenticationScheme, _ => { });

		public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, Action<ApiKeyOptions> configureOptions)
			=> builder.AddApiKey(ApiKeyDefaults.AuthenticationScheme, configureOptions);

		public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme, Action<ApiKeyOptions> configureOptions)
			=> builder.AddApiKey(authenticationScheme, displayName: null, configureOptions: configureOptions);

		public static AuthenticationBuilder AddApiKey(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<ApiKeyOptions> configureOptions)
			=> builder.AddScheme<ApiKeyOptions, ApiKeyHandler>(authenticationScheme, displayName, configureOptions);
	}
}
