using System;
using CoreGraphics;
using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SukimaNote;
using SukimaNote.iOS;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(RoundProgressBar), typeof(RoundProgressBarRenderer))]
namespace SukimaNote.iOS
{
	internal class RoundProgressBarRenderer : BoxRenderer//<RoundProgressBar, UIView>
	{
		public override void Draw(CGRect rect)
		{
			using (var context = UIGraphics.GetCurrentContext())
			{
				// モデルの参照
				var roundProgressBar = Element as RoundProgressBar;
				var lineWidth = (float)roundProgressBar.Width * (1 - roundProgressBar.StrokeWidth) / 2;

				// 四角の描画
				context.SetFillColor(roundProgressBar.Color.ToCGColor());
				var rectangle = new System.Drawing.RectangleF(0, 0, (float)roundProgressBar.Width, (float)roundProgressBar.Height);
				context.AddPath(CGPath.FromRect(rectangle));
				context.DrawPath(CGPathDrawingMode.Fill);


				// 色、線幅の指定
				context.SetStrokeColor(roundProgressBar.StrokeColor.ToCGColor());
				context.SetLineWidth(lineWidth);

				// 円弧の描画
				context.AddArc
					(
						(float)roundProgressBar.Width / 2.0f,
						(float)roundProgressBar.Height / 2.0f,
						Math.Min((float)roundProgressBar.Width / 2.0f - lineWidth / 2, (float)roundProgressBar.Height / 2.0f - lineWidth / 2),
						(float)(-Math.PI / 2),
						(float)((-Math.PI / 2) + (2 * Math.PI * roundProgressBar.Angle / 100)),
						false
					);
				context.StrokePath();
			}

		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == "Angle")
			{
				SetNeedsDisplay();
			}
		}

	}
}