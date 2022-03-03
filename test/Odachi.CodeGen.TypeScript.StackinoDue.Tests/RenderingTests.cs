using Odachi.CodeGen.Internal;
using Odachi.CodeGen.IO;
using Odachi.CodeGen.TypeScript.StackinoDue.Renderers;
using Odachi.CodeGen.TypeScript.StackinoDue.TypeHandlers;
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

namespace Odachi.CodeGen.TypeScript.StackinoDue.Tests
{
	public class RenderingTests
	{
		private string RenderModule(Package package, string moduleName, Action<TypeScriptOptions> configure = null)
		{
			var options = new TypeScriptOptions();

			configure?.Invoke(options);

			var packageContext = new TypeScriptPackageContext(package, options);
			var moduleContext = new TypeScriptModuleContext(packageContext, moduleName, new ITypeHandler[] { new StackinoDueTypeHandler(), new DefaultTypeHandler() });

			var enumRenderer = new EnumRenderer();
			var objectRenderer = new ObjectRenderer();
			var serviceRenderer = new ServiceRenderer();

			var stringBuilder = new StringBuilder();
			using (var stringWriter = new StringWriter(stringBuilder))
			{
				using (var indentedWriter = new IndentedTextWriter(stringWriter))
				{
					foreach (var @enum in package.Enums.Where(e => e.ModuleName == moduleName))
					{
						enumRenderer.Render(moduleContext, @enum, indentedWriter);
					}
					foreach (var @object in package.Objects.Where(e => e.ModuleName == moduleName))
					{
						objectRenderer.Render(moduleContext, @object, indentedWriter);
					}
					foreach (var service in package.Services.Where(e => e.ModuleName == moduleName))
					{
						serviceRenderer.Render(moduleContext, service, indentedWriter);
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
		public void Basic_object()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(BasicObject))
				.Build();

			var result = RenderModule(package, $".\\{nameof(BasicObject)}");

			Assert.Equal(@"import { makeObservable, observable } from 'mobx';

function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
function _$$_fail(message: string): never { throw new Error(message); }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : _$$_fail(`Contract violation: expected string, got '${typeof(source)}'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);

// source: Odachi.CodeModel.Tests.BasicObject

class BasicObject {
	constructor() {
		makeObservable(this, {
			foo: observable.ref,
		});
	}

	foo: string | null = null;

	static create(source: any): BasicObject {
		const result = new BasicObject();
		result.foo = _$$_factory_string_opt.create(source.foo);
		return result;
	}

	static copy(source: BasicObject, destination: BasicObject): void {
		destination.foo = source.foo;
	}

	static clone(source: BasicObject): BasicObject {
		const result = new BasicObject();
		BasicObject.copy(source, result);
		return result;
	}
}

export default BasicObject;
export { BasicObject };
", result);
		}

		[Fact]
		public void Complex_object()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(BasicObject))
				.Module_Object_Default(typeof(ComplexObject))
				.Build();

			var result = RenderModule(package, $".\\{nameof(ComplexObject)}");

			Assert.Equal(@"import { BasicObject } from './basic-object';
import { makeObservable, observable } from 'mobx';

function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
function _$$_fail(message: string): never { throw new Error(message); }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : _$$_fail(`Contract violation: expected string, got '${typeof(source)}'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);

// source: Odachi.CodeModel.Tests.ComplexObject

class ComplexObject {
	constructor() {
		makeObservable(this, {
			foo: observable.ref,
			basic: observable.ref,
		});
	}

	foo: string | null = null;
	basic: BasicObject | null = null;

	static create(source: any): ComplexObject {
		const result = new ComplexObject();
		result.foo = _$$_factory_string_opt.create(source.foo);
		result.basic = _$$_opt(BasicObject).create(source.basic);
		return result;
	}

	static copy(source: ComplexObject, destination: ComplexObject): void {
		destination.foo = source.foo;
		destination.basic = source.basic;
	}

	static clone(source: ComplexObject): ComplexObject {
		const result = new ComplexObject();
		ComplexObject.copy(source, result);
		return result;
	}
}

export default ComplexObject;
export { ComplexObject };
", result);
		}

		[Fact]
		public void Complex_object_not_null()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(BasicObject))
				.Module_Object_Default(typeof(ComplexObject), o =>
				{
					foreach (var field in o.Fields)
					{
						field.Type.IsNullable = false;
					}
				})
				.Build();

			var result = RenderModule(package, $".\\{nameof(ComplexObject)}");

			Assert.Equal(@"import { BasicObject } from './basic-object';
import { makeObservable, observable } from 'mobx';

function _$$_fail(message: string): never { throw new Error(message); }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : _$$_fail(`Contract violation: expected string, got '${typeof(source)}'`) };

// source: Odachi.CodeModel.Tests.ComplexObject

class ComplexObject {
	constructor() {
		makeObservable(this, {
			foo: observable.ref,
			basic: observable.ref,
		});
	}

	foo: string = '';
	basic: BasicObject = new BasicObject();

	static create(source: any): ComplexObject {
		const result = new ComplexObject();
		result.foo = _$$_factory_string.create(source.foo);
		result.basic = BasicObject.create(source.basic);
		return result;
	}

	static copy(source: ComplexObject, destination: ComplexObject): void {
		destination.foo = source.foo;
		destination.basic = source.basic;
	}

	static clone(source: ComplexObject): ComplexObject {
		const result = new ComplexObject();
		ComplexObject.copy(source, result);
		return result;
	}
}

export default ComplexObject;
export { ComplexObject };
", result);
		}

		[Fact]
		public void Generic_object()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(GenericObject<>))
				.Build();

