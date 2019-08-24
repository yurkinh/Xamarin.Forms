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
	[Category(UITestCategories.CollectionView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 7194, "Selection incorrect after ItemsSource change", PlatformAffected.All)]
	public class Issue7194 : TestNavigationPage
	{
		protected override void Init()
		{
#if APP
			FlagTestHelpers.SetCollectionViewTestFlag();
			PushAsync(new GalleryPages.CollectionViewGalleries.SelectionGalleries.FilterSelection());
#endif
		}

#if UITEST
		[Test]
		public void ItemsSourceChangeShouldClearSelection()
		{
			RunningApp.WaitForElement("Reset");
			
			RunningApp.Tap("Fruits.jpg, 4");	
			RunningApp.WaitForElement("Selection: Fruits.jpg, 4");

			RunningApp.Tap("Reset");	
			RunningApp.WaitForNoElement("Selection: Fruits.jpg, 4");
		}
#endif

	}
}
