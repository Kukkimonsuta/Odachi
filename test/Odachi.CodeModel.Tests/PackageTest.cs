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

	public class ByteClass
	{
		public byte Bar;
	}

	public class ShortClass
	{
		public short Bar;
	}

	public class IntClass
	{
		public int Bar;
	}

	public class NullableIntClass
	{
		public int? Bar;
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

	public class ValueTupleClass
	{
		public (string foo, string bar) Value;
	}

	public class NullableValueTupleClass
	{
		public (string foo, string bar)? Value;
	}

	public enum StandardEnum
	{
		Foo = 1,
		Bar = 2,
	}

	[Flags]
	public enum FlagsEnum
	{
		Foo = 1,
		Bar = 2,
		Cookies = 32,
		Combo = 33,
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

		[Fact]
		public void Can_describe_byte()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<ByteClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Modules,
				module =>
				{
					Assert.Collection(module.Fragments,
						fragment =>
						{
							Assert.Equal("object", fragment.Kind);

							Assert.Collection(((ObjectFragment)fragment).Fields,
								field =>
								{
									Assert.Equal(nameof(ByteClass.Bar), field.Name);
									Assert.Equal(TypeKind.Primitive, field.Type.Kind);
									Assert.Equal("byte", field.Type.Name);
									Assert.False(field.Type.IsNullable);
								}
							);
						}
					);
				}
			);
		}

		[Fact]
		public void Can_describe_short()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<ShortClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Modules,
				module =>
				{
					Assert.Collection(module.Fragments,
						fragment =>
						{
							Assert.Equal("object", fragment.Kind);

							Assert.Collection(((ObjectFragment)fragment).Fields,
								field =>
								{
									Assert.Equal(nameof(ShortClass.Bar), field.Name);
									Assert.Equal(TypeKind.Primitive, field.Type.Kind);
									Assert.Equal("short", field.Type.Name);
									Assert.False(field.Type.IsNullable);
								}
							);
						}
					);
				}
			);
		}

		[Fact]
		public void Can_describe_int()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<IntClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Modules,
				module =>
				{
					Assert.Collection(module.Fragments,
						fragment =>
						{
							Assert.Equal("object", fragment.Kind);

							Assert.Collection(((ObjectFragment)fragment).Fields,
								field =>
								{
									Assert.Equal(nameof(IntClass.Bar), field.Name);
									Assert.Equal(TypeKind.Primitive, field.Type.Kind);
									Assert.Equal("integer", field.Type.Name);
									Assert.False(field.Type.IsNullable);
								}
							);
						}
					);
				}
			);
		}

		[Fact]
		public void Can_describe_nullable_int()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<NullableIntClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Modules,
				module =>
				{
					Assert.Collection(module.Fragments,
						fragment =>
						{
							Assert.Equal("object", fragment.Kind);

							Assert.Collection(((ObjectFragment)fragment).Fields,
								field =>
								{
									Assert.Equal(nameof(NullableIntClass.Bar), field.Name);
									Assert.Equal(TypeKind.Primitive, field.Type.Kind);
									Assert.Equal("integer", field.Type.Name);
									Assert.True(field.Type.IsNullable);
								}
							);
						}
					);
				}
			);
		}

		[Fact]
		public void Can_describe_value_tuple()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<ValueTupleClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Modules,
				module =>
				{
					Assert.Collection(module.Fragments,
						fragment =>
						{
							Assert.Equal("object", fragment.Kind);

							Assert.Collection(((ObjectFragment)fragment).Fields,
								field =>
								{
									Assert.Equal(nameof(ValueTupleClass.Value), field.Name);
									Assert.Equal(TypeKind.Tuple, field.Type.Kind);
									Assert.False(field.Type.IsNullable);
								}
							);
						}
					);
				}
			);
		}

		[Fact]
		public void Can_describe_nullable_value_tuple()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<NullableValueTupleClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Modules,
				module =>
				{
					Assert.Collection(module.Fragments,
						fragment =>
						{
							Assert.Equal("object", fragment.Kind);

							Assert.Collection(((ObjectFragment)fragment).Fields,
								field =>
								{
									Assert.Equal(nameof(NullableValueTupleClass.Value), field.Name);
									Assert.Equal(TypeKind.Tuple, field.Type.Kind);
									Assert.True(field.Type.IsNullable);
								}
							);
						}
					);
				}
			);
		}

		[Fact]
		public void Can_describe_enum()
		{
			var package = new PackageBuilder("Test")
				.Module_Enum_Default<StandardEnum>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Modules,
				module =>
				{
					Assert.Collection(module.Fragments,
						fragment =>
						{
							Assert.Equal("enum", fragment.Kind);

							Assert.DoesNotContain(
								fragment.Hints,
								x => x.Key == "enum-flags" && x.Value == "true"
							);
						}
					);
				}
			);
		}

		[Fact]
		public void Can_describe_flags_enum()
		{
			var package = new PackageBuilder("Test")
				.Module_Enum_Default<FlagsEnum>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Modules,
				module =>
				{
					Assert.Collection(module.Fragments,
						fragment =>
						{
							Assert.Equal("enum", fragment.Kind);

							Assert.Contains(
								fragment.Hints,
								x => x.Key == "enum-flags" && x.Value == "true"
							);
						}
					);
				}
			);
		}
	}
}
