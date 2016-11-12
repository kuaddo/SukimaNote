
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using SukimaNote;

[assembly: Dependency(typeof(Notification_iOS))]

namespace SukimaNote
{
	public class Notification_iOS : makeNotification
	{
		public void make(string title, string text, int id, int interval)
		{
			UILocalNotification notification = new UILocalNotification();
			notification.FireDate = Foundation.NSDate.FromTimeIntervalSinceNow((double)interval);
			notification.AlertTitle = title;
			notification.AlertBody = text;

			UIApplication.SharedApplication.ScheduleLocalNotification(notification);
		}
	}
	
}