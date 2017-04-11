using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Mail
{
	public class MailTemplateRenderer
	{
		private async Task<string> RenderLayoutAsync(MailTemplateLayout layout, string body, IDictionary<string, Func<TextWriter, Task>> sections)
		{
			using (var output = new StringWriter())
			{
				layout.Output = output;

				layout.ChildBody = body;
				layout.ChildSections = sections;

				await layout.ExecuteAsync();
				await output.FlushAsync();

				layout.Output = null;

				string result;
				if (layout.Layout != null)
				{
					result = await RenderLayoutAsync(layout.Layout, output.ToString(), layout.Sections);
				}
				else
				{
					result = output.ToString();
				}

				return result;
			}
		}

		public async Task<string> RenderAsync(MailTemplate template)
		{
			using (var output = new StringWriter())
			{
				template.Output = output;

				await template.ExecuteAsync();
				await output.FlushAsync();

				template.Output = null;

				string result;
				if (template.Layout != null)
				{
					result = await RenderLayoutAsync(template.Layout, output.ToString(), template.Sections);
				}
				else
				{
					result = output.ToString();
				}

				return result;
			}
		}
	}
}
