using Odachi.CodeModel.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Odachi.CodeModel.Tests
{
	public class EmptyClass
	{ }

	public class FieldClass
	{
		public string Bar;
	}

	public class PropertyClass
	{
		public string Foo { get; set; }
	}

	public class MethodClass
	{
		public bool Test()
		{
			return true;
		}
	}

    public class PackageTests
    {
		[Fact]
		public void Can_describe_empty_class_as_object()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<EmptyClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Modules);
			Assert.Single(package.Modules[0].Fragments);
			Assert.Equal("object", package.Modules[0].Fragments[0].Kind);
		}

		[Fact]
		public void Can_describe_empty_class_as_service()
		{
			var package = new PackageBuilder("Test")
				.Module_Service_Default<EmptyClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Modules);
			Assert.Single(package.Modules[0].Fragments);
			Assert.Equal("service", package.Modules[0].Fragments[0].Kind);
		}

		[Fact]
		public void Can_describe_field_class_as_object()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<FieldClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Modules);
			Assert.Single(package.Modules[0].Fragments);
			Assert.Equal("object", package.Modules[0].Fragments[0].Kind);
		}

		[Fact]
		public void Can_describe_property_class_as_object()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<PropertyClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Modules);
			Assert.Single(package.Modules[0].Fragments);
			Assert.Equal("object", package.Modules[0].Fragments[0].Kind);
		}

		[Fact]
		public void Can_describe_method_class_as_service()
		{
			var package = new PackageBuilder("Test")
				.Module_Service_Default<MethodClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Modules);
			Assert.Single(package.Modules[0].Fragments);
			Assert.Equal("service", package.Modules[0].Fragments[0].Kind);
		}
	}
}
