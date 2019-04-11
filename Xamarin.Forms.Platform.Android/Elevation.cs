using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Xamarin.Forms.Platform.Android
{
	internal static class ElevationHelper
	{
		internal static void SetElevation(global::Android.Views.View view, VisualElement element, float? defautValue = null)
		{
			if (view == null || element == null || !Forms.IsLollipopOrNewer)
			{
				return;
			}

			var iec = element as IElementConfiguration<VisualElement>;
			var elevation = iec?.On<PlatformConfiguration.Android>().GetElevation() ?? defautValue;

			if (!elevation.HasValue)
			{
				return;
			}

			view.Elevation = elevation.Value;
		}
	}
}