using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	public class MemoryLeakGallery : MasterDetailPage
	{
		List<WeakReference> wref = new List<WeakReference>();

		Label result;

		Button checkResult;

		[Preserve(AllMembers = true)]
		class LeakPage : ContentPage
		{
			public LeakPage(View content)
			{
				Content = content;
			}
		}

		Button MakeButton(string text, View content)
		{
			return new Button
			{
				Text = text,
				Command = new Command(async () =>
				{
					await Detail.Navigation.PushAsync(new LeakPage(content));
					var pageToRemove = Detail.Navigation.NavigationStack[1];
					wref.Add(new WeakReference(pageToRemove));
					await Task.Delay(200);
					Detail.Navigation.RemovePage(pageToRemove);

					result.Text = string.Empty;
				})
			};
		}

		public MemoryLeakGallery()
		{
			result = new Label
			{
				FontSize = 16,
				Text = ""
			};

			checkResult = new Button
			{
				Text = "Check Result",
				BackgroundColor = Color.DarkRed,
				TextColor = Color.White,
				Command = new Command(() =>
				{
					GC.Collect();
					GC.WaitForPendingFinalizers();
					GC.Collect();

					if (wref.Count > 1)
					{
						if (wref[wref.Count - 2].IsAlive)
						{
							result.Text = "Failed";
							result.TextColor = Color.Red;
						}
						else
						{
							result.Text = "Success";
							result.TextColor = Color.DarkGreen;
						}
					}
				})
			};

			if ((Device.Flags?.IndexOf(ExperimentalFlags.CollectionViewExperimental) ?? -1) == -1)
			{
				List<string> flags;
				if (Device.Flags != null)
					flags = new List<string>(Device.Flags);
				 else
					flags = new List<string>();
				flags.Add(ExperimentalFlags.CollectionViewExperimental);
				Device.SetFlags(flags.ToArray());
			}

			Master = new ContentPage {
				Title = "menu",
				Content = new StackLayout
				{
					Children =
					{
						new Label { Text = "Click on button twice, then check result" },
						result,
						MakeButton("Empty page", null),
						MakeButton("Entry", new Entry { Text = "Entry" }),
						MakeButton("Editor", new Editor { Text = "Editor" }),
						MakeButton("Image", new Image { BackgroundColor = Color.Azure, HeightRequest = 50 }),
						MakeButton("Button", new Button { Text = "Button" }),
						MakeButton("BoxView", new BoxView { Color = Color.Azure }),
						MakeButton("CarouselView Horizontal", new CarouselView
						{
							ItemsLayout = new ListItemsLayout(ItemsLayoutOrientation.Horizontal)
							{
								SnapPointsType = SnapPointsType.MandatorySingle,
								SnapPointsAlignment = SnapPointsAlignment.Center
							},
							ItemTemplate = ExampleTemplates.CarouselTemplate(),
						}),
						MakeButton("CarouselView Vertical", new CarouselView
						{
							ItemsLayout = new ListItemsLayout(ItemsLayoutOrientation.Vertical)
							{
								SnapPointsType = SnapPointsType.MandatorySingle,
								SnapPointsAlignment = SnapPointsAlignment.Center
							},
							ItemTemplate = ExampleTemplates.CarouselTemplate(),
						}),
						MakeButton("ActivityIndicator", new ActivityIndicator
						{
							Color = Color.Azure,
							BackgroundColor = Color.Black
						}),
						checkResult
					}
				}
			};

			Detail = new NavigationPage(new ContentPage());
		}
	}
}