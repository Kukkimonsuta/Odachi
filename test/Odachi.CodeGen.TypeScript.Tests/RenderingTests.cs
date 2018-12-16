using Odachi.CodeGen.Internal;
using Odachi.CodeGen.IO;
using Odachi.CodeGen.TypeScript.TypeHandlers;
using Odachi.CodeModel;
using Odachi.CodeModel.Builders;
using Odachi.CodeModel.Mapping;
using Odachi.CodeModel.Tests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Odachi.CodeGen.TypeScript.Tests
{
	public class GenericsTests
	{
		private string RenderModule(Package package, string name)
		{
			var module = package.Modules
				.Single(m => m.Name == name);

			var moduleContext = new TypeScriptModuleContext(package, module, new ITypeHandler[] { new DefaultTypeHandler() }, new TypeScriptOptions());
			var enumRenderer = new Renderers.EnumRenderer();
			var objectRenderer = new Renderers.ObjectRenderer();
			var serviceRenderer = new Renderers.ServiceRenderer();

			var stringBuilder = new StringBuilder();
			using (var stringWriter = new StringWriter(stringBuilder))
			{
				using (var indentedWriter = new IndentedTextWriter(stringWriter))
				{
					if (!objectRenderer.Render(moduleContext, module.Fragments.Single(), indentedWriter))
					{
						if (!enumRenderer.Render(moduleContext, module.Fragments.Single(), indentedWriter))
						{
							if (!serviceRenderer.Render(moduleContext, module.Fragments.Single(), indentedWriter))
							{
								throw new InvalidOperationException("No renderer found");
							}
						}
					}
				}
			}
			stringBuilder.TrimEnd();

			using (var resultWriter = new StringWriter())
			{
				using (var indentedWriter = new IndentedTextWriter(resultWriter))
				{
					moduleContext.RenderHeader(indentedWriter);
					moduleContext.RenderBody(indentedWriter, stringBuilder.ToString());
					moduleContext.RenderFooter(indentedWriter);
				}

				return resultWriter.ToString();
			}
		}

		[Fact]
		public void Generic_object()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(GenericObject<>))
				.Build();

			var result = RenderModule(package, $".\\{nameof(GenericObject<object>)}");

			Assert.Equal(@"function fail(message: string): never { throw new Error(message); }
const _$$_factory_boolean = { create: (source: any): boolean => typeof source === 'boolean' ? source : fail(`Contract violation: expected boolean, got \'{typeof(source)}\'`) };

// source: Odachi.CodeModel.Tests.GenericObject`1, Odachi.CodeModel.Tests, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null

class GenericObject<T> {
	value: T | null;

	test: boolean;

	static create<T>(T_factory: { create(source: any): T }): { create: (source: any) => GenericObject<T> } {
		return {
			create: (source: any) => {
				const result = new GenericObject<T>();
				result.value = T_factory.create(source.value);
				result.test = _$$_factory_boolean.create(source.test);
				return result;
			}
		};
	}
}

export default GenericObject;
export { GenericObject };
", result);
		}

		[Fact]
		public void Object_with_generic_object()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(GenericObject<>))
				.Module_Object_Default<ObjectWithGenericObject>()
				.Build();

			var result = RenderModule(package, $".\\{nameof(ObjectWithGenericObject)}");

			Assert.Equal(@"import { GenericObject } from './generic-object';

function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
function fail(message: string): never { throw new Error(message); }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : fail(`Contract violation: expected string, got \'{typeof(source)}\'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : fail(`Contract violation: expected number, got \'{typeof(source)}\'`) };

// source: Odachi.CodeModel.Tests.ObjectWithGenericObject, Odachi.CodeModel.Tests, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null

class ObjectWithGenericObject {
	strings: GenericObject<string | null> | null;

	ints: GenericObject<number> | null;

	static create(source: any): ObjectWithGenericObject {
		const result = new ObjectWithGenericObject();
		result.strings = _$$_opt(GenericObject.create(_$$_factory_string_opt)).create(source.strings);
		result.ints = _$$_opt(GenericObject.create(_$$_factory_number)).create(source.ints);
		return result;
	}
}

export default ObjectWithGenericObject;
export { ObjectWithGenericObject };
", result);
		}

		[Fact]
		public void Object_with_array_of_generic_object()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(GenericObject<>))
				.Module_Object_Default<ObjectWithArrayOfGenericObject>()
				.Build();

			var result = RenderModule(package, $".\\{nameof(ObjectWithArrayOfGenericObject)}");

			Assert.Equal(@"import { GenericObject } from './generic-object';

function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
function fail(message: string): never { throw new Error(message); }
function _$$_factory_array<T>(T_factory: { create: (source: any) => T }) { return { create: (source: any): Array<T> => Array.isArray(source) ? source.map((item: any) => T_factory.create(item)) : fail(`Contract violation: expected array, got \\'{typeof(source)}\\'`) }; }
function _$$_factory_array_opt<T>(T_factory: { create: (source: any) => T }) { return _$$_opt(_$$_factory_array(T_factory)); }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : fail(`Contract violation: expected string, got \'{typeof(source)}\'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : fail(`Contract violation: expected number, got \'{typeof(source)}\'`) };

// source: Odachi.CodeModel.Tests.ObjectWithArrayOfGenericObject, Odachi.CodeModel.Tests, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null

class ObjectWithArrayOfGenericObject {
	strings: Array<GenericObject<string | null> | null> | null;

	ints: Array<GenericObject<number> | null> | null;

	static create(source: any): ObjectWithArrayOfGenericObject {
		const result = new ObjectWithArrayOfGenericObject();
		result.strings = _$$_factory_array_opt(_$$_opt(GenericObject.create(_$$_factory_string_opt))).create(source.strings);
		result.ints = _$$_factory_array_opt(_$$_opt(GenericObject.create(_$$_factory_number))).create(source.ints);
		return result;
	}
}

export default ObjectWithArrayOfGenericObject;
export { ObjectWithArrayOfGenericObject };
", result);
		}

		[Fact]
		public void Object_with_tuple()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(GenericObject<>))
				.Module_Object_Default<ObjectWithTuple>()
				.Build();

			var result = RenderModule(package, $".\\{nameof(ObjectWithTuple)}");

			Assert.Equal(@"import { GenericObject } from './generic-object';
import * as moment from 'moment';
import { Moment } from 'moment';

function fail(message: string): never { throw new Error(message); }
function _$$_factory_tuple3<T1, T2, T3>(T1_factory: { create: (source: any) => T1 }, T2_factory: { create: (source: any) => T2 }, T3_factory: { create: (source: any) => T3 }) { return { create: (source: any): [T1, T2, T3] => [T1_factory.create(source.item1), T2_factory.create(source.item2), T3_factory.create(source.item3)] }; }
function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : fail(`Contract violation: expected string, got \'{typeof(source)}\'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : fail(`Contract violation: expected number, got \'{typeof(source)}\'`) };
const _$$_factory_datetime = { create: (source: any): Moment => typeof source === 'string' ? moment(source) : fail(`Contract violation: expected datetime string, got \'{typeof(source)}\'`) };

// source: Odachi.CodeModel.Tests.ObjectWithTuple, Odachi.CodeModel.Tests, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null

class ObjectWithTuple {
	foo: [string | null, number, GenericObject<Moment> | null];

	static create(source: any): ObjectWithTuple {
		const result = new ObjectWithTuple();
		result.foo = _$$_factory_tuple3(_$$_factory_string_opt, _$$_factory_number, _$$_opt(GenericObject.create(_$$_factory_datetime))).create(source.foo);
		return result;
	}
}

export default ObjectWithTuple;
export { ObjectWithTuple };
", result);
		}

		[Fact]
		public void Object_with_one_of()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(GenericObject<>))
				.Module_Object_Default<ObjectWithOneOf>()
				.Build();

			var result = RenderModule(package, $".\\{nameof(ObjectWithOneOf)}");

			Assert.Equal(@"import { GenericObject } from './generic-object';
import * as moment from 'moment';
import { Moment } from 'moment';

function fail(message: string): never { throw new Error(message); }
function _$$_factory_oneof3<T1, T2, T3>(T1_factory: { create: (source: any) => T1 }, T2_factory: { create: (source: any) => T2 }, T3_factory: { create: (source: any) => T3 }) { return { create: (source: any): T1 | T2 | T3 => { switch (source.index) { case 1: return T1_factory.create(source.option1); case 2: return T2_factory.create(source.option2); case 3: return T3_factory.create(source.option3); default: return fail(`Contract violation: cannot handle OneOf index ${source.index}`); } } }; }
function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : fail(`Contract violation: expected string, got \'{typeof(source)}\'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : fail(`Contract violation: expected number, got \'{typeof(source)}\'`) };
const _$$_factory_datetime = { create: (source: any): Moment => typeof source === 'string' ? moment(source) : fail(`Contract violation: expected datetime string, got \'{typeof(source)}\'`) };

// source: Odachi.CodeModel.Tests.ObjectWithOneOf, Odachi.CodeModel.Tests, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null

class ObjectWithOneOf {
	foo: string | number | GenericObject<Moment> | null;

	static create(source: any): ObjectWithOneOf {
		const result = new ObjectWithOneOf();
		result.foo = _$$_factory_oneof3(_$$_factory_string_opt, _$$_factory_number, _$$_opt(GenericObject.create(_$$_factory_datetime))).create(source.foo);
		return result;
	}
}

export default ObjectWithOneOf;
export { ObjectWithOneOf };
", result);
		}

		[Fact]
		public void Object_with_primitives()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(ObjectWithPrimitives))
				.Build();

			var result = RenderModule(package, $".\\{nameof(ObjectWithPrimitives)}");

			Assert.Equal(@"function fail(message: string): never { throw new Error(message); }
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : fail(`Contract violation: expected number, got \'{typeof(source)}\'`) };

