using System.Threading.Tasks;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 4069, "ArgumentOutOfRangeException in Android NavigationPageRenderer", 
		PlatformAffected.Android)]
	public class Issue4069 : TestNavigationPage 
	{
		protected override async void Init()
		{
			await Task.Delay(1000);
			PushAsync(new ContentPage { Content = new Label { Text = "Success" } });
		}

#if UITEST
		[Test]
		public void Issue4069Test()
		{
			RunningApp.WaitForElement("Success");
		}
#endif
	}
}