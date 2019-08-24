using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.GalleryPages.CollectionViewGalleries.SelectionGalleries
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FilterSelection : ContentPage
	{
		DemoFilteredItemSource _demoFilteredItemSource = new DemoFilteredItemSource();

		public FilterSelection()
		{
			InitializeComponent();

			CollectionView.ItemsSource = _demoFilteredItemSource.Items;
			CollectionView.SelectionChanged += CollectionViewSelectionChanged;

			SearchBar.SearchCommand = new Command(() =>
			{
				_demoFilteredItemSource.FilterItems(SearchBar.Text);
			});

			ResetButton.Clicked += ResetButtonClicked;
		}

		private void CollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateSelectionInfo();
		}

		void ResetButtonClicked(object sender, EventArgs e)
		{
			_demoFilteredItemSource = new DemoFilteredItemSource(new Random().Next(3, 50));
			CollectionView.ItemsSource = _demoFilteredItemSource.Items;
		}

		void UpdateSelectionInfo()
		{
			var current = "[none]";

			if (CollectionView.SelectionMode == SelectionMode.Multiple)
			{
				current = CollectionView?.SelectedItems.ToCommaSeparatedList();
			}
			else if (CollectionView.SelectionMode == SelectionMode.Single)
			{
				current = ((CollectionViewGalleryTestItem)CollectionView?.SelectedItem)?.Caption;
			}

			CurrentSelection.Text = $"Selection: {current}";
		}
	}
}