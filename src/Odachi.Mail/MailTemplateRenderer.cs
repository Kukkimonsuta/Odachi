using MimeKit;
using MimeKit.Text;
using MimeKit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Odachi.Mail
{
	public class MailTemplateRenderer
	{
		private async Task<string> RenderLayoutAsync(MailTemplateLayout layout, MailTemplateContext context, string body, IDictionary<string, Func<TextWriter, Task>> sections)
		{
			using (var output = new StringWriter())
			{
				layout.Context = context;
				layout.Output = output;

				layout.ChildBody = body;
				layout.ChildSections = sections;

				await layout.ExecuteAsync();
				await output.FlushAsync();

				layout.Output = null;

				string result;
				if (layout.Layout != null)
				{
					result = await RenderLayoutAsync(layout.Layout, context, output.ToString(), layout.Sections);
				}
				else
				{
					result = output.ToString();
				}

				return result;
			}
		}

		private async Task<string> RenderAsync(MailTemplate template, MailTemplateContext context)
		{
			using (var output = new StringWriter())
			{
				template.Context = context;
				template.Output = output;

				await template.ExecuteAsync();
				await output.FlushAsync();

				template.Output = null;

				string result;
				if (template.Layout != null)
				{
					result = await RenderLayoutAsync(template.Layout, context, output.ToString(), template.Sections);
				}
				else
				{
					result = output.ToString();
				}

				return result;
			}
		}

		public async Task<MimeMessage> RenderAsync(MailTemplate template)
		{
			var context = new MailTemplateContext();

			// render template
			var htmlBody = await RenderAsync(template, context);

			// construct message
			var message = new MimeMessage();

			if (context.Subject != null)
				message.Subject = context.Subject;

			var body = new Multipart("mixed");
			body.Add(new TextPart(TextFormat.Html)
			{
				Text = htmlBody,
			});
			foreach (var bodyPart in context.BodyParts)
			{
				body.Add(bodyPart);
			}
			message.Body = body;

			return message;
		}
	}
}
