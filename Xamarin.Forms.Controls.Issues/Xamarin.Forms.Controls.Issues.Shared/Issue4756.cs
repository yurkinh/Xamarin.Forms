using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using Xamarin.Forms.Core.UITests;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.Shell)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4756, "Cannot prevent flyout menu list from scrolling", PlatformAffected.Default)]
	public class Issue4756 : TestShell
	{
		protected override void Init()
		{
			FlowDirection = FlowDirection.RightToLeft;
			FlyoutHeader = new StackLayout();
			FlyoutVerticalScroll = false;
			for (int i = 0; i < 20; i++)
				Items.Add(GenerateItem(i.ToString()));
		}

		ShellItem GenerateItem(string title)
		{
			var section = new ShellSection
			{
				Items =
				{
					new Forms.ShellContent
					{
						Content = new ContentPage
						{
							Content = new Button
							{
								Text = "Switch FlyoutVerticalScroll",
								Command = new Command(() => FlyoutVerticalScroll = !FlyoutVerticalScroll)
							}
						}
					}
				}
			};
			var item = new ShellItem
			{
				Title = title,
				Route = title,
				Items =
				{
					section
				}
			};
			item.CurrentItem = section;
			return item;
		}
	}
}