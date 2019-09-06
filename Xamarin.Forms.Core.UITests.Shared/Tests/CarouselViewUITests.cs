using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;

namespace Xamarin.Forms.Core.UITests
{
	[Category(UITestCategories.CarouselView)]
	internal class CarouselViewUITests : BaseTestFixture
	{
		string _enableCollectionView = "Enable CollectionView";
		string _carouselViewGalleries = "CarouselView Galleries";

		protected override void NavigateToGallery()
		{
			App.NavigateToGallery(GalleryQueries.CollectionViewGallery);

			App.WaitForElement(_enableCollectionView);
			App.Tap(_enableCollectionView);
			App.Tap(_carouselViewGalleries);
		}

		//protected override void TestTearDown()
		//{
		//	base.TestTearDown();
		//	ResetApp();
		//	NavigateToGallery();
		//}

		[TestCase("CarouselView (Code, Horizontal)")]
		//[TestCase("CarouselView (XAML, Horizontal)")]
		public void CarouselViewHorizontal(string subgallery)
		{
			VisitSubGallery(subgallery);

			App.WaitForElement("pos:1", "Did start on the correct position");
			var rect = App.Query(c => c.Marked("TheCarouselView")).First().Rect;
			App.DragCoordinates(rect.CenterX, rect.CenterY, rect.X + rect.Width - 1, rect.CenterY);
			App.WaitForElement("pos:0", "Did not scroll to first position");
			App.DragCoordinates(rect.CenterX, rect.CenterY, rect.X + 5, rect.CenterY);
			App.WaitForElement("pos:1", "Did not scroll to second position");

			App.Tap("Item: 1");

#if __ANDROID__
			Assert.AreEqual(App.Query(c => c.Class("AlertDialogLayout")).Count(), 1, "Alert not shown");
#elif __iOS__
			App.Query(c => c.ClassFull("_UIAlertControllerView"));
#endif

#if __ANDROID__

			App.Tap(c => c.Marked("Ok"));
#elif __iOS__
			App.Tap(c => c.Marked("OK").Parent().ClassFull("_UIAlertControllerView"));
#endif

			App.Tap("SwipeSwitch");

			App.DragCoordinates(rect.CenterX, rect.CenterY, rect.X + rect.Width - 1, rect.CenterY);

			App.WaitForNoElement("pos:0", "Swiped while swipe is disabled");
		}

		//[TestCase("CarouselView (Code, Vertical)")]
		public void CarouselViewVertical(string subgallery)
		{
			VisitSubGallery(subgallery);
			var rect = App.Query(c => c.Marked("TheCarouselView")).First().Rect;
			App.DragCoordinates(rect.CenterX, rect.CenterY, rect.CenterY, rect.Y + rect.Height - 1);
			//	App.ScrollUp(c => c.Marked("TheCarouselView"), ScrollStrategy.Gesture);

			App.WaitForElement("pos:0", "Did not scroll to first position");

			App.DragCoordinates(rect.CenterX, rect.CenterY, rect.CenterY, rect.Y - 1);
			//App.ScrollDown(c => c.Marked("TheCarouselView"), ScrollStrategy.Gesture);

			App.WaitForElement("pos:1", "Did not scroll to second position");

			App.Tap("Item: 1");

#if __ANDROID__
			Assert.AreEqual(App.Query(c => c.Class("AlertDialogLayout")).Count(), 1, "Alert not shown");
#elif __iOS__
			App.Query(c => c.ClassFull("_UIAlertControllerView"));
#endif

#if __ANDROID__

			App.Tap(c => c.Marked("Ok"));
#elif __iOS__
			App.Tap(c => c.Marked("OK").Parent().ClassFull("_UIAlertControllerView"));
#endif

			App.Tap("SwipeSwitch");

			App.DragCoordinates(rect.CenterX, rect.CenterY, rect.CenterY, rect.Y + rect.Height - 1);
			//App.ScrollUp(c => c.Marked("TheCarouselView"), ScrollStrategy.Gesture);

			App.WaitForNoElement("pos:0", "Swiped while swipe is disabled");
		}

		void VisitSubGallery(string galleryName)
		{
			App.WaitForElement(t => t.Marked(galleryName));
			App.Tap(t => t.Marked(galleryName));
		}
	}
}