using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Sandbox
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShellPage : Shell
	{
		public ShellPage()
		{
			InitializeComponent();
		}

		void Button_Clicked(object sender, EventArgs e)
		{
			this.Items[1].IsEnabled = !this.Items[1].IsEnabled;
		}
	}
}