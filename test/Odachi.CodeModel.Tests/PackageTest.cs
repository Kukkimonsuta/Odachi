using Odachi.CodeModel.Builders;
using Odachi.CodeModel.Tests.Model;
using System;
using System.Collections.Generic;
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

	public class GuidClass
	{
		public Guid Foo { get; set; }
	}

	public class MethodClass
	{
		public bool Test()
		{
			return true;
		}
	}

	public class ServiceClass
	{
		public bool Test(string foo, int bar)
		{
			return true;
		}
	}

	public class ConstantsClass
	{
		public const string TestString = "fiftyfive";
		public const int TestInt = 55;
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

	public class SelfReferencingClass
	{
		public SelfReferencingClass Self;
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
			Assert.Single(package.Objects);
			Assert.Equal(nameof(EmptyClass), package.Objects[0].Name);
		}

		[Fact]
		public void Can_describe_constants_in_object()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<ConstantsClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Objects);
			Assert.Collection(package.Objects[0].Constants,
				constant =>
				{
					Assert.Equal("TestString", constant.Name);
					Assert.Equal("fiftyfive", constant.Value);
				},
				constant =>
				{
					Assert.Equal("TestInt", constant.Name);
					Assert.Equal(55, constant.Value);
				}
			);
		}

		[Fact]
		public void Can_describe_constants_in_service()
		{
			var package = new PackageBuilder("Test")
				.Module_Service_Default<ConstantsClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Services);
			Assert.Collection(package.Services[0].Constants,
				constant =>
				{
					Assert.Equal("TestString", constant.Name);
					Assert.Equal("fiftyfive", constant.Value);
				},
				constant =>
				{
					Assert.Equal("TestInt", constant.Name);
					Assert.Equal(55, constant.Value);
				}
			);
		}

		[Fact]
		public void Can_describe_empty_class_as_service()
		{
			var package = new PackageBuilder("Test")
				.Module_Service_Default<EmptyClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Services);
			Assert.Equal(nameof(EmptyClass), package.Services[0].Name);
		}

		[Fact]
		public void Can_describe_field_class_as_object()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<FieldClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Objects);
			Assert.Equal(nameof(FieldClass), package.Objects[0].Name);
		}

		[Fact]
		public void Can_describe_property_class_as_object()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<PropertyClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Objects);
			Assert.Equal(nameof(PropertyClass), package.Objects[0].Name);
		}

		[Fact]
		public void Can_describe_method_class_as_service()
		{
			var package = new PackageBuilder("Test")
				.Module_Service_Default<MethodClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Services);
			Assert.Equal(nameof(MethodClass), package.Services[0].Name);
		}

		[Fact]
		public void Can_describe_byte()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<ByteClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Objects,
				fragment =>
				{
					Assert.Collection(fragment.Fields,
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

		[Fact]
		public void Can_describe_short()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<ShortClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Objects,
				fragment =>
				{
					Assert.Collection(fragment.Fields,
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

		[Fact]
		public void Can_describe_int()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<IntClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Objects,
				fragment =>
				{
					Assert.Collection(fragment.Fields,
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

		[Fact]
		public void Can_describe_nullable_int()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<NullableIntClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Objects,
				fragment =>
				{
					Assert.Collection(fragment.Fields,
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

		[Fact]
		public void Can_describe_guid()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<GuidClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Objects,
				fragment =>
				{
					Assert.Collection(fragment.Fields,
						field =>
						{
							Assert.Equal(nameof(GuidClass.Foo), field.Name);
							Assert.Equal(TypeKind.Struct, field.Type.Kind);
							Assert.Equal("guid", field.Type.Name);
							Assert.False(field.Type.IsNullable);
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
			Assert.Collection(package.Objects,
				fragment =>
				{
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

		[Fact]
		public void Can_describe_nullable_value_tuple()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<NullableValueTupleClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Objects,
				fragment =>
				{
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

		[Fact]
		public void Can_describe_enum()
		{
			var package = new PackageBuilder("Test")
				.Module_Enum_Default<StandardEnum>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Enums,
				fragment =>
				{
					Assert.Equal(nameof(StandardEnum), fragment.Name);

					Assert.DoesNotContain(
						fragment.Hints,
						x => x.Key == "enum-flags" && x.Value == "true"
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
			Assert.Collection(package.Enums,
				fragment =>
				{
					Assert.Equal(nameof(FlagsEnum), fragment.Name);

					Assert.Contains(
						fragment.Hints,
						x => x.Key == "enum-flags" && x.Value == "true"
					);
				}
			);
		}

		[Fact]
		public void Can_describe_self_reference()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<SelfReferencingClass>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Objects,
				fragment =>
				{
					Assert.Equal(nameof(SelfReferencingClass), fragment.Name);

					Assert.Collection(fragment.Fields,
						field =>
						{
							Assert.Equal(nameof(SelfReferencingClass.Self), field.Name);
							Assert.Equal(nameof(SelfReferencingClass), field.Type.Name);
						}
					);
				}
			);
		}

		[Fact]
		public void Can_describe_nonnullable_generics()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(NonNullableGenericItem<,>))
				.Module_Object_Default<NonNullableGeneric>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Objects,
				fragment => { },
				fragment =>
				{
					Assert.Collection(((ObjectFragment)fragment).Fields,
						field =>
						{
							Assert.Equal(nameof(NonNullableGeneric.Foo), field.Name);
							Assert.Equal(TypeKind.Class, field.Type.Kind);
							Assert.False(field.Type.IsNullable);

							Assert.Collection(field.Type.GenericArguments,
								arg =>
								{
									Assert.Equal(nameof(NonNullableGenericItem<List<string>, string>), arg.Name);
									Assert.False(arg.IsNullable);

									Assert.Collection(arg.GenericArguments,
										arg =>
										{
											Assert.Equal("array", arg.Name);
											Assert.False(arg.IsNullable);
											Assert.Collection(arg.GenericArguments,
												arg =>
												{
													Assert.Equal("string", arg.Name);
													Assert.True(arg.IsNullable);
												}
											);
										},
										arg =>
										{
											Assert.Equal("string", arg.Name);
											Assert.False(arg.IsNullable);
										}
									);
								}
							);
						}
					);
				}
			);
		}

		[Fact]
		public void Can_describe_nonnullable_service()
		{
			var package = new PackageBuilder("Test")
				.Module_Service_Default<NullableMethodParameters>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Services,
				fragment =>
				{
					Assert.Collection(fragment.Methods,
						method =>
						{
							Assert.Equal(nameof(NullableMethodParameters.Nullable), method.Name);

							Assert.Collection(method.Parameters,
								arg =>
								{
									Assert.Equal("param", arg.Name);

									Assert.False(arg.Type.IsNullable);
									Assert.Equal("array", arg.Type.Name);
									Assert.Collection(arg.Type.GenericArguments,
										arg =>
										{
											Assert.Equal("string", arg.Name);
											Assert.True(arg.IsNullable);
										}
									);
								}
							);
						},
						method =>
						{
							Assert.Equal(nameof(NullableMethodParameters.NonNullable), method.Name);

							Assert.Collection(method.Parameters,
								arg =>
								{
									Assert.Equal("param", arg.Name);

									Assert.False(arg.Type.IsNullable);
									Assert.Equal("array", arg.Type.Name);
									Assert.Collection(arg.Type.GenericArguments,
										arg =>
										{
											Assert.Equal("string", arg.Name);
											Assert.False(arg.IsNullable);
										}
									);
								}
							);
						}
					);
				}
			);
		}
	}
}
