using System;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 20090125, "Get height of the status bar", PlatformAffected.iOS | PlatformAffected.Android)]
	public class StatusBarHeightTest : TestContentPage // or TestMasterDetailPage, etc ...
	{
		protected override void Init()
		{
			// Initialize ui here instead of ctor
			Content = new Label
			{
				AutomationId = "StatusBarHeight",
				Text = $"The status bar is {Device.Info.StatusBarHeight} units tall. "
					+ Environment.NewLine + "iOS should be 44."
					+ Environment.NewLine + "Android should be 24.",
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
		}
	}
}