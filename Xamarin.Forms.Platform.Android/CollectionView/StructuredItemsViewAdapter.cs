﻿using System;
using System.ComponentModel;
using Android.Content;
using Android.Support.V7.Widget;
using ViewGroup = Android.Views.ViewGroup;

namespace Xamarin.Forms.Platform.Android
{
	public class StructuredItemsViewAdapter<TItemsView, TItemsViewSource> : ItemsViewAdapter<TItemsView, TItemsViewSource>
		where TItemsView : StructuredItemsView
		where TItemsViewSource : IItemsViewSource
	{
		internal StructuredItemsViewAdapter(TItemsView itemsView, 
			Func<View, Context, ItemContentView> createItemContentView = null) : base(itemsView, createItemContentView)
		{
			UpdateHasHeader();
			UpdateHasFooter();
		}

		protected override void ItemsViewPropertyChanged(object sender, PropertyChangedEventArgs property)
		{
			base.ItemsViewPropertyChanged(sender, property);

			if (property.Is(Xamarin.Forms.StructuredItemsView.HeaderProperty))
			{
				UpdateHasHeader();
			}
			else if (property.Is(Xamarin.Forms.StructuredItemsView.FooterProperty))
			{
				UpdateHasFooter();
			}
		}

		public override int GetItemViewType(int position)
		{
			if (IsHeader(position))
			{
				return ItemViewType.Header;
			}

			if (IsFooter(position))
			{
				return ItemViewType.Footer;
			}

			return base.GetItemViewType(position);
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var context = parent.Context;

			if (viewType == ItemViewType.Header)
			{
				return CreateHeaderFooterViewHolder(ItemsView.Header, ItemsView.HeaderTemplate, context);
			}

			if (viewType == ItemViewType.Footer)
			{
				return CreateHeaderFooterViewHolder(ItemsView.Footer, ItemsView.FooterTemplate, context);
			}

			return base.OnCreateViewHolder(parent, viewType);
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			if (IsHeader(position))
			{
				if (holder is TemplatedItemViewHolder templatedItemViewHolder)
				{
					BindTemplatedItemViewHolder(templatedItemViewHolder, ItemsView.Header);
				}

				return;
			}

			if (IsFooter(position))
			{
				if (holder is TemplatedItemViewHolder templatedItemViewHolder)
				{
					BindTemplatedItemViewHolder(templatedItemViewHolder, ItemsView.Footer);
				}

				return;
			}

			base.OnBindViewHolder(holder, position);
		}

		void UpdateHasHeader()
		{
			ItemsSource.HasHeader = ItemsView.Header != null;
		}

		void UpdateHasFooter()
		{
			ItemsSource.HasFooter = ItemsView.Footer != null;
		}

		bool IsHeader(int position)
		{
			return ItemsSource.IsHeader(position);
		}

		bool IsFooter(int position)
		{
			return ItemsSource.IsFooter(position);
		}

		protected RecyclerView.ViewHolder CreateHeaderFooterViewHolder(object content, DataTemplate template, Context context)
		{
			if (template != null)
			{
				var footerContentView = new ItemContentView(context);
				return new TemplatedItemViewHolder(footerContentView, template, isSelectionEnabled: false);
			}

			if (content is View formsView)
			{
				return SimpleViewHolder.FromFormsView(formsView, context);
			}

			// No template, Footer is not a Forms View, so just display Footer.ToString
			return SimpleViewHolder.FromText(content?.ToString(), context, fill: false);
		}
	}
}