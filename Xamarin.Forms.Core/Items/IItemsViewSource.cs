using System.ComponentModel;

namespace Xamarin.Forms
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IItemsViewSource
	{
		int Count { get; }
		object this[int index] { get; }
	}
}
