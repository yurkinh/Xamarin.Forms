using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers=true)]
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FramerMotion : ContentPage
	{
		public FramerMotion()
		{
			InitializeComponent();
			BindingContext = this;
			boxView.PropertyChanged += Layout_PropertyChanged;
		}

		void Layout_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == VisualElement.WidthProperty.PropertyName
				 || e.PropertyName == VisualElement.HeightProperty.PropertyName)
			{
				if (boxView.Width > 0 && boxView.Height > 0)
				{
					sliderX.Maximum = layout.Width - boxView.Width;
					sliderY.Maximum = layout.Height - boxView.Height;
					X = (int)sliderX.Maximum / 2;
					Y = (int)sliderY.Maximum / 2;
					Device.StartTimer(TimeSpan.FromMilliseconds(2000), () => { Scale = 2; return false; });
				}
			}
		}

		double _scale = 0;
		public double Scale
		{
			get => _scale;
			set
			{
				var val = Math.Round(value, 2);
				if (Math.Abs(val - _scale) <= 0)
					return;
				_scale = val;
				OnPropertyChanged();
			}
		}

		double _rotation;
		public double Rotation
		{
			get => _rotation;
			set
			{
				var val = Math.Round(value, 2);
				if (Math.Abs(val - _rotation) <= 0)
					return;
				_rotation = val;
				OnPropertyChanged();
			}
		}

		int _x;
		public int X
		{
			get => _x;
			set
			{
				if (_x == value)
					return;
				_x = value;
				OnPropertyChanged();
			}
		}

		int _y;
		public int Y
		{
			get => _y;
			set
			{
				if (_y == value)
					return;
				_y = value;
				OnPropertyChanged();
			}
		}
	}
}