using System;
using System.Security.Cryptography;

namespace Odachi.Extensions.Formatting
{
	public static class UriBuilderExtensions
	{
		/// <summary>
		/// Append key value pair to query string of a <see cref="UriBuilder"/>.
		/// </summary>
		/// <param name="builder">Uri builder</param>
		/// <param name="key">Key</param>
		/// <param name="value">Value</param>
		public static void AppendQuery(this UriBuilder builder, string key, string? value = null)
		{
			if (builder is null)
			{
				throw new ArgumentNullException(nameof(builder));
			}
			if (key is null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			var pair = $"{Uri.EscapeDataString(key)}{(value != null ? $"={Uri.EscapeDataString(value)}" : "")}";

			if (string.IsNullOrEmpty(builder.Query))
			{
				builder.Query = pair;
			}
			else
			{
				builder.Query = builder.Query.Substring(1) + "&" + pair;
			}
		}
	}
}
