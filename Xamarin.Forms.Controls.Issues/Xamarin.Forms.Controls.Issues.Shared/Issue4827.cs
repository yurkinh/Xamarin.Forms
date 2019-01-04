using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;


#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4827, "[Android] Software backbutton doesn't work",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Navigation)]
#endif
	public class Issue4827 : TestNavigationPage
	{
		Label results = new Label();
		const string _success = "Success";

		public void Success()
		{
			results.Text = _success;
		}

		protected override void Init()
		{
			results.Text = "Testing...";

			PushAsync(new ContentPage()
			{
				Content = new StackLayout()
				{
					Children =
					{
						results,
						new Button()
						{
							Text = "Push Page",
							Command = new Command(PushSecondPage)
						}
					}
				}
			});

			PushSecondPage();
		}

		void PushSecondPage()
		{
			results.Text = "Testing... (If you're reading this the test has failed)";
			if(Device.RuntimePlatform == Device.iOS)
				results.Text = "This test doesn't work on iOS";

			PushAsync(new BackTestContentPage(this));
		}


		[Preserve(AllMembers = true)]
		public class BackTestContentPage : ContentPage
		{
			private readonly Issue4827 _issue4827;

			public BackTestContentPage(Issue4827 issue4827)
			{
				Content = new StackLayout()
				{
					Children =
					{
						new Label()
						{
							Text = "Click back. If you see text that says Success then test has passed"
						}
					}
				};
				_issue4827 = issue4827;
			}

			protected override bool OnBackButtonPressed()
			{
				_issue4827.Success();
				return base.OnBackButtonPressed();
			}
		}

#if UITEST && __ANDROID__
		[Test]
		public void UpdatingSourceOfDisposedListViewDoesNotCrash()
		{

		}
#endif
	}
}
