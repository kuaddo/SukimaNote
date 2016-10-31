using System.Drawing;
using CoreGraphics;
using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SukimaNote;
using SukimaNote.iOS;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(BicoloredBoxView), typeof(BicoloredBoxViewRenderer))]
namespace SukimaNote.iOS
{
	internal class BicoloredBoxViewRenderer : BoxRenderer
	{
		public override void Draw(CGRect rect)
		{
			using (var context = UIGraphics.GetCurrentContext())
			{
				var bicoloredBoxView = (BicoloredBoxView)Element;
				var shadowSize = bicoloredBoxView.ShadowSize;
				var blur = shadowSize;
				RectangleF rectangle;

				// RightColorの四角の描画
				if (bicoloredBoxView.RightColor != Color.Default)
					context.SetShadow(new SizeF(shadowSize, shadowSize), blur);
				context.SetFillColor(bicoloredBoxView.RightColor.ToCGColor());
				rectangle = new RectangleF(0, 0, (float)bicoloredBoxView.Width - shadowSize, (float)bicoloredBoxView.Height - shadowSize);
				context.AddPath(CGPath.FromRect(rectangle));
				context.DrawPath(CGPathDrawingMode.Fill);

				// LeftColorの四角の描画
				if (bicoloredBoxView.RightColor == Color.Default)
					context.SetShadow(new SizeF(shadowSize, shadowSize), blur);
				else
					context.SetShadow(new SizeF(0, 0), 0);
				context.SetFillColor(bicoloredBoxView.LeftColor.ToCGColor());
				rectangle = new RectangleF(0, 0, (float)bicoloredBoxView.Width * bicoloredBoxView.Ratio / 100 - shadowSize, (float)bicoloredBoxView.Height - shadowSize);
				context.AddPath(CGPath.FromRect(rectangle));
				context.DrawPath(CGPathDrawingMode.Fill);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == "Ratio")
			{
				SetNeedsDisplay();
			}
		}
	}
}