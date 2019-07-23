using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FramerMotion : ContentPage
	{
		public FramerMotion()
		{
			InitializeComponent();
			BindingContext = this;
		}

		protected override void OnAppearing()
		{
			var storyBoard = (boxView.Resources["animateScale"] as Storyboard);
			if (storyBoard != null)
			{
				storyBoard.Duration = 300;
				storyBoard.Easing = Easing.SpringIn;
				Storyboard.SetTarget(storyBoard, boxView);
			}
			base.OnAppearing();
		}

		double _sliderValue = 1;
		public double SliderValue
		{
			get => _sliderValue;
			set
			{
				if (Math.Abs(value - _sliderValue) < 0)
					return;
				_sliderValue = value;
				OnPropertyChanged();
			}
		}

		double _rotation = 0;
		public double Rotation
		{
			get => _rotation;
			set
			{
				if(Math.Abs(value - _rotation) < 0)
					return;
				_rotation = value;
				OnPropertyChanged();
			}
		}

		int _x;
		public int X
		{
			get => _x;
			set
			{
				if (Math.Abs(value - _x) < 0)
					return;
				_x = value;
				OnPropertyChanged();

				StartStoryboard();
			}
		}

		readonly int y;
		public int Y
		{
			get => y;
			set
			{
				if (Math.Abs(value - y) < 0)
					return;
				_x = value;
				OnPropertyChanged();

				StartStoryboard();
			}
		}

		void StartStoryboard()
		{
			var storyBoard = (boxView.Resources["animateScale"] as Storyboard);
			if (storyBoard != null)
			{
				storyBoard.Duration = 130;
				storyBoard.Easing = Easing.SpringIn;
				Storyboard.SetTarget(storyBoard, boxView);
				storyBoard.Begin();
			}
		}
	}

	public class Div : BoxView
	{
		public Div()
		{
			AnchorX = 0.5;

			AnchorY = 0.5;

			CornerRadius = 25;

			BackgroundColor = Color.White;

			AbsoluteLayout.SetLayoutFlags(this, AbsoluteLayoutFlags.PositionProportional);
			AbsoluteLayout.SetLayoutBounds(this, new Rectangle(0.5, 0.5, 100, 100));
		}
	}
}