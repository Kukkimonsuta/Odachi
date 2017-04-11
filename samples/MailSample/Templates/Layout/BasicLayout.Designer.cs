namespace MailSample.Templates.Layout
{
    using System;
    using System.Threading.Tasks;

    public class BasicLayout : Odachi.Mail.MailTemplateLayout
    {
        #line hidden
        public BasicLayout()
        {
        }

        #pragma warning disable 1998
        public override async Task ExecuteAsync()
        {
            WriteLiteral("<!DOCTYPE html>\r\n<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n</head>\r\n<body>\r\n");
#line 8 "..\BasicLayout.cshtml"
	

#line default
#line hidden

#line 8 "..\BasicLayout.cshtml"
       RenderBody(); 

#line default
#line hidden

            WriteLiteral("</body>\r\n</html>");
        }
        #pragma warning restore 1998
    }
}
