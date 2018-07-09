using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Issues
{
#if APP
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 2664, "", PlatformAffected.Android)]
	[XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class Issue2664 : ContentPage
	{
		public Issue2664 ()
		{
			InitializeComponent ();
		}
	}
#endif
}