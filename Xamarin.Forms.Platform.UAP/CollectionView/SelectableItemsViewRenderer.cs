﻿using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using UWPListViewSelectionMode = Windows.UI.Xaml.Controls.ListViewSelectionMode;

namespace Xamarin.Forms.Platform.UWP
{
	public class SelectableItemsViewRenderer : StructuredItemsViewRenderer
	{
		SelectableItemsView _selectableItemsView;
		bool _ignoreNativeSelectionChange;

		protected override void TearDownOldElement(ItemsView oldElement)
		{
			var oldListViewBase = ListViewBase;
			if (oldListViewBase != null)
			{
				oldListViewBase.ClearValue(ListViewBase.SelectionModeProperty);
				oldListViewBase.SelectionChanged -= OnNativeSelectionChanged;
			}

			if (_selectableItemsView != null)
			{
				_selectableItemsView.SelectionChanged -= OnSelectionChanged;
			}

			base.TearDownOldElement(oldElement);
		}

		protected override void SetUpNewElement(ItemsView newElement)
		{
			base.SetUpNewElement(newElement);

			if (newElement == null)
			{
				return;
			}

			_selectableItemsView = newElement as SelectableItemsView;

			if (_selectableItemsView != null)
			{
				_selectableItemsView.SelectionChanged += OnSelectionChanged;
			}

			var newListViewBase = ListViewBase;

			if (newListViewBase != null)
			{
				newListViewBase.SetBinding(ListViewBase.SelectionModeProperty,
						new Windows.UI.Xaml.Data.Binding
						{
							Source = _selectableItemsView,
							Path = new Windows.UI.Xaml.PropertyPath("SelectionMode"),
							Converter = new SelectionModeConvert(),
							Mode = Windows.UI.Xaml.Data.BindingMode.TwoWay
						});

				newListViewBase.SelectionChanged += OnNativeSelectionChanged;
			}

			UpdateNativeSelection();
		}

		protected override void UpdateItemsSource()
		{
			_ignoreNativeSelectionChange = true;

			base.UpdateItemsSource();

			_ignoreNativeSelectionChange = false;
		}

		void UpdateNativeSelection()
		{
			_ignoreNativeSelectionChange = true;

			switch (ListViewBase.SelectionMode)
			{
				case UWPListViewSelectionMode.None:
					break;
				case UWPListViewSelectionMode.Single:
					if (_selectableItemsView != null)
					{
						if (_selectableItemsView.SelectedItem == null)
						{
							ListViewBase.SelectedItem = null;
						}
						else
						{
							ListViewBase.SelectedItem =
								ListViewBase.Items.First(item =>
								{
									if (item is ItemTemplateContext itemPair)
									{
										return itemPair.Item == _selectableItemsView.SelectedItem;
									}
									else
									{
										return item == _selectableItemsView.SelectedItem;
									}
								});
						}
					}
					
					break;
				case UWPListViewSelectionMode.Multiple:
					ListViewBase.SelectedItems.Clear();
					foreach (var nativeItem in ListViewBase.Items)
					{
						if (nativeItem is ItemTemplateContext itemPair && _selectableItemsView.SelectedItems.Contains(itemPair.Item))
						{
							ListViewBase.SelectedItems.Add(nativeItem);
						}
						else if (_selectableItemsView.SelectedItems.Contains(nativeItem))
						{
							ListViewBase.SelectedItems.Add(nativeItem);
						}
					}
					break;
				case UWPListViewSelectionMode.Extended:
					break;
				default:
					break;
			}

			_ignoreNativeSelectionChange = false;
		}

		void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			UpdateNativeSelection();
		}

		void OnNativeSelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
		{
			if (_ignoreNativeSelectionChange)
			{
				return;
			}

			if (Element is SelectableItemsView selectableItemsView)
			{
				switch (ListViewBase.SelectionMode)
				{
					case UWPListViewSelectionMode.None:
						break;
					case UWPListViewSelectionMode.Single:
						var selectedItem = 
							ListViewBase.SelectedItem is ItemTemplateContext itemPair ? itemPair.Item : ListViewBase.SelectedItem;
						selectableItemsView.SelectionChanged -= OnSelectionChanged;
						selectableItemsView.SetValueFromRenderer(SelectableItemsView.SelectedItemProperty, selectedItem);
						selectableItemsView.SelectionChanged += OnSelectionChanged;
						break;
					case UWPListViewSelectionMode.Multiple:
						selectableItemsView.SelectionChanged -= OnSelectionChanged;

						_selectableItemsView.SelectedItems.Clear();
						var selectedItems =
							ListViewBase.SelectedItems
								.Select(a =>
								{
									var item = a is ItemTemplateContext itemPair1 ? itemPair1.Item : a;
									return item;
								})
								.ToList();

						foreach (var item in selectedItems)
						{
							_selectableItemsView.SelectedItems.Add(item);
						}

						selectableItemsView.SelectionChanged += OnSelectionChanged;
						break;

					case UWPListViewSelectionMode.Extended:
						break;

					default:
						break;
				}
			}
		}

		class SelectionModeConvert : Windows.UI.Xaml.Data.IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, string language)
			{
				var formSelectionMode = (SelectionMode)value;
				switch (formSelectionMode)
				{
					case SelectionMode.None:
						return UWPListViewSelectionMode.None;
					case SelectionMode.Single:
						return UWPListViewSelectionMode.Single;
					case SelectionMode.Multiple:
						return UWPListViewSelectionMode.Multiple;
					default:
						return UWPListViewSelectionMode.None;
				}
			}

			public object ConvertBack(object value, Type targetType, object parameter, string language)
			{
				var uwpListViewSelectionMode = (UWPListViewSelectionMode)value;
				switch (uwpListViewSelectionMode)
				{
					case UWPListViewSelectionMode.None:
						return SelectionMode.None;
					case UWPListViewSelectionMode.Single:
						return SelectionMode.Single;
					case UWPListViewSelectionMode.Multiple:
						return SelectionMode.Multiple;
					case UWPListViewSelectionMode.Extended:
						return SelectionMode.None;
					default:
						return SelectionMode.None;
				}
			}
		}
	}
}
