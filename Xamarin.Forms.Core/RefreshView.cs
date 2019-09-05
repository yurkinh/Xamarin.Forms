﻿using System;
using System.Windows.Input;
using Xamarin.Forms.Platform;

namespace Xamarin.Forms
{
	[ContentProperty("Content")]
	[RenderWith(typeof(_RefreshViewRenderer))]
	public class RefreshView : ContentView, IElementConfiguration<RefreshView>
	{
		readonly Lazy<PlatformConfigurationRegistry<RefreshView>> _platformConfigurationRegistry;

		public RefreshView()
		{
			IsClippedToBounds = true;
			VerticalOptions = LayoutOptions.FillAndExpand;
			HorizontalOptions = LayoutOptions.FillAndExpand;

			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<RefreshView>>(() => new PlatformConfigurationRegistry<RefreshView>(this));
		}

		public static readonly BindableProperty IsRefreshingProperty =
			BindableProperty.Create(nameof(IsRefreshing), typeof(bool), typeof(RefreshView), false, BindingMode.TwoWay);

		public bool IsRefreshing
		{
			get { return (bool)GetValue(IsRefreshingProperty); }
			set
			{
				if ((bool)GetValue(IsRefreshingProperty) == value)
					OnPropertyChanged(nameof(IsRefreshing));

				SetValue(IsRefreshingProperty, value);
			}
		}

		public static readonly BindableProperty CommandProperty =
			BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(RefreshView));

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public static readonly BindableProperty CommandParameterProperty =
			BindableProperty.Create(nameof(CommandParameter),
				typeof(object),
				typeof(RefreshView),
				null,
				propertyChanged: (bindable, oldvalue, newvalue) => ((RefreshView)bindable).RefreshCommandCanExecuteChanged(bindable, EventArgs.Empty));

		public object CommandParameter
		{
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		void RefreshCommandCanExecuteChanged(object sender, EventArgs eventArgs)
		{
			if (Command != null)
				IsEnabled = Command.CanExecute(CommandParameter);
		}

		public static readonly BindableProperty RefreshColorProperty =
			BindableProperty.Create(nameof(RefreshColor), typeof(Color), typeof(RefreshView), Color.Default);

		public Color RefreshColor
		{
			get { return (Color)GetValue(RefreshColorProperty); }
			set { SetValue(RefreshColorProperty, value); }
		}

		protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			if (Content == null)
				return new SizeRequest(new Size(100, 100));

			return base.OnMeasure(widthConstraint, heightConstraint);
		}

		public IPlatformElementConfiguration<T, RefreshView> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}
	}
}