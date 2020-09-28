namespace Odachi.Testing.Scenarios
{
	public static class ApplicationExtensions
	{
		public static TApplication UsePlugin<TApplication, TPlugin>(this TApplication application, TPlugin plugin)
			where TApplication : Application
			where TPlugin : ApplicationPlugin
		{
			application.Plugins.Add(plugin);

			return application;
		}
	}
}
