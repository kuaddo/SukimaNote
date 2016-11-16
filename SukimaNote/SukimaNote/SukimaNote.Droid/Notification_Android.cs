
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
using Java.Util;
using System;
using Android.Widget;

[assembly: Dependency(typeof(Notification_Android))]

namespace SukimaNote.Droid
{
	public class Notification_Android : IMakeNotification
	{
		// SukimaNoteのNotificationと区別ができていないようなので、namespaceを含めた完全修飾子で表記
		public void make(string title, string text, int id, int interval)
		{
			var context = Forms.Context;

			var intent = new Intent("intent.action.TEST")
				.PutExtra("TITLE", title)
				.PutExtra("TEXT", text)
				.PutExtra("ID", id);
        	var sender = PendingIntent.GetBroadcast(context, id, intent, PendingIntentFlags.UpdateCurrent);
			
			// 通知時刻の設定
			Calendar calender = Calendar.GetInstance(Locale.Japan);
			calender.TimeInMillis = JavaSystem.CurrentTimeMillis();
			calender.Add(CalendarField.Second, interval);

			AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
			alarmManager.Set(AlarmType.RtcWakeup, calender.TimeInMillis, sender);
		}
	}

	[BroadcastReceiver]
	[IntentFilter(new[] { "intent.action.TEST" })]
	public class NotificationReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			// 通知をタップされた時に発行されるインテント
			var pendingIntent = Android.App.TaskStackBuilder.Create(context)
				.AddNextIntent(new Intent(context, typeof(MainActivity)))
				.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);

			// 通知の要素の指定
			var builder = new Android.App.Notification.Builder(context)
				.SetContentTitle(intent.GetStringExtra("TITLE"))
				.SetContentText(intent.GetStringExtra("TEXT"))
				.SetTicker("スキマNote")
				.SetContentIntent(pendingIntent)
				.SetSmallIcon(Resource.Drawable.ic_notification)
				.SetAutoCancel(true);

			// 通知の作成
			Android.App.Notification notification = builder.Build();

			// notification managerの取得
			NotificationManager notificationManager =
				context.GetSystemService(Context.NotificationService) as NotificationManager;

			// 通知の実行
			notificationManager.Notify(intent.GetIntExtra("ID", 0), notification);
		}
	}
}