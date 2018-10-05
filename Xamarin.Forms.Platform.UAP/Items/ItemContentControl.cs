using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.UWP
{
	public class ItemContentControl : ContentControl
	{
		public ItemContentControl()
		{
			DefaultStyleKey = typeof(ItemContentControl);
		}

		public static readonly DependencyProperty FormsDataTemplateProperty = DependencyProperty.Register(
			nameof(FormsDataTemplate), typeof(DataTemplate), typeof(ItemContentControl), 
			new PropertyMetadata(default(DataTemplate), FormsDataTemplateChanged));

		static void FormsDataTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue == null)
			{
				return;
			}

			var formsContentControl = (ItemContentControl)d;
			//Debug.WriteLine($"ItemContentControl FormsDataTemplate changed");
			formsContentControl.RealizeFormsDataTemplate((DataTemplate)e.NewValue);
		}

		public DataTemplate FormsDataTemplate
		{
			get => (DataTemplate)GetValue(FormsDataTemplateProperty);
			set => SetValue(FormsDataTemplateProperty, value);
		}

		public static readonly DependencyProperty FormsDataContextProperty = DependencyProperty.Register(
			nameof(FormsDataContext), typeof(object), typeof(ItemContentControl), 
			new PropertyMetadata(default(object), FormsDataContextChanged));

		static void FormsDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var formsContentControl = (ItemContentControl)d;
			//Debug.WriteLine($"ItemContentControl FormsDataContext changed (to {e.NewValue})");
			formsContentControl.SetFormsDataContext(e.NewValue);
		}

		public object FormsDataContext
		{
			get => GetValue(FormsDataContextProperty);
			set => SetValue(FormsDataContextProperty, value);
		}

		VisualElement _rootElement;

		internal void RealizeFormsDataTemplate(DataTemplate template)
		{
			var content = FormsDataTemplate.CreateContent();

			if (content is VisualElement visualElement)
			{
				if (_rootElement != null)
				{
					_rootElement.MeasureInvalidated -= RootElementOnMeasureInvalidated;
				}

				_rootElement = visualElement;
				_rootElement.MeasureInvalidated += RootElementOnMeasureInvalidated;

				_rootElement.Parent = CollectionViewRenderer.CollectionViewParent;

				Content = Platform.CreateRenderer(visualElement).ContainerElement;
			}

			if (FormsDataContext != null)
			{
				SetFormsDataContext(FormsDataContext);
			}
		}

		void RootElementOnMeasureInvalidated(object sender, EventArgs e)
		{
			InvalidateMeasure();
		}

		internal void SetFormsDataContext(object context)
		{
			//Debug.WriteLine($"Setting data context to {FormsDataContext}");
			if (_rootElement == null)
			{
				//Debug.WriteLine($"But _rootElement was null!");
				return;
			}

			//Debug.WriteLine($"Setting inherited binding context to {context}");

			BindableObject.SetInheritedBindingContext(_rootElement, context);
			_rootElement.InvalidateMeasureNonVirtual(InvalidationTrigger.MeasureChanged);
		}

		protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
		{
			if (FormsDataContext != null && FormsDataContext.ToString().Contains("November 16"))
			{
				Debug.WriteLine("November 16 being measured");
				if (_rootElement == null)
				{
					Debug.WriteLine("But root element is null");
				}
			}

			if (_rootElement == null)
			{
				return base.MeasureOverride(availableSize);
			}

			Size request = _rootElement.Measure(availableSize.Width, availableSize.Height, 
				MeasureFlags.IncludeMargins).Request;

			_rootElement.Layout(new Rectangle(Point.Zero, request));

			return new Windows.Foundation.Size(request.Width, request.Height); 
		}

		protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
		{
			if (FormsDataContext != null && FormsDataContext.ToString().Contains("November 16"))
			{
				Debug.WriteLine("November 16 being arranged");
				if (!(Content is FrameworkElement))
				{
					Debug.WriteLine("But Content is null or not a framework element");
				}
			}

			if (!(Content is FrameworkElement frameworkElement))
			{
				return finalSize;
			}
		
			frameworkElement.Arrange(new Rect(_rootElement.X, _rootElement.Y, _rootElement.Width, _rootElement.Height));
			return base.ArrangeOverride(finalSize);
		}
	}
}