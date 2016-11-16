
using System.Drawing;
using CoreGraphics;
using SukimaNote.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SukimaNote;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(PostItView), typeof(PostItViewRenderer))]
namespace SukimaNote.iOS
{
	internal class PostItViewRenderer : BoxRenderer
	{
		public override void Draw(CGRect rect)
		{
			using (var context = UIGraphics.GetCurrentContext())
			{
				var postItView = (PostItView)Element;
				var shadowSize = postItView.ShadowSize;
				var postItWidth = postItView.Width - shadowSize;
				var postItHeight = postItView.Height - shadowSize;
				var blur = shadowSize;

				// 付箋の左側の描画
				context.SetFillColor(postItView.LightColor.ToCGColor());
				context.SetShadow(new SizeF(shadowSize, shadowSize), blur);
				var rectangle = new RectangleF(0, 0, (float)(postItWidth * 0.05), (float)postItHeight);
				context.AddPath(CGPath.FromRect(rectangle));
				context.DrawPath(CGPathDrawingMode.Fill);

				// 付箋の右側の描画
				context.SetFillColor(Color.White.ToCGColor());
				context.SetLineWidth(0);
				context.SetShadow(new SizeF(shadowSize, shadowSize), blur);
				context.MoveTo((float)(postItWidth * 0.05), 0);
				context.AddLineToPoint((float)(postItWidth * 0.05), (float)postItHeight);
				context.AddLineToPoint((float)(postItWidth * 0.85), (float)postItHeight);
				context.AddLineToPoint((float)postItWidth, (float)(postItHeight * 0.85));
				context.AddLineToPoint((float)postItWidth, 0);
				context.ClosePath();
				context.DrawPath(CGPathDrawingMode.Fill);

				// 裏面を表現する線の描画
				context.SetLineWidth(0.1f);
				context.SetStrokeColor(Color.Black.ToCGColor());
				context.SetFillColor(Color.Transparent.ToCGColor());    // 透明色?
				context.MoveTo((float)(postItWidth * 0.85), (float)postItHeight);
				context.AddLineToPoint((float)(postItWidth * 0.97), (float)(postItHeight * 0.80));
				context.AddLineToPoint((float)postItWidth, (float)(postItHeight * 0.85));
				context.StrokePath();   // こっちは閉じない
			}
		}
	}
}