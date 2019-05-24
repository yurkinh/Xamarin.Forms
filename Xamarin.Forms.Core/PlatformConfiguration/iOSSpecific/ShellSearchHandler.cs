using FormsElement = Xamarin.Forms.Shell;

namespace Xamarin.Forms.PlatformConfiguration.iOSSpecific
{
	public static class ShellSearchHandler
	{
		public static readonly BindableProperty PureSearchBackgroundEnabledProperty =
			BindableProperty.Create("PureSearchBackgroundEnabled", typeof(bool),
			typeof(ShellSearchHandler), true);

		public static bool GetSearchPureBackground(BindableObject element)
			=> (bool)element.GetValue(PureSearchBackgroundEnabledProperty);

		public static void SetSearchPureBackgroundEnabled(BindableObject element, bool value)
			=> element.SetValue(PureSearchBackgroundEnabledProperty, value);

		public static bool GetSearchPureBackgroundEnabled(this IPlatformElementConfiguration<iOS, FormsElement> config)
			=> (bool)config.Element.GetValue(PureSearchBackgroundEnabledProperty);

		public static IPlatformElementConfiguration<iOS, FormsElement> SetSearchPureBackgroundEnabled(
			this IPlatformElementConfiguration<iOS, FormsElement> config, bool value)
		{
			config.Element.SetValue(PureSearchBackgroundEnabledProperty, value);
			return config;
		}
	}
}
