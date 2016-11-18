using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using SukimaNote.Droid;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;

[assembly : ExportRenderer (typeof(Label), typeof(NFLabelRenderer))]
namespace SukimaNote.Droid
{
	public class NFLabelRenderer : LabelRenderer
	{
		private static Typeface syunkasyuutouFont = Typeface.CreateFromAsset(Forms.Context.Assets, "syunkasyuutouBB.ttf");

		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);
			if (e.NewElement.FontFamily == "syunkasyuutouBB.ttf")
			{
				Control.Typeface = syunkasyuutouFont;
			}
		}
	}
}
