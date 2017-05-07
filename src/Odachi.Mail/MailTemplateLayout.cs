using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Odachi.Mail
{
	public abstract class MailTemplateLayout : MailTemplateBase
	{
		internal string ChildBody { get; set; }
		internal IDictionary<string, Func<TextWriter, Task>> ChildSections { get; set; }

		protected void RenderBody()
		{
			WriteLiteral(ChildBody);
		}

		protected Task RenderSectionAsync(string name)
		{
			if (Output == null)
				throw new InvalidOperationException("No output writer defined");

			if (ChildSections == null || !ChildSections.TryGetValue(name, out var section))
				throw new InvalidOperationException($"Section '{name}' was not found");

			return section(Output);
		}
	}
}
