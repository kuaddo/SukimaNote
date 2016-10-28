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

	class RoundProgressBarRenderer : BoxRenderer
	{
		public override void Draw(Canvas canvas)
		{
			// モデルオブジェクトの取得
			var roundProgressBar = Element as RoundProgressBar;

			using (var paint = new Paint())
			{
				// 基準の範囲
				RectF rectangleOuter = new RectF(0, 0, Width, Height);
				paint.Color = roundProgressBar.Color.ToAndroid();
				paint.SetStyle(Paint.Style.FillAndStroke);
				canvas.DrawRect(rectangleOuter, paint);

				// アンチエイリアス有効
				paint.AntiAlias = true;
				// 外側の円描画
				paint.Color = roundProgressBar.StrokeColor.ToAndroid();
				canvas.DrawArc(rectangleOuter, -90, 360 * roundProgressBar.Angle / 100, true, paint);

				// 内側の円の描画
				paint.Color = roundProgressBar.Color.ToAndroid();
				float radiusInner = roundProgressBar.StrokeWidth * Width / 2.0f;
				canvas.DrawCircle(Width / 2.0f, Height / 2.0f, radiusInner, paint);
			}
		}


		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			// Radius, BorderWidthの変更時再描画
			if (e.PropertyName == "Angle")
			{
				Invalidate();
			}
		}
	}
}

