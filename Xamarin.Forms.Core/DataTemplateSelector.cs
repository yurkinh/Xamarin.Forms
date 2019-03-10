using System;
using System.Collections.Generic;

namespace Xamarin.Forms
{
	public abstract class DataTemplateSelector : DataTemplate
	{
		readonly Dictionary<Type, DataTemplate> _dataTemplates = new Dictionary<Type, DataTemplate>();

		// NOTE: we don't call base as this is a proxy object that does not need to be bound
		//       and we do not actually make use of any of the base functionality
		internal override object OnCreateContent(object item, BindableObject container) =>
			SelectTemplate(item, container).CreateContent();

		public DataTemplate SelectTemplate(object item, BindableObject container)
		{
			var recycle = false;
			if (container is ListView listView)
				recycle = listView.CachingStrategy.HasFlag(ListViewCachingStrategy.RecycleElementAndDataTemplate);

			if (recycle && _dataTemplates.TryGetValue(item.GetType(), out var dataTemplate))
				return dataTemplate;

			dataTemplate = OnSelectTemplate(item, container);
			if (dataTemplate is DataTemplateSelector)
				throw new NotSupportedException(
					"DataTemplateSelector.OnSelectTemplate must not return another DataTemplateSelector");

			if (recycle)
			{
				if (!dataTemplate.CanRecycle)
					throw new NotSupportedException(
						"RecycleElementAndDataTemplate requires DataTemplate activated with ctor taking a type.");

				_dataTemplates[item?.GetType()] = dataTemplate;
			}

			return dataTemplate;
		}

		protected abstract DataTemplate OnSelectTemplate(object item, BindableObject container);
	}
}
