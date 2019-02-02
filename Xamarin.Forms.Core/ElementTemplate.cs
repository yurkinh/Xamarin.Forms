using System;
using System.Collections.Generic;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms
{
	public class ElementTemplate : IElement
#pragma warning disable 612
		, IDataTemplate
#pragma warning restore 612
	{
		List<Action<object, ResourcesChangedEventArgs>> _changeHandlers;
		Element _parent;
		Func<object> _loadTemplate;

		internal ElementTemplate()
		{
		}

		internal ElementTemplate(Type type) : this()
		{
			if (type == null)
				throw new ArgumentNullException("type");

			CanRecycle = true;

			_loadTemplate = () => Activator.CreateInstance(type);
		}

		internal ElementTemplate(Func<object> loadTemplate) : this()
		{
			_loadTemplate = loadTemplate ?? throw new ArgumentNullException(nameof(loadTemplate));
		}

#pragma warning disable 0612
		Func<object> IDataTemplate.LoadTemplate
		{
			get { return _loadTemplate; }
			set { _loadTemplate = value; }
		}
#pragma warning restore 0612

		void IElement.AddResourcesChangedListener(Action<object, ResourcesChangedEventArgs> onchanged)
		{
			_changeHandlers = _changeHandlers ?? new List<Action<object, ResourcesChangedEventArgs>>(1);
			_changeHandlers.Add(onchanged);
		}

		internal bool CanRecycle { get; private set; }

		Element IElement.Parent
		{
			get { return _parent; }
			set
			{
				if (_parent == value)
					return;
				if (_parent != null)
					((IElement)_parent).RemoveResourcesChangedListener(OnResourcesChanged);
				_parent = value;
				if (_parent != null)
					((IElement)_parent).AddResourcesChangedListener(OnResourcesChanged);
			}
		}

		void IElement.RemoveResourcesChangedListener(Action<object, ResourcesChangedEventArgs> onchanged)
		{
			if (_changeHandlers == null)
				return;
			_changeHandlers.Remove(onchanged);
		}

		public object CreateContent() =>
			OnCreateContent(null, null);

		public object CreateContent(object item, BindableObject container) =>
			OnCreateContent(item, container);

		internal virtual object OnCreateContent(object item, BindableObject container)
		{
			if (_loadTemplate == null)
				throw new InvalidOperationException("LoadTemplate should not be null");

			return _loadTemplate();
		}

		void OnResourcesChanged(object sender, ResourcesChangedEventArgs e)
		{
			if (_changeHandlers == null)
				return;
			foreach (Action<object, ResourcesChangedEventArgs> handler in _changeHandlers)
				handler(this, e);
		}
	}
}