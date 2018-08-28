using System;
using System.Reflection;
using System.Threading.Tasks;
using MailSample.Templates;
using Odachi.Mail;

namespace MailSample
{
	public class Program
    {
		public static async Task Main(string[] args)
		{
			var renderer = new MailTemplateRenderer();

			var message = await renderer.RenderAsync(new HelloMessage());
			
			Console.WriteLine(message.ToString());

			Console.ReadKey();
		}
    }
}
