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
		public static readonly BindableProperty IndicatorsTintColorProperty = BindableProperty.Create(nameof(IndicatorsTintColor), typeof(Color), typeof(IndicatorsView), Color.Silver);

		public Color IndicatorsTintColor
		{
			get { return (Color)GetValue(IndicatorsTintColorProperty); }
			set { SetValue(IndicatorsTintColorProperty, value); }
		}

		public static readonly BindableProperty SelectedIndicatorTintColorProperty = BindableProperty.Create(nameof(SelectedIndicatorTintColor), typeof(Color), typeof(IndicatorsView), Color.Gray);

		public Color SelectedIndicatorTintColor
		{
			get { return (Color)GetValue(SelectedIndicatorTintColorProperty); }
			set { SetValue(SelectedIndicatorTintColorProperty, value); }
		}

		public static readonly BindableProperty IndicatorsShapeProperty = BindableProperty.Create(nameof(IndicatorsShape), typeof(IndicatorsShape), typeof(IndicatorsView), IndicatorsShape.Circle);

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

		public static readonly BindableProperty ItemsSourceByProperty = BindableProperty.Create("ItemsSourceBy", typeof(CarouselView), typeof(IndicatorsView), default(CarouselView), propertyChanged: OnItemsSourceByPropertyChanged);

		[TypeConverter(typeof(ReferenceTypeConverter))]
		public static CarouselView GetItemsSourceBy(BindableObject bindable)
		{
			return (CarouselView)bindable.GetValue(ItemsSourceByProperty);
		}

		public static void SetItemsSourceBy(BindableObject bindable, CarouselView value)
		{
			bindable.SetValue(ItemsSourceByProperty, value);
		}

		protected IItemsViewSource ItemsSource;

		protected CarouselView CarouselItemsView => GetItemsSourceBy(this);

		static void OnPositionPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			var indicatorsView = (IndicatorsView)bindable;

			var args = new PositionChangedEventArgs((int)oldValue, (int)newValue);

			indicatorsView.PositionChanged?.Invoke(indicatorsView, args);
		}

		static void OnItemsSourceByPropertyChanged(object bindable, object oldValue, object newValue)
		{
			var oldCarouselView = (oldValue as CarouselView);

			var newCarouselView = (newValue as CarouselView);
			if (newCarouselView == null)
				return;

			var indicatorsView = (bindable as IndicatorsView);
			UpdateItemsSource(indicatorsView, newCarouselView.ItemsSource);

			newCarouselView.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName.Equals(nameof(CarouselView.Position)))
				{
					UpdatePosition(indicatorsView, s as CarouselView);
				}
				if (e.PropertyName.Equals(nameof(CarouselView.ItemsSource)))
				{
					UpdateItemsSource(indicatorsView, newCarouselView.ItemsSource);

				}
			};
		}

		static void UpdatePosition(IndicatorsView indicatorsView, CarouselView carouselView)
		{
			var newPosition = carouselView.Position;
			indicatorsView.SetValue(PositionProperty, newPosition);
		}

		static void UpdateItemsSource(IndicatorsView indicatorsView, IEnumerable itemsView)
		{
			indicatorsView.ItemsSource = ItemsSourceFactory.Create(itemsView, null);
			indicatorsView.SetValue(CountProperty, indicatorsView.ItemsSource.Count);
			UpdatePosition(indicatorsView, indicatorsView.CarouselItemsView);
		}
	}
}
