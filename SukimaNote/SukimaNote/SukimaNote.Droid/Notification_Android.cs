
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

		/*
		public void make(string title, string text, int id, int interval)
		{

			// アイコン
			var image = new Image
			{
				Source = "icon.png"
			};
			Notification.Builder builder = new Notification.Builder(this)
				.SetSmallIcon(Resources.drawable.AppIcon)
				.SetContentTitle(title)
				.SetContentText(text);

			Notification notification = builder.Build();
			NotificationManager notificationManager;
			notificationManager.Notify(id, notification);
		}
		*/
	}


}