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
using DrawTest;
using DrawTest.Droid;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(ExtendedBoxView), typeof(ExtendedBoxViewRenderer))]
namespace DrawTest.Droid
{

    [Activity(Label = "DrawTest", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]

    public class MainActivity : FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }


    class ExtendedBoxViewRenderer : BoxRenderer
    {
        public override void Draw(Canvas canvas)
        {
            // モデルオブジェクトの取得
            var extendedBoxView = Element as ExtendedBoxView;

            using (var paint = new Paint())
            {
                // 基準の範囲
                RectF rectangleOuter = new RectF(0, 0, Width, Height);
                paint.Color = extendedBoxView.FillColor.ToAndroid();
                paint.SetStyle(Paint.Style.FillAndStroke);
                canvas.DrawRect(rectangleOuter, paint);

                // アンチエイリアス有効
                paint.AntiAlias = true;
                // 外側の円描画
                paint.Color = extendedBoxView.StrokeColor.ToAndroid();
                canvas.DrawArc(rectangleOuter, -90, extendedBoxView.Angle, true, paint);

                // 内側の円の描画
                paint.Color = extendedBoxView.FillColor.ToAndroid();
                float radiusInner = extendedBoxView.BorderWidth * Width / 2.0f;
                canvas.DrawCircle(Width / 2.0f, Height / 2.0f, radiusInner, paint);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            // Radius, BorderWidthの変更時再描画
            if (e.PropertyName == "Radius" || e.PropertyName == "BorderWidth")
            {
                Invalidate();
            }
        }
    }
}



