
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.App;
using Android.OS;
using Android.Support.V4.App;
using SukimaNote;

[assembly: Dependency(typeof(Notification_Android))]

namespace SukimaNote
{
	public class Notification_Android : makeNotification
	{
		public void make(string title, string text, int id, int interval)
		{

			// アイコン
			var image = new Image
			{
				Source = "icon.png"
			};
			NotificationCompat.Builder builder = new NotificationCompat.Builder(this)
				.SetSmallIcon()
				.SetContentTitle(title)
				.SetContentText(text);
		}
	}


}