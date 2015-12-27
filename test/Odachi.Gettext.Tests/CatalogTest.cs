using Odachi.Gettext.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Odachi.Gettext
{
	public class CatalogTest
	{
		public CatalogTest()
		{
			_assembly = typeof(CatalogTest).GetTypeInfo().Assembly;
		}

		private Assembly _assembly;

		[Fact]
		public void Can_load_czech()
		{
			using (var stream = _assembly.GetManifestResourceStream("Odachi.Gettext.Tests.Resources.cs.mo"))
			using (var reader = new MoReader(stream))
			{
				var catalog = Catalog.Load(reader);

				Assert.Equal("cs", catalog.Culture.Name);
				Assert.Equal(4, catalog.Count);

				var translation = catalog[new GettextKey(null, "String", null)];
				Assert.Equal(1, translation.Count);
				Assert.Equal("Řetězec", translation[0]);

				translation = catalog[new GettextKey(null, "Plural string", "Plural strings")];
				Assert.Equal(3, translation.Count);
				Assert.Equal("Řetězec", translation[0]);
				Assert.Equal("Řetězce", translation[1]);
				Assert.Equal("Řetězců", translation[2]);

				translation = catalog[new GettextKey("context", "Particular string", null)];
				Assert.Equal(1, translation.Count);
				Assert.Equal("Specifický řetězec", translation[0]);

				translation = catalog[new GettextKey("context", "Particular plural string", "Particular plural strings")];
				Assert.Equal(3, translation.Count);
				Assert.Equal("Specifický řetězec", translation[0]);
				Assert.Equal("Specifické řetězce", translation[1]);
				Assert.Equal("Specifických řetězců", translation[2]);
			}
		}
	}
}
