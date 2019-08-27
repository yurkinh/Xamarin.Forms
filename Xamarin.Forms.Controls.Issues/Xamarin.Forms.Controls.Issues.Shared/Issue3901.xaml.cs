using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
#if APP
	[Preserve (AllMembers = true)]
	[Issue (IssueTracker.Github, 3901, "Setting FontAttribute on a Span makes it ignore FontSize from the parent Label", PlatformAffected.Android | PlatformAffected.iOS)]
	public partial class Issue3901 : ContentPage
	{	
		public Issue3901 ()
		{
			InitializeComponent ();
		}
	}
#endif
}