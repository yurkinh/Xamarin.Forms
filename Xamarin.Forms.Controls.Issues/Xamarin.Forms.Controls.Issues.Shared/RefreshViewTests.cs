using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.RefreshView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.None, 0, "Refresh View Tests", PlatformAffected.All)]
	public class RefreshViewTests : TestContentPage
	{
		RefreshView _refreshView;
		public RefreshViewTests()
		{
		}

		protected override void Init()
		{
			Title = "Refresh View Tests";

			_refreshView = new RefreshView()
			{
				Content = new ScrollView(),
				Command = new Command(() =>
				{
					_refreshView.IsRefreshing = false;
				})
			};

			Content = new StackLayout()
			{
				Children =
				{
					new Button()
					{
						Text = "Toggle Refresh",
						Command = new Forms.Command(() =>
						{
							_refreshView.IsRefreshing = !_refreshView.IsRefreshing;
						})
					},
					_refreshView
				}
			};
		}
	}
}
