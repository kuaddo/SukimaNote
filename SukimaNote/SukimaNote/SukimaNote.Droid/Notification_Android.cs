
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.App;
using Android.OS;
using Android.Support.V4.App;
using SukimaNote;
using Java.Lang;
using Android.Content;
using Android.Content.PM;
using SukimaNote.Droid;

[assembly: Dependency(typeof(Notification_Android))]

namespace SukimaNote.Droid
{
	public class Notification_Android : IMakeNotification
	{
		// SukimaNoteのNotificationと区別ができていないようなので、namespaceを含めた完全修飾子で表記
		public void make(string title, string text, int id, int interval)
		{
			var context = Forms.Context;
			var pendingIntent = Android.App.TaskStackBuilder.Create(context)
				.AddNextIntent(new Intent(context, typeof(MainActivity)))
				.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);

			// Instantiate the builder and set notification elements:
			var builder = new Android.App.Notification.Builder(context)
				.SetContentTitle(title)
				.SetContentText(text)
				.SetContentIntent(pendingIntent)
				.SetSmallIcon(Resource.Drawable.ic_notification);

			// Build the notification:
			Android.App.Notification notification = builder.Build();

			// Get the notification manager:
			NotificationManager notificationManager =
				context.GetSystemService(Context.NotificationService) as NotificationManager;

			// Publish the notification:
			notificationManager.Notify(id, notification);

		}
	}
}