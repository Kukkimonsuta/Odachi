namespace Odachi.Mail
{
	public abstract class MailTemplate : MailTemplateBase
	{
	}

	public abstract class MailTemplate<TModel> : MailTemplate
	{
		public TModel Model { get; set; }
	}
}
