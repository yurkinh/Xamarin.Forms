using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	public enum IndicatorsShape
	{
		Circle,
		Square
	}

	[RenderWith(typeof(_IndicatorsViewRenderer))]
	public class IndicatorsView : View
	{
		public static readonly BindableProperty IndicatorsTintColorProperty = BindableProperty.Create(nameof(IndicatorsTintColor), typeof(Color), typeof(CarouselView), Color.Silver);

		public Color IndicatorsTintColor
		{
			get { return (Color)GetValue(IndicatorsTintColorProperty); }
			set { SetValue(IndicatorsTintColorProperty, value); }
		}

		public static readonly BindableProperty SelectedIndicatorTintColorProperty = BindableProperty.Create(nameof(SelectedIndicatorTintColor), typeof(Color), typeof(CarouselView), Color.Gray);

		public Color SelectedIndicatorTintColor
		{
			get { return (Color)GetValue(SelectedIndicatorTintColorProperty); }
			set { SetValue(SelectedIndicatorTintColorProperty, value); }
		}

		public static readonly BindableProperty IndicatorsShapeProperty = BindableProperty.Create(nameof(IndicatorsShape), typeof(IndicatorsShape), typeof(CarouselView), IndicatorsShape.Circle);

		public IndicatorsShape IndicatorsShape
		{
			get { return (IndicatorsShape)GetValue(IndicatorsShapeProperty); }
			set { SetValue(IndicatorsShapeProperty, value); }
		}

		public static readonly BindableProperty CountProperty = BindableProperty.Create(nameof(Count), typeof(int), typeof(IndicatorsView), default(int));

		public int Count
		{
			get => (int)GetValue(CountProperty);
			set => SetValue(CountProperty, value);
		}

		public static readonly BindableProperty PositionProperty = BindableProperty.Create(nameof(Position), typeof(int), typeof(IndicatorsView), default(int), BindingMode.TwoWay, propertyChanged: OnPositionPropertyChanged);

		public int Position
		{
			get => (int)GetValue(PositionProperty);
			set => SetValue(PositionProperty, value);
		}

		public event EventHandler<PositionChangedEventArgs> PositionChanged;

		public static readonly BindableProperty ItemsSourceByProperty = BindableProperty.Create("ItemsSourceBy", typeof(SelectableItemsView), typeof(IndicatorsView), default(SelectableItemsView), propertyChanged: OnItemsSourceByPropertyChanged);

		[TypeConverter(typeof(ReferenceTypeConverter))]
		public static SelectableItemsView GetItemsSourceBy(BindableObject bindable)
		{
			return (SelectableItemsView)bindable.GetValue(ItemsSourceByProperty);
		}

		public static void SetItemsSourceBy(BindableObject bindable, ItemsView value)
		{
			bindable.SetValue(ItemsSourceByProperty, value);
		}

		protected IItemsViewSource ItemsSource;

		protected SelectableItemsView ItemsView => GetItemsSourceBy(this);

		static void OnPositionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var indicatorsView = (IndicatorsView)bindable;

			var args = new PositionChangedEventArgs((int)oldValue, (int)newValue);

			indicatorsView.PositionChanged?.Invoke(indicatorsView, args);
		}

		static void OnItemsSourceByPropertyChanged(object bindable, object oldValue, object newValue)
		{
			var oldItemsView = (oldValue as SelectableItemsView);

			var newSelectableItemsView = (newValue as SelectableItemsView);
			if (newSelectableItemsView == null)
				return;

			var indicatorsView = (bindable as IndicatorsView);
			UpdateFromItemsSource(indicatorsView, newSelectableItemsView.ItemsSource);

			newSelectableItemsView.PropertyChanged += (s, e) =>
			{
				//TODO: rmarinho Validate this only works with SelectionMode single
				if (e.PropertyName.Equals(nameof(SelectableItemsView.SelectedItem)))
				{
					UpdatePositionFromSelectedItem(indicatorsView, s as SelectableItemsView);
				}
				if (e.PropertyName.Equals(nameof(SelectableItemsView.ItemsSource)))
				{
					UpdateFromItemsSource(indicatorsView, newSelectableItemsView.ItemsSource);

				}
			};
		}

		static void UpdatePositionFromSelectedItem(IndicatorsView indicatorsView, SelectableItemsView selectableItemsView)
		{
			var selectedItem = selectableItemsView?.SelectedItem;
			var newPosition = indicatorsView.GetPositionForItem(selectedItem);
			indicatorsView.SetValue(PositionProperty, newPosition);
		}

		static void UpdateFromItemsSource(IndicatorsView indicatorsView, IEnumerable itemsView)
		{
			indicatorsView.ItemsSource = ItemsSourceFactory.Create(itemsView, null);
			indicatorsView.SetValue(CountProperty, indicatorsView.ItemsSource.Count);
			UpdatePositionFromSelectedItem(indicatorsView, indicatorsView.ItemsView);
		}

		int GetPositionForItem(object item)
		{
			for (int n = 0; n < Count; n++)
			{
				if (ItemsSource[n] == item)
				{
					return n;
				}
			}
			return -1;
		}
	}
}
