using System.Drawing;
using CoreGraphics;
using SukimaNote.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using SukimaNote;

[assembly: ExportRenderer(typeof(NoteBoxView), typeof(NoteBoxViewRenderer))]
namespace SukimaNote.iOS
{
	internal class NoteBoxViewRenderer : BoxRenderer
	{
		public override void Draw(CGRect rect)
		{
			using (var context = UIGraphics.GetCurrentContext())
			{
				var noteBoxView = (NoteBoxView)Element;
				double edgeSpace = noteBoxView.Width * noteBoxView.EdgeSpaceRatio / 2;
				int row = noteBoxView.Row;
				double rowHeight = (double)noteBoxView.Height / row;

				// ノートの背景の描画
				context.SetFillColor(noteBoxView.Color.ToCGColor());
				var rectangle = new RectangleF(0, 0, (float)noteBoxView.Width, (float)noteBoxView.Height);
				context.AddPath(CGPath.FromRect(rectangle));
				context.DrawPath(CGPathDrawingMode.Fill);

				// ノートの線の描画
				context.SetFillColor(noteBoxView.StrokeColor.ToCGColor());
				for (int i = 1; i <= row; i++)
				{
					rectangle = new RectangleF((float)edgeSpace, (float)(rowHeight * i - noteBoxView.StrokeWidth * 1.2), (float)(noteBoxView.Width - edgeSpace * 2), (float)(noteBoxView.StrokeWidth));
					context.AddPath(CGPath.FromRect(rectangle));
				}
				context.DrawPath(CGPathDrawingMode.Fill);
			}
		}
	}
}