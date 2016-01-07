using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Localization.Extraction
{
	public class Invocation
	{
		public Invocation(string name, IReadOnlyList<Argument> arguments, SourceInfo? source = null)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			if (arguments == null)
				throw new ArgumentNullException(nameof(arguments));

			Name = name;
			Arguments = arguments;
			Source = source;
		}

		public string Name { get; private set; }
		public IReadOnlyList<Argument> Arguments { get; private set; }

		public SourceInfo? Source { get; set; }

		public override string ToString()
		{
			return string.Concat(
				Name,
				"(",
				string.Join(", ", Arguments),
				")"
			);
		}
	}

	public struct Argument
	{
		public Argument(string name, string value)
		{
			Name = name;
			Value = value;
		}

		public string Name { get; private set; }
		public string Value { get; private set; }

		public override string ToString()
		{
			var builder = new StringBuilder();

			if (Name != null)
			{
				builder.Append(Name);
				builder.Append(": ");
			}

			if (Value != null)
			{
				builder.Append("\"");
				builder.Append(Value);
				builder.Append("\"");
			}
			else
				builder.Append("<unsupported>");

			return builder.ToString();
		}
	}
}
