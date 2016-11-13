using Android.Graphics;
using SukimaNote;
using SukimaNote.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PostItView), typeof(PostItViewRenderer))]
namespace SukimaNote.Droid
{
	internal class PostItViewRenderer : BoxRenderer
	{
		public override void Draw(Canvas canvas)
		{
			var postItView = (PostItView)Element;

			using (var paint = new Paint())
			{
				paint.AntiAlias = true;
				var shadowSize = postItView.ShadowSize;
				var postItWidth = Width - shadowSize;
				var postItHeight = Height - shadowSize;

				// 影の描画
				paint.Color = (Xamarin.Forms.Color.FromRgba(0, 0, 0, 112)).ToAndroid();
				paint.SetMaskFilter(new BlurMaskFilter(shadowSize, BlurMaskFilter.Blur.Normal));
				var path = new Path();
				path.MoveTo(shadowSize, shadowSize);
				path.LineTo(shadowSize, Height);
				path.LineTo((float)(Width * 0.85) + shadowSize, Height);
				path.LineTo(Width, (float)(Height * 0.85) + shadowSize);
				path.LineTo(Width, shadowSize);
				path.Close();
				canvas.DrawPath(path, paint);

				// 付箋の左側の描画
				paint.Color = postItView.Color.ToAndroid();
				var rectangle = new RectF(0, 0, (float)(postItWidth * 0.03), (float)postItHeight);
				canvas.DrawRect(rectangle, paint);

				// 付箋の右側の描画
				path = new Path();
				paint.Color = postItView.LightColor.ToAndroid();
				path.MoveTo((float)(postItWidth * 0.03), 0);
				path.LineTo((float)(postItWidth * 0.03), (float)postItHeight);
				path.LineTo((float)(postItWidth * 0.85), (float)postItHeight);
				path.LineTo((float)postItWidth, (float)(postItHeight * 0.85));
				path.LineTo((float)postItWidth, 0);
				path.Close();
				canvas.DrawPath(path, paint);

				// 裏面を表現する線の描画
				paint.SetStyle(Paint.Style.Stroke);     // スタイルを線描画に設定
				paint.Color = Android.Graphics.Color.Black;
				paint.StrokeWidth = 1;
				canvas.DrawLine((float)(postItWidth * 0.85), (float)postItHeight, (float)(postItWidth * 0.97), (float)(postItHeight * 0.80), paint);
				canvas.DrawLine((float)(postItWidth * 0.97), (float)(postItHeight * 0.80), (float)postItWidth, (float)(postItHeight * 0.85), paint);

			}
		}
	}
}