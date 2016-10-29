using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using SukimaNote;
using SukimaNote.Droid;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(RoundProgressBar), typeof(RoundProgressBarRenderer))]
namespace SukimaNote.Droid
{
	[Activity(Label = "SukimaNote.Droid", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsApplicationActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			Forms.Init(this, bundle);
			LoadApplication(new Main());
		}
	}
}

