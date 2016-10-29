using System.ComponentModel;
using Android.Graphics;
using SukimaNote;
using SukimaNote.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BicoloredBoxView), typeof(BicoloredBoxViewRenderer))]
namespace SukimaNote.Droid
{
	internal class BicoloredBoxViewRenderer : BoxRenderer
	{
		public override void Draw(Canvas canvas)
		{
			//base.Draw(canvas);

			var bicoloredBoxView = (BicoloredBoxView)Element;

			using (var paint = new Paint())
			{

				var shadowSize = bicoloredBoxView.ShadowSize;
				var blur = shadowSize;

				paint.AntiAlias = true;

				// 影の描画
				paint.Color = (Xamarin.Forms.Color.FromRgba(0, 0, 0, 112)).ToAndroid();
				paint.SetMaskFilter(new BlurMaskFilter(blur, BlurMaskFilter.Blur.Normal));
				RectF rectangle;
				if (bicoloredBoxView.RightColor == Xamarin.Forms.Color.Default)
					rectangle = new RectF(shadowSize, shadowSize, (float)Width * bicoloredBoxView.Ratio / 100, Height);
				else
					rectangle = new RectF(shadowSize, shadowSize, Width, Height);
				canvas.DrawRect(rectangle, paint);

				// RightColorの四角の描画
				paint.Color = bicoloredBoxView.RightColor.ToAndroid();
				rectangle = new RectF(0, 0, Width - shadowSize, Height - shadowSize);
				canvas.DrawRect(rectangle, paint);

				// LeftColorの四角の描画
				paint.Color = bicoloredBoxView.LeftColor.ToAndroid();
				rectangle = new RectF(0, 0, (float)Width * bicoloredBoxView.Ratio / 100 - shadowSize, Height - shadowSize);
				canvas.DrawRect(rectangle, paint);
			}
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);
			if (e.PropertyName == "Ratio")
			{
				Invalidate();
			}
		}
	}
}