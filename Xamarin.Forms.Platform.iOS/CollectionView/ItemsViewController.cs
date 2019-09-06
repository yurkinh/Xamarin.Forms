﻿using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.iOS
{
	// TODO hartez 2018/06/01 14:21:24 Add a method for updating the layout	
	public class ItemsViewController : UICollectionViewController
	{
		public IItemsViewSource ItemsSource { get; protected set; }
		public ItemsView ItemsView { get; }
		protected ItemsViewLayout ItemsViewLayout { get; set; }
		bool _initialConstraintsSet;
		bool _isEmpty;
		bool _currentBackgroundIsEmptyView;
		bool _disposed;

		UIView _backgroundUIView;
		UIView _emptyUIView;
		VisualElement _emptyViewFormsElement;

		protected UICollectionViewDelegator Delegator { get; set; }

		public ItemsViewController(ItemsView itemsView, ItemsViewLayout layout) : base(layout)
		{
			ItemsView = itemsView;
			ItemsSource = CreateItemsViewSource();

			UpdateLayout(layout);
		}

		public void UpdateLayout(ItemsViewLayout layout)
		{
			ItemsViewLayout = layout;
			ItemsViewLayout.GetPrototype = GetPrototype;

			// If we're updating from a previous layout, we should keep any settings for the SelectableItemsViewController around
			var selectableItemsViewController = Delegator?.SelectableItemsViewController;
			Delegator = new UICollectionViewDelegator(ItemsViewLayout, this);

			CollectionView.Delegate = Delegator;

			if (CollectionView.CollectionViewLayout != ItemsViewLayout)
			{
				// We're updating from a previous layout

				// Make sure the new layout is sized properly
				ItemsViewLayout.ConstrainTo(CollectionView.Bounds.Size);

				CollectionView.SetCollectionViewLayout(ItemsViewLayout, false);

				// Reload the data so the currently visible cells get laid out according to the new layout
				CollectionView.ReloadData();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				ItemsSource?.Dispose();
				_emptyUIView?.Dispose();
				_emptyUIView = null;
				_emptyViewFormsElement = null;
			}

			_disposed = true;

			base.Dispose(disposing);
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.DequeueReusableCell(DetermineCellReuseId(), indexPath) as UICollectionViewCell;

			switch (cell)
			{
				case DefaultCell defaultCell:
					UpdateDefaultCell(defaultCell, indexPath);
					break;
				case TemplatedCell templatedCell:
					UpdateTemplatedCell(templatedCell, indexPath);
					break;
			}

			return cell;
		}

		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			var count = ItemsSource.ItemCountInGroup(section);

			CheckForEmptySource();

			return count;
		}

		void CheckForEmptySource()
		{
			var wasEmpty = _isEmpty;

			_isEmpty = ItemsSource.ItemCount == 0;

			if (wasEmpty != _isEmpty)
			{
				UpdateEmptyViewVisibility(_isEmpty);
			}
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			AutomaticallyAdjustsScrollViewInsets = false;
			RegisterViewTypes();
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();

			// We can't set this constraint up on ViewDidLoad, because Forms does other stuff that resizes the view
			// and we end up with massive layout errors. And View[Will/Did]Appear do not fire for this controller
			// reliably. So until one of those options is cleared up, we set this flag so that the initial constraints
			// are set up the first time this method is called.
			if (!_initialConstraintsSet)
			{
				ItemsViewLayout.ConstrainTo(CollectionView.Bounds.Size);
				_initialConstraintsSet = true;
			}
		}

		protected virtual IItemsViewSource CreateItemsViewSource()
		{
			return ItemsSourceFactory.Create(ItemsView.ItemsSource, CollectionView);
		}

		public virtual void UpdateItemsSource()
		{
			ItemsSource = CreateItemsViewSource();
			CollectionView.ReloadData();
			CollectionView.CollectionViewLayout.InvalidateLayout();
		}

		public override nint NumberOfSections(UICollectionView collectionView)
		{
			CheckForEmptySource();
			return ItemsSource.GroupCount;
		}

		protected virtual void UpdateDefaultCell(DefaultCell cell, NSIndexPath indexPath)
		{
			cell.Label.Text = ItemsSource[indexPath].ToString();

			if (cell is ItemsViewCell constrainedCell)
			{
				ItemsViewLayout.PrepareCellForLayout(constrainedCell);
			}
		}

		protected virtual void UpdateTemplatedCell(TemplatedCell cell, NSIndexPath indexPath)
		{
			ApplyTemplateAndDataContext(cell, indexPath);

			if (cell is ItemsViewCell constrainedCell)
			{
				ItemsViewLayout.PrepareCellForLayout(constrainedCell);
			}
		}

		public virtual NSIndexPath GetIndexForItem(object item)
		{
			return ItemsSource.GetIndexForItem(item);
		}

		protected object GetItemAtIndex(NSIndexPath index)
		{
			return ItemsSource[index];
		}

		void ApplyTemplateAndDataContext(TemplatedCell cell, NSIndexPath indexPath)
		{
			var template = ItemsView.ItemTemplate;
			var item = ItemsSource[indexPath];

			// Run this through the extension method in case it's really a DataTemplateSelector
			template = template.SelectDataTemplate(item, ItemsView);

			// Create the content and renderer for the view and 
			var view = template.CreateContent() as View;
			var renderer = CreateRenderer(view);
			cell.SetRenderer(renderer);

			// Bind the view to the data item
			view.BindingContext = ItemsSource[indexPath];

			// And make sure it's a "child" of the ItemsView
			ItemsView.AddLogicalChild(view);

			cell.ContentSizeChanged += CellContentSizeChanged;
		}

		void CellContentSizeChanged(object sender, EventArgs e)
		{
			if (_disposed)
				return;

			Layout?.InvalidateLayout();
		}

		internal void PrepareCellForRemoval(UICollectionViewCell cell)
		{
			if (cell is TemplatedCell templatedCell)
			{
				templatedCell.ContentSizeChanged -= CellContentSizeChanged;

				var oldView = templatedCell.VisualElementRenderer?.Element;
				if (oldView != null)
				{
					oldView.BindingContext = null;
					ItemsView.RemoveLogicalChild(oldView);
				}

				templatedCell.PrepareForRemoval();
			}
		}

		protected IVisualElementRenderer CreateRenderer(View view)
		{
			if (view == null)
			{
				throw new ArgumentNullException(nameof(view));
			}

			var renderer = Platform.CreateRenderer(view);
			Platform.SetRenderer(view, renderer);

			return renderer;
		}

		protected virtual string DetermineCellReuseId()
		{
			if (ItemsView.ItemTemplate != null)
			{
				return ItemsViewLayout.ScrollDirection == UICollectionViewScrollDirection.Horizontal
					? HorizontalTemplatedCell.ReuseId
					: VerticalTemplatedCell.ReuseId;
			}

			return ItemsViewLayout.ScrollDirection == UICollectionViewScrollDirection.Horizontal
				? HorizontalDefaultCell.ReuseId
				: VerticalDefaultCell.ReuseId;
		}

		UICollectionViewCell GetPrototype()
		{
			if (ItemsSource.ItemCount == 0)
			{
				return null;
			}

			var group = 0;

			if (ItemsSource.GroupCount > 1)
			{
				// If we're in a grouping situation, then we need to make sure we find an actual data item
				// to use for our prototype cell. It's possible that we have empty groups.
				for (int n = 0; n < ItemsSource.GroupCount; n++)
				{
					if (ItemsSource.ItemCountInGroup(n) > 0)
					{
						group = n;
						break;
					}
				}
			}

			var indexPath = NSIndexPath.Create(group, 0);

			return GetCell(CollectionView, indexPath);
		}

		protected virtual void RegisterViewTypes()
		{
			CollectionView.RegisterClassForCell(typeof(HorizontalDefaultCell), HorizontalDefaultCell.ReuseId);
			CollectionView.RegisterClassForCell(typeof(VerticalDefaultCell), VerticalDefaultCell.ReuseId);
			CollectionView.RegisterClassForCell(typeof(HorizontalTemplatedCell),
				HorizontalTemplatedCell.ReuseId);
			CollectionView.RegisterClassForCell(typeof(VerticalTemplatedCell), VerticalTemplatedCell.ReuseId);
		}

		protected bool IsHorizontal => (ItemsView?.ItemsLayout as ItemsLayout)?.Orientation == ItemsLayoutOrientation.Horizontal;

		internal void UpdateEmptyView()
		{
			UpdateView(ItemsView?.EmptyView, ItemsView?.EmptyViewTemplate, ref _emptyUIView, ref _emptyViewFormsElement);

			// If the empty view is being displayed, we might need to update it
			UpdateEmptyViewVisibility(ItemsSource?.ItemCount == 0);
		}

		protected void UpdateSubview(object view, DataTemplate viewTemplate, ref UIView uiView, ref VisualElement formsElement)
		{
			if (uiView != null)
				CollectionView.Subviews.Remove(uiView);

			if (formsElement != null)
			{
				ItemsView.RemoveLogicalChild(formsElement);
				formsElement.MeasureInvalidated -= OnFormsElementMeasureInvalidated;
			}

			UpdateView(view, viewTemplate, ref uiView, ref formsElement);

			if (uiView != null)
				CollectionView.AddSubview(uiView);

			if (formsElement != null)
				ItemsView.AddLogicalChild(formsElement);

			if (formsElement != null)
			{
				RemeasureLayout(formsElement);
				formsElement.MeasureInvalidated += OnFormsElementMeasureInvalidated;
			}
			else if (uiView != null)
			{
				uiView.SizeToFit();
			}
		}

		void RemeasureLayout(VisualElement formsElement)
		{
			if (IsHorizontal)
			{
				var request = formsElement.Measure(double.PositiveInfinity, CollectionView.Frame.Height, MeasureFlags.IncludeMargins);
				Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(formsElement, new Rectangle(-request.Request.Width, 0, request.Request.Width, CollectionView.Frame.Height));
			}
			else
			{
				var request = formsElement.Measure(CollectionView.Frame.Width, double.PositiveInfinity, MeasureFlags.IncludeMargins);
				Xamarin.Forms.Layout.LayoutChildIntoBoundingRegion(formsElement, new Rectangle(0, -request.Request.Height, CollectionView.Frame.Width, request.Request.Height));
			}
		}

		protected void OnFormsElementMeasureInvalidated(object sender, EventArgs e)
		{
			if (sender is VisualElement formsElement)
			{
				HandleFormsElementMeasureInvalidated(formsElement);
			}
		}

		protected virtual void HandleFormsElementMeasureInvalidated(VisualElement formsElement)
		{
			RemeasureLayout(formsElement);
		}

		internal void UpdateView(object view, DataTemplate viewTemplate, ref UIView uiView, ref VisualElement formsElement)
		{
			// Is view set on the ItemsView?
			if (view == null)
			{
				// Clear the cached Forms and native views
				uiView = null;
				formsElement = null;
			}
			else
			{
				// Create the native renderer for the view, and keep the actual Forms element (if any)
				// around for updating the layout later
				var (NativeView, FormsElement) = RealizeView(view, viewTemplate);
				uiView = NativeView;
				formsElement = FormsElement;
			}
		}

		void UpdateEmptyViewVisibility(bool isEmpty)
		{
			if (isEmpty && _emptyUIView != null)
			{
				if (!_currentBackgroundIsEmptyView)
				{
					// Cache any existing background view so we can restore it later
					_backgroundUIView = CollectionView.BackgroundView;
				}

				// Replace any current background with the EmptyView. This will also set the native empty view's frame
				// to match the UICollectionView's frame
				CollectionView.BackgroundView = _emptyUIView;
				_currentBackgroundIsEmptyView = true;

				if (_emptyViewFormsElement != null)
				{
					if (ItemsView.EmptyViewTemplate == null)
					{
						ItemsView.AddLogicalChild(_emptyViewFormsElement);
					}

					// Now that the native empty view's frame is sized to the UICollectionView, we need to handle
					// the Forms layout for its content
					_emptyViewFormsElement.Layout(_emptyUIView.Frame.ToRectangle());
				}
			}
			else
			{
				// Is the empty view currently in the background? Swap back to the default.
				if (_currentBackgroundIsEmptyView)
				{
					CollectionView.BackgroundView = _backgroundUIView;
					ItemsView.RemoveLogicalChild(_emptyViewFormsElement);
				}

				_currentBackgroundIsEmptyView = false;
			}
		}

		internal (UIView NativeView, VisualElement FormsElement) RealizeView(object view, DataTemplate viewTemplate)
		{
			if (viewTemplate != null)
			{
				// Run this through the extension method in case it's really a DataTemplateSelector
				viewTemplate = viewTemplate.SelectDataTemplate(view, ItemsView);

				// We have a template; turn it into a Forms view 
				var templateElement = viewTemplate.CreateContent() as View;
				var renderer = CreateRenderer(templateElement);

				// and set the EmptyView as its BindingContext
				BindableObject.SetInheritedBindingContext(renderer.Element, view);

				return (renderer.NativeView, renderer.Element);
			}

			if (view is View formsView)
			{
				// No template, and the EmptyView is a Forms view; use that
				var renderer = CreateRenderer(formsView);

				return (renderer.NativeView, renderer.Element);
			}

			return (new UILabel { Text = $"{view}" }, null);
		}
	}
}
