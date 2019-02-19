using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

[assembly: Visual("BetterSkia", typeof(SkiaSharpVisual.SkiaSharp))]
[assembly: Visual("BetterMaterialName", typeof(VisualMarker.MaterialVisual))]
namespace Xamarin.Forms.Sandbox
{
	public partial class App 
	{
		// This code is called from the App Constructor so just initialize the main page of the application here
		void InitializeMainPage()
		{
			MainPage = new NavigationPage(new XamlPage());
		}
	}
}
