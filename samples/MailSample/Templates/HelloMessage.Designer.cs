namespace MailSample.Templates
{
    using System;
    using System.Threading.Tasks;

    public class HelloMessage : Odachi.Mail.MailTemplate
    {
        #line hidden
        public HelloMessage()
        {
        }

        #pragma warning disable 1998
        public override async Task ExecuteAsync()
        {
#line 2 "..\HelloMessage.cshtml"
  
	Layout = new MailSample.Templates.Layout.BasicLayout();

#line default
#line hidden

            WriteLiteral("<p>\r\n\tHello world!\r\n</p>");
        }
        #pragma warning restore 1998
    }
}
