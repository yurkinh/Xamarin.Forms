using System.ComponentModel;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class DataTemplateExtensions
	{
		public static DataTemplate SelectDataTemplate(this DataTemplate self, object item, BindableObject container)
		{
			if (self is DataTemplateSelector selector)
				return selector.SelectTemplate(item, container);

			return self;
		}
	}
}