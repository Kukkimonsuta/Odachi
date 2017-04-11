using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Odachi.Mail;
using MailSample.Templates;
using System.Reflection;

namespace MailSample
{
    public class Program
    {
		public static int Main(string[] args)
		{
			var renderer = new MailTemplateRenderer();

			foreach (var n in typeof(Program).GetTypeInfo().Assembly.GetManifestResourceNames())
				Console.WriteLine(n);

			var message = renderer.RenderAsync(new HelloMessage()).GetAwaiter().GetResult();

			Console.WriteLine(message.ToString());

			Console.ReadKey();

			return 0;
		}
    }
}
