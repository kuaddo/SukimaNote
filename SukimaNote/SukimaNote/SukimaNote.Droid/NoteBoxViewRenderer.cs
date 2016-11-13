using Android.Graphics;
using SukimaNote;
using SukimaNote.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(NoteBoxView), typeof(NoteBoxViewRenderer))]
namespace SukimaNote.Droid
{
	internal class NoteBoxViewRenderer : BoxRenderer
	{
		public override void Draw(Canvas canvas)
		{
			var noteBoxView = (NoteBoxView)Element;

			using (var paint = new Paint())
			{
				double edgeSpace = Width * noteBoxView.EdgeSpaceRatio / 2;
				int row = noteBoxView.Row;
				double rowHeight = (double)Height / row;

				paint.AntiAlias = true;

				// ノートの背景の描画
				paint.Color = noteBoxView.Color.ToAndroid();
				var rectangle = new RectF(0, 0, Width, Height);
				canvas.DrawRect(rectangle, paint);

				// ノートの線の描画
				paint.Color = noteBoxView.StrokeColor.ToAndroid();
				for (int i = 1; i <= row; i++)
				{
					rectangle = new RectF((float)edgeSpace, (float)(rowHeight * i - noteBoxView.StrokeWidth * 1.2), (float)(Width - edgeSpace), (float)(rowHeight * i - noteBoxView.StrokeWidth * 0.2));
					canvas.DrawRect(rectangle, paint);
				}
			}
		}
	}
}