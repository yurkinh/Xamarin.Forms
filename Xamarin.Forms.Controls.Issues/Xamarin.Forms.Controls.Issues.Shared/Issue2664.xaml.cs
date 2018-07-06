using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Issues
{
	[XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class Issue2664 : ContentPage
	{
		public Issue2664 ()
		{
			InitializeComponent ();
		}
	}
}