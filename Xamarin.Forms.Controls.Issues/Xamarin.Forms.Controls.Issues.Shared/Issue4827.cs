using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	// This test only passes because of an override in FormsAppCompatActivity passes the 
	// software back button press to the OnBackButtonPressed Command
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4827, "[Android] Software backbutton doesn't continue on device rotation",
		PlatformAffected.iOS)]
#if UITEST
	[NUnit.Framework.Category(UITestCategories.Navigation)]
#endif
	public class Issue4827 : TestNavigationPage
	{


		Label results = new Label();
		const string _success = "Success";
		const string _pushPageButton = "Push Page";
		const string _instructionsLabel = "_instructionsLabel";

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
							Text = _pushPageButton,
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
							Text = "Click back. If you see text that says Success then this test has passed",
							AutomationId = _instructionsLabel
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
		public void TestForSoftwareBackButtonTriggeringOnBackButtonPressed()
		{
			RunningApp.WaitForElement(_instructionsLabel);

			var softwareBackButton = RunningApp.Query(app => app.Marked("toolbar").Descendant()).Where(x=> x.Class.Contains("AppCompatImageButton")).FirstOrDefault();

			RunningApp.TapCoordinates(softwareBackButton.Rect.X, softwareBackButton.Rect.Y);
			RunningApp.WaitForElement(_success);
			RunningApp.SetOrientationLandscape();
			RunningApp.Tap(_pushPageButton);
			RunningApp.TapCoordinates(softwareBackButton.Rect.X, softwareBackButton.Rect.Y);
			RunningApp.WaitForElement(_success);
			RunningApp.SetOrientationPortrait();
			RunningApp.Tap(_pushPageButton);
			RunningApp.TapCoordinates(softwareBackButton.Rect.X, softwareBackButton.Rect.Y);
			RunningApp.WaitForElement(_success);
		}
#endif
	}
}
