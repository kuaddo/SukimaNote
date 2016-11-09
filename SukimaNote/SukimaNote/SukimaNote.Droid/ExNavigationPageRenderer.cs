using Android.App;
using SukimaNote;
using SukimaNote.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ExNavigationPage), typeof(ExNavigationPageRenderer))]
namespace SukimaNote.Droid
{
	class ExNavigationPageRenderer : NavigationRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e)
		{
			base.OnElementChanged(e);

			var actionBar = ((Activity)Context).ActionBar;
			actionBar.SetIcon(Android.Resource.Color.Transparent);
		}
	}
}