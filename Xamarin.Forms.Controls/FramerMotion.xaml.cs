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

		double sliderValue;
		public double SliderValue
		{
			get => sliderValue;
			set
			{
				if (value == sliderValue)
					return;
				sliderValue = value;
				OnPropertyChanged();

				var storyBoard = (boxView.Resources["animateScale"] as Storyboard);
				if(storyBoard != null)
				{
					storyBoard.Duration = 100;
					storyBoard.Easing = Easing.BounceOut;
					Storyboard.SetTarget(storyBoard, boxView);
					storyBoard.Begin();
				}
			}
		}


	}
}