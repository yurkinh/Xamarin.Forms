#if APP
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5268, "ListView with PullToRefresh enabled gestures conflict", PlatformAffected.Android)]
	public partial class Issue5268 : ContentPage
	{
		[Preserve(AllMembers = true)]
		public class SrcItem
		{
			public string Val { get; set; }
		}

		public ObservableCollection<SrcItem> Sources { get; }
		public ICommand Command { get; }

		public Issue5268()
		{
			InitializeComponent();
			Sources = new ObservableCollection<SrcItem>();
			Command = new Command(AddData);
			Sources.Add(new SrcItem { Val = "gstql!! - " + Sources.Count });
			MyListView.BindingContext = this;
		}

		void AddData()
		{
			IsBusy = true;
			Sources.Add(new SrcItem { Val = "gstql!! - " + Sources.Count });
			IsBusy = false;
		}
	}
}
#endif