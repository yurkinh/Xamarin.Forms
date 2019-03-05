using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4919, "Webview Navigation cancel not working", PlatformAffected.Android)]
	public class Issue4919 : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			var url = "https://www.microsoft.com/";
			var cancel = true;
			var webView = new WebView()
			{
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Source = url
			};
			webView.Navigating += (_, e) => e.Cancel = cancel;

			Content = new StackLayout
			{
				Children =
				{
					new Label { Text = "WebView must be empty on init" },
					webView,
					new Button
					{
						Text = "Go to github",
						Command = new Command(() => webView.Source = "https://github.com/xamarin/Xamarin.Forms")
					},
					new Button
					{
						Text = "Toggle cancel navigation",
						Command = new Command(() => cancel = !cancel)
					}
				}
			};
		}
	}
}