#nullable enable

using System;
using System.Linq;
using Xunit;

namespace Odachi.Extensions.Formatting.Tests
{
	public class UriBuilderTests
	{
		[Fact]
		public void No_value()
		{
			var uri = new UriBuilder("http://google.com");

			uri.AppendQuery("test");

			Assert.Equal("http://google.com/?test", uri.Uri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped));
		}

		[Fact]
		public void Single_pair()
		{
			var uri = new UriBuilder("http://google.com");

			uri.AppendQuery("test", "22");

			Assert.Equal("http://google.com/?test=22", uri.Uri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped));
		}

		[Fact]
		public void Complex()
		{
			var uri = new UriBuilder("http://google.com");

			uri.AppendQuery("test", "22");
			uri.AppendQuery("foo");
			uri.AppendQuery("bar", "jar");

			Assert.Equal("http://google.com/?test=22&foo&bar=jar", uri.Uri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped));
		}

		[Fact]
		public void Existing_query()
		{
			var uri = new UriBuilder("http://google.com?foo=bar");

			uri.AppendQuery("test", "42");

			Assert.Equal("http://google.com/?foo=bar&test=42", uri.Uri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped));
		}
	}
}
