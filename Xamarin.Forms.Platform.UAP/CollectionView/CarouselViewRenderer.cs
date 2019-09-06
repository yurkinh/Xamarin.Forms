using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using UWPApp = Windows.UI.Xaml.Application;
using UWPDataTemplate = Windows.UI.Xaml.DataTemplate;

namespace Xamarin.Forms.Platform.UWP
{
	public class CarouselViewRenderer : ItemsViewRenderer
	{
		IItemsLayout _layout;

		public CarouselViewRenderer()
		{
			CollectionView.VerifyCollectionViewFlagEnabled(nameof(CarouselViewRenderer));
		}

		CarouselView CarouselView => (CarouselView)Element;
		UWPDataTemplate CarouselItemsViewTemplate => (UWPDataTemplate)UWPApp.Current.Resources["CarouselItemsViewDefaultTemplate"];

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs changedProperty)
		{
			base.OnElementPropertyChanged(sender, changedProperty);

			if (changedProperty.Is(ItemsView.ItemsSourceProperty))
				UpdateItemsSource();
			else if (changedProperty.Is(ItemsView.ItemTemplateProperty))
				UpdateItemTemplate();
			else if (changedProperty.Is(CarouselView.PeekAreaInsetsProperty))
				UpdatePeekAreaInsets();
			else if (changedProperty.Is(CarouselView.IsSwipeEnabledProperty))
				UpdateIsSwipeEnabled();
			else if (changedProperty.Is(CarouselView.IsBounceEnabledProperty))
				UpdateIsBounceEnabled();
		}

		protected override void SetUpNewElement(ItemsView newElement)
		{
			base.SetUpNewElement(newElement);

			_layout = newElement.ItemsLayout;

			UpdateItemsSource();
			UpdateItemTemplate();
			UpdatePeekAreaInsets();
			UpdateIsSwipeEnabled();
			UpdateIsBounceEnabled();
		}

		protected override void TearDownOldElement(ItemsView oldElement)
		{
			if (oldElement == null)
				return;

			if (_layout != null)
				_layout = null;
		}

		protected override ListViewBase SelectLayout(IItemsLayout layoutSpecification)
		{
			switch (layoutSpecification)
			{
				case ListItemsLayout listItemsLayout:
					return CreateCarouselListLayout(listItemsLayout.Orientation);
			}

			return base.SelectLayout(layoutSpecification);
		}

		protected override void UpdateItemTemplate()
		{
			if (Element == null || ListViewBase == null)
			{
				return;
			}

			ListViewBase.ItemTemplate = CarouselItemsViewTemplate;

			base.UpdateItemTemplate();
		}

		void UpdatePeekAreaInsets()
		{
			UpdateItemTemplate();
		}

		void UpdateIsSwipeEnabled()
		{
			if (CarouselView == null)
				return;

			ListViewBase.IsSwipeEnabled = CarouselView.IsSwipeEnabled;

			switch (_layout)
			{
				case ListItemsLayout listItemsLayout when listItemsLayout.Orientation == ItemsLayoutOrientation.Horizontal:
					ScrollViewer.SetHorizontalScrollMode(ListViewBase, CarouselView.IsSwipeEnabled ? ScrollMode.Auto : ScrollMode.Disabled);
					ScrollViewer.SetHorizontalScrollBarVisibility(ListViewBase, CarouselView.IsSwipeEnabled ? Windows.UI.Xaml.Controls.ScrollBarVisibility.Auto : Windows.UI.Xaml.Controls.ScrollBarVisibility.Disabled);
					break;
				case ListItemsLayout listItemsLayout when listItemsLayout.Orientation == ItemsLayoutOrientation.Vertical:
					ScrollViewer.SetVerticalScrollMode(ListViewBase, CarouselView.IsSwipeEnabled ? ScrollMode.Auto : ScrollMode.Disabled);
					ScrollViewer.SetVerticalScrollBarVisibility(ListViewBase, CarouselView.IsSwipeEnabled ? Windows.UI.Xaml.Controls.ScrollBarVisibility.Auto : Windows.UI.Xaml.Controls.ScrollBarVisibility.Disabled);
					break;
			}
		}

		void UpdateIsBounceEnabled()
		{
			var scrollViewer = ListViewBase.GetFirstDescendant<ScrollViewer>();

			if (scrollViewer != null)
				scrollViewer.IsScrollInertiaEnabled = CarouselView.IsBounceEnabled;
		}

		ListViewBase CreateCarouselListLayout(ItemsLayoutOrientation layoutOrientation)
		{
			Windows.UI.Xaml.Controls.ListView listView;

			if (layoutOrientation == ItemsLayoutOrientation.Horizontal)
			{
				listView = new Windows.UI.Xaml.Controls.ListView()
				{
					Style = (Windows.UI.Xaml.Style)UWPApp.Current.Resources["HorizontalCarouselListStyle"],
					ItemsPanel = (ItemsPanelTemplate)UWPApp.Current.Resources["HorizontalListItemsPanel"]
				};
			}
			else
			{
				listView = new Windows.UI.Xaml.Controls.ListView()
				{
					Style = (Windows.UI.Xaml.Style)UWPApp.Current.Resources["VerticalCarouselListStyle"]
				};
			}

			return listView;
		}
	}
}