using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Xamarin.Forms.Controls
{
	public partial class VSMTestPage : ContentPage
	{
		public VSMTestPage()
		{
			InitializeComponent();
		}

		void Handle_Clicked(object sender, System.EventArgs e)
		{
			entry.IsEnabled = false;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			Device.StartTimer(TimeSpan.FromSeconds(2), () =>
			{
				var storyboard = this.Resources["myStoryboard"] as Storyboard;
				storyboard.Begin();
				return false;
			});
		}
	}
}
