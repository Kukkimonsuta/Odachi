using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odachi.CodeModel.Mapping
{
	public class BuiltinTypeDefinition : TypeDefinition
	{
		public BuiltinTypeDefinition(string name, params GenericArgumentDefinition[] genericArgumentDefinitions)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			Name = name;
			GenericArgumentDefinitions = genericArgumentDefinitions;
		}

		public override string Module { get; } = null;
		public override string Name { get; }
		public override IReadOnlyList<GenericArgumentDefinition> GenericArgumentDefinitions { get; }

		#region Static members

		public static readonly BuiltinTypeDefinition Void = new BuiltinTypeDefinition("void");
		public static readonly BuiltinTypeDefinition Boolean = new BuiltinTypeDefinition("boolean");
		public static readonly BuiltinTypeDefinition Integer = new BuiltinTypeDefinition("integer");
		public static readonly BuiltinTypeDefinition Long = new BuiltinTypeDefinition("long");
		public static readonly BuiltinTypeDefinition Float = new BuiltinTypeDefinition("float");
		public static readonly BuiltinTypeDefinition Double = new BuiltinTypeDefinition("double");
		public static readonly BuiltinTypeDefinition Decimal = new BuiltinTypeDefinition("decimal");
		public static readonly BuiltinTypeDefinition String = new BuiltinTypeDefinition("string");
		public static readonly BuiltinTypeDefinition DateTime = new BuiltinTypeDefinition("datetime");
		public static readonly BuiltinTypeDefinition Array = new BuiltinTypeDefinition("array", new GenericArgumentDefinition("T"));
		public static readonly BuiltinTypeDefinition File = new BuiltinTypeDefinition("file");

		public static readonly BuiltinTypeDefinition EntityReference = new BuiltinTypeDefinition("EntityReference");
		public static readonly BuiltinTypeDefinition PagingOptions = new BuiltinTypeDefinition("PagingOptions");
		public static readonly BuiltinTypeDefinition Page = new BuiltinTypeDefinition("Page", new GenericArgumentDefinition("TItem"));

		#endregion
	}
}