// source: Odachi.CodeModel.Tests.ObjectWithPrimitives, Odachi.CodeModel.Tests, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null

class ObjectWithPrimitives {
	byte: number;

	short: number;

	integer: number;

	long: number;

	float: number;

	double: number;

	decimal: number;

	static create(source: any): ObjectWithPrimitives {
		const result = new ObjectWithPrimitives();
		result.byte = _$$_factory_number.create(source.byte);
		result.short = _$$_factory_number.create(source.short);
		result.integer = _$$_factory_number.create(source.integer);
		result.long = _$$_factory_number.create(source.long);
		result.float = _$$_factory_number.create(source.float);
		result.double = _$$_factory_number.create(source.double);
		result.decimal = _$$_factory_number.create(source.decimal);
		return result;
	}
}

export default ObjectWithPrimitives;
export { ObjectWithPrimitives };
", result);
		}

		[Fact]
		public void Object_with_self_reference()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(ObjectWithSelfReference))
				.Build();

			var result = RenderModule(package, $".\\{nameof(ObjectWithSelfReference)}");

			Assert.Equal(@"function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }

// source: Odachi.CodeModel.Tests.ObjectWithSelfReference, Odachi.CodeModel.Tests, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null

class ObjectWithSelfReference {
	self: ObjectWithSelfReference | null;

	static create(source: any): ObjectWithSelfReference {
		const result = new ObjectWithSelfReference();
		result.self = _$$_opt(ObjectWithSelfReference).create(source.self);
		return result;
	}
}

export default ObjectWithSelfReference;
export { ObjectWithSelfReference };
", result);
		}

		[Fact]
		public void Object_with_constants()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(ConstantsClass))
				.Build();

			var result = RenderModule(package, $".\\{nameof(ConstantsClass)}");

			Assert.Equal(@"// source: Odachi.CodeModel.Tests.ConstantsClass, Odachi.CodeModel.Tests, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null

class ConstantsClass {
	static readonly testString: string = 'fiftyfive';
	static readonly testInt: number = 55;

	static create(source: any): ConstantsClass {
		const result = new ConstantsClass();
		return result;
	}
}

export default ConstantsClass;
export { ConstantsClass };
", result);
		}

		[Fact]
		public void Service_with_constants()
		{
			var package = new PackageBuilder("Test")
				.Module_Service_Default(typeof(ConstantsClass))
				.Build();

			var result = RenderModule(package, $".\\{nameof(ConstantsClass)}");

			Assert.Equal(@"import { RpcClient } from '@odachi/rpc-client';

// source: Odachi.CodeModel.Tests.ConstantsClass, Odachi.CodeModel.Tests, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null

class ConstantsClass {
	static readonly testString: string = 'fiftyfive';
	static readonly testInt: number = 55;

	constructor(client: RpcClient) {
		this.client = client;
	}

	private client: RpcClient;
}

export default ConstantsClass;
export { ConstantsClass };
", result);
		}

		[Fact]
		public void Enum()
		{
			var package = new PackageBuilder("Test")
				.Module_Enum_Default(typeof(StandardEnum))
				.Build();

			var result = RenderModule(package, $".\\{nameof(StandardEnum)}");

			Assert.Equal(@"// source: Odachi.CodeModel.Tests.StandardEnum, Odachi.CodeModel.Tests, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null

enum StandardEnum {
	foo = 1,
	bar = 2,
}

const names = {
	[StandardEnum.foo]: 'foo',
	[StandardEnum.bar]: 'bar',
};

namespace StandardEnum {
	export function create(value: any): StandardEnum {
		if (!StandardEnum.hasOwnProperty(value)) {
			throw new Error(`Value '${value}' is not valid for enum StandardEnum`);
		}

		return value as StandardEnum;
	}

	export function getValues(): StandardEnum[] {
		return [
			StandardEnum.foo,
			StandardEnum.bar,
		];
	}

	export function getNames(): string[] {
		return [
			'foo',
			'bar',
		];
	}

	export function getName(value: StandardEnum): string {
		const name = names[value];

		if (name === undefined) {
			throw new Error(`Cannot get name of StandardEnum '${value}'`);
		}

		return name;
	}
}

export default StandardEnum;
export { StandardEnum };
", result);
		}

		[Fact]
		public void Enum_with_flags()
		{
			var package = new PackageBuilder("Test")
				.Module_Enum_Default(typeof(FlagsEnum))
				.Build();

			var result = RenderModule(package, $".\\{nameof(FlagsEnum)}");

			Assert.Equal(@"// source: Odachi.CodeModel.Tests.FlagsEnum, Odachi.CodeModel.Tests, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null

enum FlagsEnum {
	foo = 1,
	bar = 2,
	cookies = 32,
	combo = 33,
}

const names = {
	[FlagsEnum.foo]: 'foo',
	[FlagsEnum.bar]: 'bar',
	[FlagsEnum.cookies]: 'cookies',
	[FlagsEnum.combo]: 'combo',
};

namespace FlagsEnum {
	export function create(value: any): FlagsEnum {
	    if (typeof value !== 'number') {
	        throw new Error(`Value '${value}' is not valid for enum FlagsEnum`);
	    }

	    let remainder = value;
	    for (let k in FlagsEnum) {
	        const v = FlagsEnum[k];
	        if (!FlagsEnum.hasOwnProperty(v)) {
	            continue;
	        }

	        remainder = remainder & ~v;
	    }

		if (remainder !== 0) {
			throw new Error(`Remainder '${remainder}' of '${value}' is not valid for enum FlagsEnum`);
		}

		return value as FlagsEnum;
	};

	export function getValues(): FlagsEnum[] {
		return [
			FlagsEnum.foo,
			FlagsEnum.bar,
			FlagsEnum.cookies,
			FlagsEnum.combo,
		];
	}

	export function getNames(): string[] {
		return [
			'foo',
			'bar',
			'cookies',
			'combo',
		];
	}

	export function getName(value: FlagsEnum): string {
		const name = names[value];

		if (name === undefined) {
			throw new Error(`Cannot get name of FlagsEnum '${value}'`);
		}

		return name;
	}
}

export default FlagsEnum;
export { FlagsEnum };
", result);
		}
	}
}