			var result = RenderModule(package, $".\\{nameof(GenericObject<object>)}");

			Assert.Equal(@"import { makeObservable, observable } from 'mobx';

function _$$_fail(message: string): never { throw new Error(message); }
const _$$_factory_boolean = { create: (source: any): boolean => typeof source === 'boolean' ? source : _$$_fail(`Contract violation: expected boolean, got '${typeof(source)}'`) };

// source: Odachi.CodeModel.Tests.GenericObject`1

class GenericObject<T> {
	constructor() {
		makeObservable(this, {
			value: observable.ref,
			test: observable.ref,
		});
	}

	value: T | null = null;
	test: boolean = false;

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

	static copy<T>(source: GenericObject<T>, destination: GenericObject<T>): void {
		destination.value = source.value;
		destination.test = source.test;
	}

	static clone<T>(source: GenericObject<T>): GenericObject<T> {
		const result = new GenericObject<T>();
		GenericObject.copy<T>(source, result);
		return result;
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
import { makeObservable, observable } from 'mobx';

function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
function _$$_fail(message: string): never { throw new Error(message); }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : _$$_fail(`Contract violation: expected string, got '${typeof(source)}'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : _$$_fail(`Contract violation: expected number, got '${typeof(source)}'`) };

// source: Odachi.CodeModel.Tests.ObjectWithGenericObject

class ObjectWithGenericObject {
	constructor() {
		makeObservable(this, {
			strings: observable.ref,
			ints: observable.ref,
		});
	}

	strings: GenericObject<string | null> | null = null;
	ints: GenericObject<number> | null = null;

	static create(source: any): ObjectWithGenericObject {
		const result = new ObjectWithGenericObject();
		result.strings = _$$_opt(GenericObject.create(_$$_factory_string_opt)).create(source.strings);
		result.ints = _$$_opt(GenericObject.create(_$$_factory_number)).create(source.ints);
		return result;
	}

	static copy(source: ObjectWithGenericObject, destination: ObjectWithGenericObject): void {
		destination.strings = source.strings;
		destination.ints = source.ints;
	}

	static clone(source: ObjectWithGenericObject): ObjectWithGenericObject {
		const result = new ObjectWithGenericObject();
		ObjectWithGenericObject.copy(source, result);
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
import { makeObservable, observable } from 'mobx';

function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
function _$$_fail(message: string): never { throw new Error(message); }
function _$$_factory_array<T>(T_factory: { create: (source: any) => T }) { return { create: (source: any): Array<T> => Array.isArray(source) ? source.map((item: any) => T_factory.create(item)) : _$$_fail(`Contract violation: expected array, got '${typeof(source)}'`) }; }
function _$$_factory_array_opt<T>(T_factory: { create: (source: any) => T }) { return _$$_opt(_$$_factory_array(T_factory)); }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : _$$_fail(`Contract violation: expected string, got '${typeof(source)}'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : _$$_fail(`Contract violation: expected number, got '${typeof(source)}'`) };

// source: Odachi.CodeModel.Tests.ObjectWithArrayOfGenericObject

class ObjectWithArrayOfGenericObject {
	constructor() {
		makeObservable(this, {
			strings: observable.shallow,
			ints: observable.shallow,
		});
	}

	strings: Array<GenericObject<string | null> | null> | null = null;
	ints: Array<GenericObject<number> | null> | null = null;

	static create(source: any): ObjectWithArrayOfGenericObject {
		const result = new ObjectWithArrayOfGenericObject();
		result.strings = _$$_factory_array_opt(_$$_opt(GenericObject.create(_$$_factory_string_opt))).create(source.strings);
		result.ints = _$$_factory_array_opt(_$$_opt(GenericObject.create(_$$_factory_number))).create(source.ints);
		return result;
	}

	static copy(source: ObjectWithArrayOfGenericObject, destination: ObjectWithArrayOfGenericObject): void {
		destination.strings = source.strings;
		destination.ints = source.ints;
	}

	static clone(source: ObjectWithArrayOfGenericObject): ObjectWithArrayOfGenericObject {
		const result = new ObjectWithArrayOfGenericObject();
		ObjectWithArrayOfGenericObject.copy(source, result);
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
import { Page } from '@odachi/collections';
import { makeObservable, observable } from 'mobx';

function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
function _$$_fail(message: string): never { throw new Error(message); }
function _$$_factory_array<T>(T_factory: { create: (source: any) => T }) { return { create: (source: any): Array<T> => Array.isArray(source) ? source.map((item: any) => T_factory.create(item)) : _$$_fail(`Contract violation: expected array, got '${typeof(source)}'`) }; }
function _$$_factory_array_opt<T>(T_factory: { create: (source: any) => T }) { return _$$_opt(_$$_factory_array(T_factory)); }
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : _$$_fail(`Contract violation: expected number, got '${typeof(source)}'`) };
function _$$_factory_Page<T>(T_factory: { create: (source: any) => T }) { return { create: (source: any): Page<T> => new Page(Array.isArray(source.data) ? source.data.map((item: any) => T_factory.create(item)) : _$$_fail(`Contract violation: expected array, got '${typeof(source)}'`), _$$_factory_number.create(source.number), _$$_factory_number.create(source.count), _$$_factory_number.create(source.size), typeof source.total === 'number' ? _$$_factory_number.create(source.total) : undefined) }; }
function _$$_factory_Page_opt<T>(T_factory: { create: (source: any) => T }) { return _$$_opt(_$$_factory_Page(T_factory)); }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : _$$_fail(`Contract violation: expected string, got '${typeof(source)}'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);

// source: Odachi.CodeModel.Tests.ObjectWithArrayOfGenericObjectWithPages

class ObjectWithArrayOfGenericObjectWithPages {
	constructor() {
		makeObservable(this, {
			strings: observable.shallow,
		});
	}

	strings: Array<GenericObject<Page<string | null> | null> | null> | null = null;

	static create(source: any): ObjectWithArrayOfGenericObjectWithPages {
		const result = new ObjectWithArrayOfGenericObjectWithPages();
		result.strings = _$$_factory_array_opt(_$$_opt(GenericObject.create(_$$_factory_Page_opt(_$$_factory_string_opt)))).create(source.strings);
		return result;
	}

	static copy(source: ObjectWithArrayOfGenericObjectWithPages, destination: ObjectWithArrayOfGenericObjectWithPages): void {
		destination.strings = source.strings;
	}

	static clone(source: ObjectWithArrayOfGenericObjectWithPages): ObjectWithArrayOfGenericObjectWithPages {
		const result = new ObjectWithArrayOfGenericObjectWithPages();
		ObjectWithArrayOfGenericObjectWithPages.copy(source, result);
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
import { DateTime } from 'luxon';
import { makeObservable, observable } from 'mobx';

function _$$_fail(message: string): never { throw new Error(message); }
function _$$_factory_tuple3<T1, T2, T3>(T1_factory: { create: (source: any) => T1 }, T2_factory: { create: (source: any) => T2 }, T3_factory: { create: (source: any) => T3 }) { return { create: (source: any): [T1, T2, T3] => [T1_factory.create(source.item1), T2_factory.create(source.item2), T3_factory.create(source.item3)] }; }
function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : _$$_fail(`Contract violation: expected string, got '${typeof(source)}'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : _$$_fail(`Contract violation: expected number, got '${typeof(source)}'`) };
const _$$_factory_datetime = { create: (source: any): DateTime => typeof source === 'string' ? DateTime.fromISO(source, { setZone: true }) : _$$_fail(`Contract violation: expected datetime string, got '${typeof(source)}'`) };

// source: Odachi.CodeModel.Tests.ObjectWithTuple

class ObjectWithTuple {
	constructor() {
		makeObservable(this, {
			foo: observable.ref,
		});
	}

	foo: [string | null, number, GenericObject<DateTime> | null] = [null, 0, null];

	static create(source: any): ObjectWithTuple {
		const result = new ObjectWithTuple();
		result.foo = _$$_factory_tuple3(_$$_factory_string_opt, _$$_factory_number, _$$_opt(GenericObject.create(_$$_factory_datetime))).create(source.foo);
		return result;
	}

	static copy(source: ObjectWithTuple, destination: ObjectWithTuple): void {
		destination.foo = source.foo;
	}

	static clone(source: ObjectWithTuple): ObjectWithTuple {
		const result = new ObjectWithTuple();
		ObjectWithTuple.copy(source, result);
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
import { DateTime } from 'luxon';
import { makeObservable, observable } from 'mobx';

function _$$_fail(message: string): never { throw new Error(message); }
function _$$_factory_oneof3<T1, T2, T3>(T1_factory: { create: (source: any) => T1 }, T2_factory: { create: (source: any) => T2 }, T3_factory: { create: (source: any) => T3 }) { return { create: (source: any): T1 | T2 | T3 => { switch (source.index) { case 1: return T1_factory.create(source.option1); case 2: return T2_factory.create(source.option2); case 3: return T3_factory.create(source.option3); default: return _$$_fail(`Contract violation: cannot handle OneOf index ${source.index}`); } } }; }
function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
const _$$_factory_string = { create: (source: any): string => typeof source === 'string' ? source : _$$_fail(`Contract violation: expected string, got '${typeof(source)}'`) };
const _$$_factory_string_opt = _$$_opt(_$$_factory_string);
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : _$$_fail(`Contract violation: expected number, got '${typeof(source)}'`) };
const _$$_factory_datetime = { create: (source: any): DateTime => typeof source === 'string' ? DateTime.fromISO(source, { setZone: true }) : _$$_fail(`Contract violation: expected datetime string, got '${typeof(source)}'`) };

// source: Odachi.CodeModel.Tests.ObjectWithOneOf

class ObjectWithOneOf {
	constructor() {
		makeObservable(this, {
			foo: observable.ref,
		});
	}

	foo: string | number | GenericObject<DateTime> | null = null;

	static create(source: any): ObjectWithOneOf {
		const result = new ObjectWithOneOf();
		result.foo = _$$_factory_oneof3(_$$_factory_string_opt, _$$_factory_number, _$$_opt(GenericObject.create(_$$_factory_datetime))).create(source.foo);
		return result;
	}

	static copy(source: ObjectWithOneOf, destination: ObjectWithOneOf): void {
		destination.foo = source.foo;
	}

	static clone(source: ObjectWithOneOf): ObjectWithOneOf {
		const result = new ObjectWithOneOf();
		ObjectWithOneOf.copy(source, result);
		return result;
	}
}

export default ObjectWithOneOf;
export { ObjectWithOneOf };
", result);
		}

		[Fact]
		public void Object_with_net6_date_times()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default<ObjectWithNet6DateTimes>()
				.Build();

			var result = RenderModule(package, $".\\{nameof(ObjectWithNet6DateTimes)}", options =>
			{
				options.UseTemporal = true;
			});

			Assert.Equal(@"import { Temporal } from '@js-temporal/polyfill';
import { makeObservable, observable } from 'mobx';

function _$$_fail(message: string): never { throw new Error(message); }
const _$$_factory_date = { create: (source: any): Temporal.PlainDate => typeof source === 'string' ? Temporal.PlainDate.from(source) : _$$_fail(`Contract violation: expected date string, got '${typeof(source)}'`) };
const _$$_factory_time = { create: (source: any): Temporal.PlainTime => typeof source === 'string' ? Temporal.PlainTime.from(source) : _$$_fail(`Contract violation: expected time string, got '${typeof(source)}'`) };
const _$$_factory_datetime = { create: (source: any): Temporal.PlainDateTime => typeof source === 'string' ? Temporal.PlainDateTime.from(source) : _$$_fail(`Contract violation: expected datetime string, got '${typeof(source)}'`) };
const _$$_factory_duration = { create: (source: any): Temporal.Duration => typeof source === 'string' ? Temporal.Duration.from(source) : _$$_fail(`Contract violation: expected duration string, got '${typeof(source)}'`) };

// source: Odachi.CodeModel.Tests.ObjectWithNet6DateTimes

class ObjectWithNet6DateTimes {
	constructor() {
		makeObservable(this, {
			date: observable.ref,
			time: observable.ref,
			dateTime: observable.ref,
			timeSpan: observable.ref,
		});
	}

	date: Temporal.PlainDate = new Temporal.PlainDate(1900, 1, 1);
	time: Temporal.PlainTime = new Temporal.PlainTime();
	dateTime: Temporal.PlainDateTime = new Temporal.PlainDateTime(1900, 1, 1);
	timeSpan: Temporal.Duration = new Temporal.Duration();

	static create(source: any): ObjectWithNet6DateTimes {
		const result = new ObjectWithNet6DateTimes();
		result.date = _$$_factory_date.create(source.date);
		result.time = _$$_factory_time.create(source.time);
		result.dateTime = _$$_factory_datetime.create(source.dateTime);
		result.timeSpan = _$$_factory_duration.create(source.timeSpan);
		return result;
	}

	static copy(source: ObjectWithNet6DateTimes, destination: ObjectWithNet6DateTimes): void {
		destination.date = source.date;
		destination.time = source.time;
		destination.dateTime = source.dateTime;
		destination.timeSpan = source.timeSpan;
	}

	static clone(source: ObjectWithNet6DateTimes): ObjectWithNet6DateTimes {
		const result = new ObjectWithNet6DateTimes();
		ObjectWithNet6DateTimes.copy(source, result);
		return result;
	}
}

export default ObjectWithNet6DateTimes;
export { ObjectWithNet6DateTimes };
", result);
		}

		[Fact]
		public void Object_with_primitives()
		{
			var package = new PackageBuilder("Test")
				.Module_Object_Default(typeof(ObjectWithPrimitives))
				.Build();

			var result = RenderModule(package, $".\\{nameof(ObjectWithPrimitives)}");

			Assert.Equal(@"import { makeObservable, observable } from 'mobx';

function _$$_fail(message: string): never { throw new Error(message); }
const _$$_factory_number = { create: (source: any): number => typeof source === 'number' ? source : _$$_fail(`Contract violation: expected number, got '${typeof(source)}'`) };

// source: Odachi.CodeModel.Tests.ObjectWithPrimitives

class ObjectWithPrimitives {
	constructor() {
		makeObservable(this, {
			byte: observable.ref,
			short: observable.ref,
			integer: observable.ref,
			long: observable.ref,
			float: observable.ref,
			double: observable.ref,
			decimal: observable.ref,
		});
	}

	byte: number = 0;
	short: number = 0;
	integer: number = 0;
	long: number = 0;
	float: number = 0;
	double: number = 0;
	decimal: number = 0;

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

	static copy(source: ObjectWithPrimitives, destination: ObjectWithPrimitives): void {
		destination.byte = source.byte;
		destination.short = source.short;
		destination.integer = source.integer;
		destination.long = source.long;
		destination.float = source.float;
		destination.double = source.double;
		destination.decimal = source.decimal;
	}

	static clone(source: ObjectWithPrimitives): ObjectWithPrimitives {
		const result = new ObjectWithPrimitives();
		ObjectWithPrimitives.copy(source, result);
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

			Assert.Equal(@"import { makeObservable, observable } from 'mobx';

function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }

// source: Odachi.CodeModel.Tests.ObjectWithSelfReference

class ObjectWithSelfReference {
	constructor() {
		makeObservable(this, {
			self: observable.ref,
		});
	}

	self: ObjectWithSelfReference | null = null;

	static create(source: any): ObjectWithSelfReference {
		const result = new ObjectWithSelfReference();
		result.self = _$$_opt(ObjectWithSelfReference).create(source.self);
		return result;
	}

	static copy(source: ObjectWithSelfReference, destination: ObjectWithSelfReference): void {
		destination.self = source.self;
	}

	static clone(source: ObjectWithSelfReference): ObjectWithSelfReference {
		const result = new ObjectWithSelfReference();
		ObjectWithSelfReference.copy(source, result);
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

			Assert.Equal(@"// source: Odachi.CodeModel.Tests.ConstantsClass

class ConstantsClass {
	static readonly testString: string = 'fiftyfive';
	static readonly testInt: number = 55;
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

			Assert.Equal(@"// source: Odachi.CodeModel.Tests.ConstantsClass

class ConstantsClass {
	static readonly testString: string = 'fiftyfive';
	static readonly testInt: number = 55;
}

export default ConstantsClass;
export { ConstantsClass };
", result);
		}

		[Fact]
		public void Service()
		{
			var package = new PackageBuilder("Test")
				.Module_Service_Default(typeof(ServiceClass))
				.Build();

			var result = RenderModule(package, $".\\{nameof(ServiceClass)}");

			Assert.Equal(@"import { Injectable, Tag } from '@stackino/due';
import { RpcClientTag } from '@stackino/due-plugin-odachirpcclient';

function _$$_fail(message: string): never { throw new Error(message); }
const _$$_factory_boolean = { create: (source: any): boolean => typeof source === 'boolean' ? source : _$$_fail(`Contract violation: expected boolean, got '${typeof(source)}'`) };

// source: Odachi.CodeModel.Tests.ServiceClass

const ServiceClassTag = new Tag<ServiceClass>('Test ServiceClass');

class ServiceClass extends Injectable {
	private readonly client = this.$dependency(RpcClientTag);

	async testAsync(foo: string | null, bar: number): Promise<boolean> {
		const result = await this.client.callAsync('Test', { foo, bar });
		return _$$_factory_boolean.create(result);
	}
}

export default ServiceClass;
export { ServiceClassTag, ServiceClass };
", result);
		}

		[Fact]
		public void Enum()
		{
			var package = new PackageBuilder("Test")
				.Module_Enum_Default(typeof(StandardEnum))
				.Build();

			var result = RenderModule(package, $".\\{nameof(StandardEnum)}");

			Assert.Equal(@"// source: Odachi.CodeModel.Tests.StandardEnum

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

			Assert.Equal(@"// source: Odachi.CodeModel.Tests.FlagsEnum

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

		[Fact]
		public void Validation_state()
		{
			var package = new PackageBuilder("Test")
				.UseValidation()
				.Module_Object_Default<ObjectWithValidationState>()
				.Build();

			var result = RenderModule(package, $".\\{nameof(ObjectWithValidationState)}");

			Assert.Equal(@"import { ValidationState } from '@odachi/validation';
import { makeObservable, observable } from 'mobx';

function _$$_opt<T>(T_factory: { create: (source: any) => T }): { create: (source: any) => T | null } { return { create: (source: any): T | null => source === undefined || source === null ? null : T_factory.create(source) }; }
function _$$_fail(message: string): never { throw new Error(message); }
const _$$_factory_ValidationState = { create: (source: any): ValidationState => typeof Array.isArray(source) ? new ValidationState(source) : _$$_fail(`Contract violation: expected validation state, got '${typeof(source)}'`) };
const _$$_factory_ValidationState_opt = _$$_opt(_$$_factory_ValidationState);

// source: Odachi.CodeModel.Tests.ObjectWithValidationState

class ObjectWithValidationState {
	constructor() {
		makeObservable(this, {
			foo: observable.ref,
		});
	}

	foo: ValidationState | null = null;

	static create(source: any): ObjectWithValidationState {
		const result = new ObjectWithValidationState();
		result.foo = _$$_factory_ValidationState_opt.create(source.foo);
		return result;
	}

	static copy(source: ObjectWithValidationState, destination: ObjectWithValidationState): void {
		destination.foo = source.foo;
	}

	static clone(source: ObjectWithValidationState): ObjectWithValidationState {
		const result = new ObjectWithValidationState();
		ObjectWithValidationState.copy(source, result);
		return result;
	}
}

export default ObjectWithValidationState;
export { ObjectWithValidationState };
", result);
		}

	}
}
