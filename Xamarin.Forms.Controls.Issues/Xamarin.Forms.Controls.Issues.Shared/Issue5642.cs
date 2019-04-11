using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve (AllMembers=true)]
	[Issue (IssueTracker.Github, 5642, "[Material] Button Overlay Issue", PlatformAffected.Android)]
	public class Issue5642 : TestContentPage
	{
		protected override void Init()
		{
			var button = new Button
			{
				Visual = VisualMarker.Material,
				Text = "Button",
				BackgroundColor = Color.DarkBlue
			};			
			AbsoluteLayout.SetLayoutFlags(button, AbsoluteLayoutFlags.All);
			AbsoluteLayout.SetLayoutBounds(button, new Rectangle(0, 0, 1, 1));

			var label1 = new Label
			{
				Text = "Left",
				TextColor = Color.Red,
				BackgroundColor = Color.Transparent,
				Margin = new Thickness(10, 0)
			};
			AbsoluteLayout.SetLayoutFlags(label1, AbsoluteLayoutFlags.PositionProportional);
			AbsoluteLayout.SetLayoutBounds(label1, new Rectangle(0, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

			var label2 = new Label
			{
				Text = "Right",
				TextColor = Color.Red,
				BackgroundColor = Color.Transparent,
				Margin = new Thickness(10, 0)
			};
			AbsoluteLayout.SetLayoutFlags(label2, AbsoluteLayoutFlags.PositionProportional);
			AbsoluteLayout.SetLayoutBounds(label2, new Rectangle(1, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

			Content = new AbsoluteLayout
			{
				Padding = 0,
				BackgroundColor = Color.Transparent,
				MinimumHeightRequest = 50,
				VerticalOptions = LayoutOptions.Center,
				Children =
				{
					button,
					label1,
					label2
				}
			};
		}
	}
}
