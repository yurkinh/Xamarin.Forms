using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Embedding.XF;
using Xamarin.Forms.Platform.UWP;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

namespace Embedding.UWP
{
	public sealed partial class MainPage : Page
	{
		readonly Xamarin.Forms.ContentPage _page4;

		public MainPage()
		{
			InitializeComponent();

			HelloFlyout.Content = new Hello().CreateFrameworkElement();

			_page4 = new Page4();
		}

		void NavToUWPPage(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(Page2));
		}

		void NavToFormsPage4(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(_page4);
		}

		void NavToFormsPage3(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(new Page3());
		}

		void NavToAlertsAndActionSheets(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(new AlertsAndActionSheets());
		}

		async void NewWindow(object sender, RoutedEventArgs e)
		{
			CoreApplicationView newView = CoreApplication.CreateNewView();
			int newViewId = 0;
			await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				var frame = new Windows.UI.Xaml.Controls.Frame();
				frame.Navigate(typeof(Page2));
				Window.Current.Content = frame;
				// You have to activate the window in order to show it later.
				Window.Current.Activate();

				newViewId = ApplicationView.GetForCurrentView().Id;
			});
			bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
		}
	}
}