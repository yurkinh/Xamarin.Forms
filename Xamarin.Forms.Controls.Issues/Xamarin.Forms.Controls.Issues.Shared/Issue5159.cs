using System;
using Xamarin.Forms.Internals;
using Xamarin.Forms.CustomAttributes;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5159, "[Android] Calling Focus on all Pickers running an API 28 devices no longer opens Picker", PlatformAffected.Android)]
	public class Issue5159 : TestContentPage
	{
		protected override void Init()
		{
			var stackLayout = new StackLayout
			{
				VerticalOptions = LayoutOptions.Center,
				HorizontalOptions = LayoutOptions.Center
			};

			var button = new Button
			{
				Text = "Show picker"
			};

			var picker = new DatePicker
			{
				IsVisible = false
			};

			button.Clicked += (s, a) =>
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					if (picker.IsFocused)
						picker.Unfocus();

					picker.Focus();
				});
			};

			stackLayout.Children.Add(button);
			stackLayout.Children.Add(picker);

			Content = stackLayout;
		}
	}
}
