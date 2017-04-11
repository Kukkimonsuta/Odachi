namespace Odachi.Mail
{
	public abstract class MailTemplate : MailTemplateBase
	{
		public string Subject { get; set; }
	}

	public abstract class MailTemplate<TModel> : MailTemplate
	{
		public TModel Model { get; set; }
	}
}
