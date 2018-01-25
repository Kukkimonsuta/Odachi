using Odachi.CodeGen.Internal;
using Odachi.CodeGen.IO;
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

			var moduleContext = new TypeScriptModuleContext(package, module);
			var objectRenderer = new Renderers.ObjectRenderer();

			var stringBuilder = new StringBuilder();
			using (var stringWriter = new StringWriter(stringBuilder))
			{
				using (var indentedWriter = new IndentedTextWriter(stringWriter))
				{
					objectRenderer.Render(moduleContext, module.Fragments.Single(), indentedWriter);
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

			Assert.Equal(@"import { observable } from 'mobx';

function fail(message: string): never { throw new Error(message); }
const _$$_factory_boolean = { create: (source: any): boolean => typeof source === 'boolean' ? source : fail(`Contract violation: expected boolean, got \'{typeof(source)}\'`) };

// source: Odachi.CodeModel.Tests.GenericObject`1, Odachi.CodeModel.Tests, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null

class GenericObject<T> {
	@observable.ref
	value: T | null;

	@observable.ref
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
import { observable } from 'mobx';

function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
function fail(message: string): never { throw new Error(message); }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : fail(`Contract violation: expected string, got \'{typeof(source)}\'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : fail(`Contract violation: expected number, got \'{typeof(source)}\'`) };

// source: Odachi.CodeModel.Tests.ObjectWithGenericObject, Odachi.CodeModel.Tests, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null

class ObjectWithGenericObject {
	@observable.ref
	strings: GenericObject<string | null> | null;

	@observable.ref
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
import { observable } from 'mobx';

function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
function fail(message: string): never { throw new Error(message); }
function _$$_factory_array<T>(T_factory: { create: (source: any) => T }) { return { create: (source: any): Array<T> => Array.isArray(source) ? source.map((item: any) => T_factory.create(item)) : fail(`Contract violation: expected array, got \\'{typeof(source)}\\'`) }; }
function _$$_factory_array_opt<T>(T_factory: { create: (source: any) => T }) { return _$$_opt(_$$_factory_array); }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : fail(`Contract violation: expected string, got \'{typeof(source)}\'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : fail(`Contract violation: expected number, got \'{typeof(source)}\'`) };

// source: Odachi.CodeModel.Tests.ObjectWithArrayOfGenericObject, Odachi.CodeModel.Tests, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null

class ObjectWithArrayOfGenericObject {
	@observable.ref
	strings: Array<GenericObject<string | null> | null> | null;

	@observable.ref
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
		public void Object_with_array_of_generic_object_with_pages()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(GenericObject<>))
				.Module_Object_Default<ObjectWithArrayOfGenericObjectWithPages>()
				.Build();

			var result = RenderModule(package, $".\\{nameof(ObjectWithArrayOfGenericObjectWithPages)}");

			Assert.Equal(@"import { GenericObject } from './generic-object';
import { core } from '@stackino/uno';
import { observable } from 'mobx';

function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
function fail(message: string): never { throw new Error(message); }
function _$$_factory_array<T>(T_factory: { create: (source: any) => T }) { return { create: (source: any): Array<T> => Array.isArray(source) ? source.map((item: any) => T_factory.create(item)) : fail(`Contract violation: expected array, got \\'{typeof(source)}\\'`) }; }
function _$$_factory_array_opt<T>(T_factory: { create: (source: any) => T }) { return _$$_opt(_$$_factory_array); }
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : fail(`Contract violation: expected number, got \'{typeof(source)}\'`) };
function _$$_factory_Page<T>(T_factory: { create: (source: any) => T }) { return { create: (source: any): core.Page<T> =>
		typeof source === 'object' && source !== null ?
			new core.Page<T>(
				_$$_factory_array(T_factory).create(source.data),
				_$$_factory_number.create(source.number),
				_$$_factory_number.create(source.count)
			) :
			fail(`Contract violation: expected page, got \\'{typeof(source)}\\'`) }; }
function _$$_factory_Page_opt<T>(T_factory: { create: (source: any) => T }) { return _$$_opt(_$$_factory_Page); }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : fail(`Contract violation: expected string, got \'{typeof(source)}\'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);

// source: Odachi.CodeModel.Tests.ObjectWithArrayOfGenericObjectWithPages, Odachi.CodeModel.Tests, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null

class ObjectWithArrayOfGenericObjectWithPages {
	@observable.ref
	strings: Array<GenericObject<core.Page<string | null> | null> | null> | null;

	static create(source: any): ObjectWithArrayOfGenericObjectWithPages {
		const result = new ObjectWithArrayOfGenericObjectWithPages();
		result.strings = _$$_factory_array_opt(_$$_opt(GenericObject.create(_$$_factory_Page_opt(_$$_factory_string_opt)))).create(source.strings);
		return result;
	}
}

export default ObjectWithArrayOfGenericObjectWithPages;
export { ObjectWithArrayOfGenericObjectWithPages };
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
import { observable } from 'mobx';
import * as moment from 'moment';
import { Moment } from 'moment';

function fail(message: string): never { throw new Error(message); }
function _$$_factory_tuple3<T1, T2, T3>(T1_factory: { create: (source: any) => T1 }, T2_factory: { create: (source: any) => T2 }, T3_factory: { create: (source: any) => T3 }) { return { create: (source: any): [T1, T2, T3] => [T1_factory.create(source[0]), T2_factory.create(source[1]), T3_factory.create(source[2])] }; }
function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : fail(`Contract violation: expected string, got \'{typeof(source)}\'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : fail(`Contract violation: expected number, got \'{typeof(source)}\'`) };
const _$$_factory_datetime = { create: (source: any): Moment => typeof source === 'string' ? moment(source) : fail(`Contract violation: expected datetime string, got \'{typeof(source)}\'`) };

// source: Odachi.CodeModel.Tests.ObjectWithTuple, Odachi.CodeModel.Tests, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null

class ObjectWithTuple {
	@observable.ref
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
import { observable } from 'mobx';
import * as moment from 'moment';
import { Moment } from 'moment';

function fail(message: string): never { throw new Error(message); }
function _$$_factory_oneof3<T1, T2, T3>(T1_factory: { create: (source: any) => T1 }, T2_factory: { create: (source: any) => T2 }, T3_factory: { create: (source: any) => T3 }) { return { create: (source: any): T1 | T2 | T3 => { switch (source.index) { case 1: return T1_factory.create(source.option1); case 2: return T2_factory.create(source.option2); case 3: return T3_factory.create(source.option3); default: fail(`Contract violation: cannot handle OneOf index ${source.index}`); } } }; }
function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : fail(`Contract violation: expected string, got \'{typeof(source)}\'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : fail(`Contract violation: expected number, got \'{typeof(source)}\'`) };
const _$$_factory_datetime = { create: (source: any): Moment => typeof source === 'string' ? moment(source) : fail(`Contract violation: expected datetime string, got \'{typeof(source)}\'`) };

// source: Odachi.CodeModel.Tests.ObjectWithOneOf, Odachi.CodeModel.Tests, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null

class ObjectWithOneOf {
	@observable.ref
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
		public void Can_render_validation_state()
		{
			var package = new PackageBuilder("Test")
				.UseValidation()
				.Module_Object_Default<ObjectWithValidationState>()
				.Build();

			var result = RenderModule(package, $".\\{nameof(ObjectWithValidationState)}");

			Assert.Equal(@"import { validation } from '@stackino/uno';
import { observable } from 'mobx';

function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
function fail(message: string): never { throw new Error(message); }
const _$$_factory_ValidationState = { create: (source: any): validation.ValidationState => typeof source === 'object' && source !== null && typeof source.state === 'object' && source.state !== null ? new validation.ValidationState(source.state) : fail(`Contract violation: expected validation state, got \\'{typeof(source)}\\'`) };
const _$$_factory_ValidationState_opt = _$$_opt(_$$_factory_ValidationState);

// source: Odachi.CodeModel.Tests.ObjectWithValidationState, Odachi.CodeModel.Tests, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null

class ObjectWithValidationState {
	@observable.ref
	foo: validation.ValidationState | null;

	static create(source: any): ObjectWithValidationState {
		const result = new ObjectWithValidationState();
		result.foo = _$$_factory_ValidationState_opt.create(source.foo);
		return result;
	}
}

export default ObjectWithValidationState;
export { ObjectWithValidationState };
", result);
		}
	}
}
