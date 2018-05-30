using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve (AllMembers=true)]
	[Issue (IssueTracker.Github, 2832, "AccessViolationException when added items in OnItemAppearing", PlatformAffected.UWP)]
	public class Issue2832: TestContentPage
	{
		ObservableCollection<string> items;
		const int maxItems = 10;

		protected override void Init()
		{
			items = new ObservableCollection<string>() { "first" };

			ListView list = new ListView { ItemsSource = items };

			list.ItemAppearing += OnItemAppearing;

			Content = new StackLayout
			{
				Children = { list }
			};
		}

		void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
		{
			if (items.Count < maxItems)
				items.Add("item");
		}
	}
}