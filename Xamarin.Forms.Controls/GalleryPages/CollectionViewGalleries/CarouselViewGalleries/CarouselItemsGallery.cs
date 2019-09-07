﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.CarouselViewGalleries
{
	[Preserve(AllMembers = true)]
	public class CarouselItemsGallery : ContentPage
	{
		public CarouselItemsGallery()
		{
			var viewModel = new CarouselItemsGalleryViewModel();

			Title = $"CarouselView (Items)";

			var layout = new Grid
			{
				RowDefinitions = new RowDefinitionCollection
				{
					new RowDefinition { Height = GridLength.Star },
					new RowDefinition { Height = GridLength.Auto }
				}
			};

			var itemsLayout =
			new LinearItemsLayout(ItemsLayoutOrientation.Horizontal)
			{
				SnapPointsType = SnapPointsType.MandatorySingle,
				SnapPointsAlignment = SnapPointsAlignment.Center
			};

			var itemTemplate = GetCarouselTemplate();

			var carouselView = new CarouselView
			{
				ItemsLayout = itemsLayout,
				ItemTemplate = itemTemplate,
				ItemsSource = viewModel.Items,
				IsScrollAnimated = true,
				IsBounceEnabled = true
			};

			layout.Children.Add(carouselView, 0, 0);

			var stacklayoutButtons = new StackLayout
			{
				Orientation = StackOrientation.Horizontal
			};

			var addItemButton = new Button
			{
				Text = "Add Item"
			};

			addItemButton.Clicked += (sender, e) =>
			{
				viewModel.Items.Add(new CarouselData
				{
					Color = Color.Red,
					Name = $"{viewModel.Items.Count + 1}"
				});
				carouselView.Position = viewModel.Items.Count - 1;
			};

			var removeItemButton = new Button
			{
				Text = "Remove Item"
			};

			removeItemButton.Clicked += (sender, e) =>
			{
				if (viewModel.Items.Any())
					viewModel.Items.RemoveAt(viewModel.Items.Count - 1);

				if (viewModel.Items.Count > 0)
					carouselView.Position = viewModel.Items.Count - 1;
			};

			var clearItemsButton = new Button
			{
				Text = "Clear Items"
			};

			clearItemsButton.Clicked += (sender, e) =>
			{
				viewModel.Items.Clear();
			};

			stacklayoutButtons.Children.Add(addItemButton);
			stacklayoutButtons.Children.Add(removeItemButton);
			stacklayoutButtons.Children.Add(clearItemsButton);

			layout.Children.Add(stacklayoutButtons, 0, 1);

			Content = layout;
			BindingContext = viewModel;
		}

		internal DataTemplate GetCarouselTemplate()
		{
			return new DataTemplate(() =>
			{
				var grid = new Grid();

				var info = new Label
				{
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					Margin = new Thickness(6)
				};

				info.SetBinding(Label.TextProperty, new Binding("Name"));

				grid.Children.Add(info);

				var frame = new Frame
				{
					Content = grid,
					HasShadow = false
				};

				frame.SetBinding(BackgroundColorProperty, new Binding("Color"));

				return frame;
			});
		}
	}

	[Preserve(AllMembers = true)]
	public class CarouselItemsGalleryViewModel : BindableObject
	{
		ObservableCollection<CarouselData> _items;

		public CarouselItemsGalleryViewModel()
		{
			Items = new ObservableCollection<CarouselData>();

			var random = new Random();

			for (int n = 0; n < 5; n++)
			{
				_items.Add(new CarouselData
				{
					Color = Color.FromRgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)),
					Name = $"{n + 1}"
				});
			}
		}

		public ObservableCollection<CarouselData> Items
		{
			get { return _items; }
			set
			{
				_items = value;
				OnPropertyChanged();
			}
		}
	}
}
