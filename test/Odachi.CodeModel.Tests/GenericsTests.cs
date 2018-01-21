using Odachi.CodeModel.Builders;
using Odachi.CodeModel.Mapping;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Odachi.CodeModel.Tests
{
	public class GenericObject<T>
	{
		public T Value { get; set; }

		public bool Test { get; set; }
	}

	public class ObjectWithGenericObject
	{
		public GenericObject<string> Strings { get; set; }
		public GenericObject<int> Ints { get; set; }
	}

	public class GenericsTests
	{
		[Fact]
		public void Can_describe_generic_object()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(GenericObject<>))
				.Build();

			Assert.NotNull(package);
			Assert.Single(package.Modules);

			var module = package.Modules[0];
			Assert.Equal($".\\{nameof(GenericObject<object>)}", module.Name);
			Assert.Single(module.Fragments);

			var fragment = module.Fragments[0] as ObjectFragment;
			Assert.NotNull(fragment);
			Assert.Equal("object", fragment.Kind);
			Assert.Equal(nameof(GenericObject<object>), fragment.Name);
			Assert.Collection(fragment.Fields,
				field =>
				{
					Assert.Equal(nameof(GenericObject<object>.Value), field.Name);
					Assert.Equal(TypeKind.GenericParameter, field.Type.Kind);
				},
				field =>
				{
					Assert.Equal(nameof(GenericObject<object>.Test), field.Name);
					Assert.Equal(TypeKind.Primitive, field.Type.Kind);
				}
			);
		}

		[Fact]
		public void Can_describe_object_with_registered_generic_object()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(GenericObject<>))
				.Module_Object_Default<ObjectWithGenericObject>()
				.Build();

			Assert.NotNull(package);
			Assert.Collection(package.Modules,
				module =>
				{
					Assert.Equal($".\\{nameof(GenericObject<object>)}", module.Name);
					Assert.Collection(module.Fragments,
						fragment =>
						{
							Assert.NotNull(fragment);
							Assert.Equal("object", fragment.Kind);
							Assert.Equal(nameof(GenericObject<object>), fragment.Name);
							Assert.Collection(((ObjectFragment)fragment).Fields,
								field =>
								{
									Assert.Equal(nameof(GenericObject<object>.Value), field.Name);
									Assert.Equal(TypeKind.GenericParameter, field.Type.Kind);
								},
								field =>
								{
									Assert.Equal(nameof(GenericObject<object>.Test), field.Name);
									Assert.Equal(TypeKind.Primitive, field.Type.Kind);
								}
							);
						}
					);
				},
				module =>
				{
					Assert.Equal($".\\{nameof(ObjectWithGenericObject)}", module.Name);
					Assert.Collection(module.Fragments,
						fragment =>
						{
							Assert.NotNull(fragment);
							Assert.Equal("object", fragment.Kind);
							Assert.Equal(nameof(ObjectWithGenericObject), fragment.Name);
							Assert.Collection(((ObjectFragment)fragment).Fields,
								field =>
								{
									Assert.Equal(nameof(ObjectWithGenericObject.Strings), field.Name);
									Assert.Equal(TypeKind.Class, field.Type.Kind);
									Assert.Equal($".\\{nameof(GenericObject<object>)}", field.Type.Module);
									Assert.Equal(nameof(GenericObject<object>), field.Type.Name);
									Assert.Collection(field.Type.GenericArguments,
										argument =>
										{
											Assert.Equal(BuiltinTypeDefinition.String.Name, argument.Name);
										}
									);
								},
								field =>
								{
									Assert.Equal(nameof(ObjectWithGenericObject.Ints), field.Name);
									Assert.Equal(TypeKind.Class, field.Type.Kind);
									Assert.Equal($".\\{nameof(GenericObject<object>)}", field.Type.Module);
									Assert.Equal(nameof(GenericObject<object>), field.Type.Name);
									Assert.Collection(field.Type.GenericArguments,
										argument =>
										{
											Assert.Equal(BuiltinTypeDefinition.Integer.Name, argument.Name);
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
