using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.CodeModel.Mapping
{
	public class BuiltinTypeDefinition : TypeDefinition
	{
		public BuiltinTypeDefinition(string name, params GenericArgumentDefinition[] genericArguments)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			GenericArguments = genericArguments ?? throw new ArgumentNullException(nameof(genericArguments));
		}

		public override string Module { get; } = null;
		public override string Name { get; }
		public override IReadOnlyList<GenericArgumentDefinition> GenericArguments { get; }

		#region Static members

		public static readonly BuiltinTypeDefinition Void = new BuiltinTypeDefinition("void");
		public static readonly BuiltinTypeDefinition Null = new BuiltinTypeDefinition("null");

		public static readonly BuiltinTypeDefinition Boolean = new BuiltinTypeDefinition("boolean");
		public static readonly BuiltinTypeDefinition Byte = new BuiltinTypeDefinition("byte");
		public static readonly BuiltinTypeDefinition Short = new BuiltinTypeDefinition("short");
		public static readonly BuiltinTypeDefinition Integer = new BuiltinTypeDefinition("integer");
		public static readonly BuiltinTypeDefinition Long = new BuiltinTypeDefinition("long");
		public static readonly BuiltinTypeDefinition Float = new BuiltinTypeDefinition("float");
		public static readonly BuiltinTypeDefinition Double = new BuiltinTypeDefinition("double");
		public static readonly BuiltinTypeDefinition Decimal = new BuiltinTypeDefinition("decimal");
		public static readonly BuiltinTypeDefinition String = new BuiltinTypeDefinition("string");
		public static readonly BuiltinTypeDefinition DateTime = new BuiltinTypeDefinition("datetime");
		public static readonly BuiltinTypeDefinition Array = new BuiltinTypeDefinition("array", new GenericArgumentDefinition("T"));
		public static readonly BuiltinTypeDefinition File = new BuiltinTypeDefinition("file");
		public static readonly BuiltinTypeDefinition Guid = new BuiltinTypeDefinition("guid");

		public static readonly BuiltinTypeDefinition Tuple1 = new BuiltinTypeDefinition("tuple", new GenericArgumentDefinition("T1"));
		public static readonly BuiltinTypeDefinition Tuple2 = new BuiltinTypeDefinition("tuple", new GenericArgumentDefinition("T1"), new GenericArgumentDefinition("T2"));
		public static readonly BuiltinTypeDefinition Tuple3 = new BuiltinTypeDefinition("tuple", new GenericArgumentDefinition("T1"), new GenericArgumentDefinition("T2"), new GenericArgumentDefinition("T3"));
		public static readonly BuiltinTypeDefinition Tuple4 = new BuiltinTypeDefinition("tuple", new GenericArgumentDefinition("T1"), new GenericArgumentDefinition("T2"), new GenericArgumentDefinition("T3"), new GenericArgumentDefinition("T4"));
		public static readonly BuiltinTypeDefinition Tuple5 = new BuiltinTypeDefinition("tuple", new GenericArgumentDefinition("T1"), new GenericArgumentDefinition("T2"), new GenericArgumentDefinition("T3"), new GenericArgumentDefinition("T4"), new GenericArgumentDefinition("T5"));
		public static readonly BuiltinTypeDefinition Tuple6 = new BuiltinTypeDefinition("tuple", new GenericArgumentDefinition("T1"), new GenericArgumentDefinition("T2"), new GenericArgumentDefinition("T3"), new GenericArgumentDefinition("T4"), new GenericArgumentDefinition("T5"), new GenericArgumentDefinition("T6"));
		public static readonly BuiltinTypeDefinition Tuple7 = new BuiltinTypeDefinition("tuple", new GenericArgumentDefinition("T1"), new GenericArgumentDefinition("T2"), new GenericArgumentDefinition("T3"), new GenericArgumentDefinition("T4"), new GenericArgumentDefinition("T5"), new GenericArgumentDefinition("T6"), new GenericArgumentDefinition("T7"));
		public static readonly BuiltinTypeDefinition Tuple8 = new BuiltinTypeDefinition("tuple", new GenericArgumentDefinition("T1"), new GenericArgumentDefinition("T2"), new GenericArgumentDefinition("T3"), new GenericArgumentDefinition("T4"), new GenericArgumentDefinition("T5"), new GenericArgumentDefinition("T6"), new GenericArgumentDefinition("T7"), new GenericArgumentDefinition("T8"));

		public static readonly BuiltinTypeDefinition OneOf2 = new BuiltinTypeDefinition("oneof", new GenericArgumentDefinition("T1"), new GenericArgumentDefinition("T2"));
		public static readonly BuiltinTypeDefinition OneOf3 = new BuiltinTypeDefinition("oneof", new GenericArgumentDefinition("T1"), new GenericArgumentDefinition("T2"), new GenericArgumentDefinition("T3"));
		public static readonly BuiltinTypeDefinition OneOf4 = new BuiltinTypeDefinition("oneof", new GenericArgumentDefinition("T1"), new GenericArgumentDefinition("T2"), new GenericArgumentDefinition("T3"), new GenericArgumentDefinition("T4"));
		public static readonly BuiltinTypeDefinition OneOf5 = new BuiltinTypeDefinition("oneof", new GenericArgumentDefinition("T1"), new GenericArgumentDefinition("T2"), new GenericArgumentDefinition("T3"), new GenericArgumentDefinition("T4"), new GenericArgumentDefinition("T5"));
		public static readonly BuiltinTypeDefinition OneOf6 = new BuiltinTypeDefinition("oneof", new GenericArgumentDefinition("T1"), new GenericArgumentDefinition("T2"), new GenericArgumentDefinition("T3"), new GenericArgumentDefinition("T4"), new GenericArgumentDefinition("T5"), new GenericArgumentDefinition("T6"));
		public static readonly BuiltinTypeDefinition OneOf7 = new BuiltinTypeDefinition("oneof", new GenericArgumentDefinition("T1"), new GenericArgumentDefinition("T2"), new GenericArgumentDefinition("T3"), new GenericArgumentDefinition("T4"), new GenericArgumentDefinition("T5"), new GenericArgumentDefinition("T6"), new GenericArgumentDefinition("T7"));
		public static readonly BuiltinTypeDefinition OneOf8 = new BuiltinTypeDefinition("oneof", new GenericArgumentDefinition("T1"), new GenericArgumentDefinition("T2"), new GenericArgumentDefinition("T3"), new GenericArgumentDefinition("T4"), new GenericArgumentDefinition("T5"), new GenericArgumentDefinition("T6"), new GenericArgumentDefinition("T7"), new GenericArgumentDefinition("T8"));
		public static readonly BuiltinTypeDefinition OneOf9 = new BuiltinTypeDefinition("oneof", new GenericArgumentDefinition("T1"), new GenericArgumentDefinition("T2"), new GenericArgumentDefinition("T3"), new GenericArgumentDefinition("T4"), new GenericArgumentDefinition("T5"), new GenericArgumentDefinition("T6"), new GenericArgumentDefinition("T7"), new GenericArgumentDefinition("T8"), new GenericArgumentDefinition("T9"));

		public static readonly BuiltinTypeDefinition PagingOptions = new BuiltinTypeDefinition("PagingOptions");
		public static readonly BuiltinTypeDefinition Page = new BuiltinTypeDefinition("Page", new GenericArgumentDefinition("TItem"));

		#endregion
	}
}
