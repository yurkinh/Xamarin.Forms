using System;
using System.Collections.Generic;
using System.Reflection;

using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Xaml
{
	// IMPORTANT!!
	//  READ BEFORE MODIFYING!!
	// This class represents the API that the Forms previewer will use
	//  via reflection. Adding members or changing implementations is fine, but
	//  DO NOT break this API unless you know what you're doing and/or are
	//  coordinating with the designer team.
	// Thanks!
	static class DesignerInterface
	{
		const int LatestSupportedVersion = 1;
		static int Version => LatestSupportedVersion;

		// These types are used via reflection for type checks..
		static Type IMarkupExtensionType => typeof (IMarkupExtension);
		static Type ViewType => typeof (Xamarin.Forms.View);
		static Type BoxViewType => typeof (Xamarin.Forms.BoxView);
		static Type VisualElementType => typeof (Xamarin.Forms.VisualElement);
		static Type ApplicationType => typeof (Xamarin.Forms.Application);

		static Func<ResourceLoader.ResourceLoadingQuery, ResourceLoader.ResourceLoadingResponse> ResourceProvider2 {
			get => ResourceLoader.ResourceProvider2;
			set => ResourceLoader.ResourceProvider2 = value;
		}

		static Action<Exception> ExceptionHandler {
			get => ResourceLoader.ExceptionHandler;
			set => ResourceLoader.ExceptionHandler = value;
		}

		static Func<IList<XamlLoader.FallbackTypeInfo>, Type, Type> FallbackTypeResolver {
			get => XamlLoader.FallbackTypeResolver;
			set => XamlLoader.FallbackTypeResolver = value;
		}

		static Action<XamlLoader.CallbackTypeInfo, object> ValueCreatedCallback  {
			get => XamlLoader.ValueCreatedCallback;
			set => XamlLoader.ValueCreatedCallback = value;
		}

		static void SetIdiom(TargetIdiom idiom) => Device.SetIdiom(idiom);

		static void SetLayout(Page page, Rectangle layout)
			=> page.Layout(layout);

		static TXaml LoadFromXaml<TXaml>(TXaml view, string xaml)
			=> Extensions.LoadFromXaml(view, xaml);

		static object CreateFromXaml(string xaml, bool doNotThrow, bool useDesignProperties)
			=> XamlLoader.Create(xaml, doNotThrow, useDesignProperties);

		static string GetPathForType(Type type)
			=> XamlResourceIdAttribute.GetPathForType(type);

		static void RegisterAll(Type[] attrTypes)
			=> Xamarin.Forms.Internals.Registrar.RegisterAll (attrTypes);

		static Page GetPage (object obj)
		{
			switch (obj) {

			case null: return null;
			case Page p: return p;
			case View v: return new ContentPage { Content = v };
			case ViewCell cell:
				return new ContentPage { Content = cell.View ?? throw new EmptyViewCellException () };
			default:
				throw new UnsupportedTypeException (obj.GetType ());
			}
		}

		public class EmptyViewCellException : Exception
		{
		}

		public class UnsupportedTypeException : Exception
		{
			public UnsupportedTypeException (Type type)
				: base (type.FullName)
			{
			}
		}
	}

	// Platform assembly members that are reflected:
	//  Note that visibility is irrelevant to our concerns; they may be made non-public
	//  without breaking this contract.

	// All platforms:
	//
	//  Xamarin.Forms.ExportRendererAttribute
	//  Xamarin.Forms.ExportCellAttribute
	//  Xamarin.Forms.ExportImageSourceHandlerAttribute
	//
	//  static void Forms.SetFlags(string[] flags)
	//  static IVisualElementRenderer Platform.CreateRenderer(VisualElement element)
	//  static void Platform.SetRenderer(VisualElement bindable, IVisualElementRenderer value)

	// iOS:
	//
	//  static void Forms.Init()
	//  UIView IVisualElementRenderer.NativeView { get; }

	// Android:
	//
	//  static bool Forms.IsInitialized { get; }
	//  static void ResourceManager.Init(Assembly masterAssembly)
	//  static bool FileImageSourceHandler.DecodeSynchronously { set; }
	//  static void Platform.SetPageContext(BindableObject bindable, Context context)
	//  void IVisualElementRenderer.UpdateLayout()
	//  ViewGroup IVisualElementRenderer.ViewGroup { get; }*
	//  static void Forms.Init(Context activity, Bundle bundle)*
}
