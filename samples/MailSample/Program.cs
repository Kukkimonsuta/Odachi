using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Odachi.Mail;
using MailSample.Templates;

namespace MailSample
{
    public class Program
    {
		public static int Main(string[] args)
		{
			var renderer = new MailTemplateRenderer();

			var task = renderer.RenderAsync(new HelloMessage());
			task.Wait();

			Console.WriteLine(task.Result);

			Console.ReadKey();

			return 0;
		}
    }
}
