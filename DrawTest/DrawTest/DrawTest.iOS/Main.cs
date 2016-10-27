using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Foundation;
using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using DrawTest;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(ExtendedBoxView), typeof(DrawTest.iOS.ExtendedViewRenderer))]
namespace DrawTest.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }

    public class ExtendedViewRenderer : ViewRenderer<ExtendedBoxView, UIView>
    {
        public override void Draw(CGRect rect)
        {
            //base.Draw(rect);
            using (var context = UIGraphics.GetCurrentContext())
            {
                // モデルの参照
                var boxView = Element as ExtendedBoxView;

                // 色、線幅の指定
                context.SetStrokeColor(boxView.FillColor.ToCGColor());
                context.SetLineWidth(boxView.BorderWidth);

                // 円弧の描画
                context.AddArc
                    (
                        rect.Width / 2.0f,
                        rect.Height / 2, 0f,
                        0f,
                        Convert.ToSingle(-boxView.Angle * Math.PI / 180f),
                        false
                    );
                context.StrokePath();
            }

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == "BorderWidth" || e.PropertyName == "Angle")
            {
                SetNeedsDisplay();
            }
        }

    }
}
