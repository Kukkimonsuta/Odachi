using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Odachi.AspNetCore.Authentication.Basic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class BasicExtensions
	{
		public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder)
			=> builder.AddBasic(BasicDefaults.AuthenticationScheme, _ => { });

		public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder, Action<BasicOptions> configureOptions)
			=> builder.AddBasic(BasicDefaults.AuthenticationScheme, configureOptions);

		public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder, string authenticationScheme, Action<BasicOptions> configureOptions)
			=> builder.AddBasic(authenticationScheme, displayName: null, configureOptions: configureOptions);

		public static AuthenticationBuilder AddBasic(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BasicOptions> configureOptions)
			=> builder.AddScheme<BasicOptions, BasicHandler>(authenticationScheme, displayName, configureOptions);
	}
}
