using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.CodeModel.Builders.Abstraction
{
	public abstract class BuilderBase<TSelf, TItem>
		where TSelf : BuilderBase<TSelf, TItem>
	{
		public BuilderBase(PackageContext context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
		}

		public PackageContext Context { get; }

		public IDictionary<string, string> Hints { get; } = new Dictionary<string, string>();

		public TSelf Hint(string key, string value)
		{
			Hints[key] = value;

			return (TSelf)this;
		}

		public abstract TItem Build();
	}
}
