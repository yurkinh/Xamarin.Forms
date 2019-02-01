using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Xamarin.Forms
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class ItemsSourceFactory
	{
		public static IItemsViewSource Create(IEnumerable itemsSource, Func<INotifyCollectionChanged, IItemsViewSource> observableItemsSourceFactory)
		{
			if (itemsSource == null)
				return new EmptySource();
		
			switch (itemsSource)
			{
				case IList _ when itemsSource is INotifyCollectionChanged:
					return observableItemsSourceFactory != null ? observableItemsSourceFactory(itemsSource as INotifyCollectionChanged) : new ObservableListSource(itemsSource);
				case IEnumerable<object> generic:
					return new ListSource(generic);
			}

			return new ListSource(itemsSource);
		}
	}

	internal class ListSource : List<object>, IItemsViewSource
	{
		public ListSource()
		{
		}

		public ListSource(IEnumerable<object> enumerable) : base(enumerable)
		{

		}

		public ListSource(IEnumerable enumerable)
		{
			foreach (object item in enumerable)
			{
				Add(item);
			}
		}
	}

	internal class ObservableListSource : ListSource
	{
		public ObservableListSource(IEnumerable enumerable)
		{
			foreach (object item in enumerable)
			{
				Add(item);
			}
		}
	}

	internal class EmptySource : IItemsViewSource
	{
		public int Count => 0;

		public object this[int index] => throw new IndexOutOfRangeException("IItemsViewSource is empty");
	}
}
