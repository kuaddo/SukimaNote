using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace SukimaNote
{
	public class TopPage : ContentPage
	{
		public TopPage()
		{
			Title = "トップページ";
			Content = new Label
			{
				Text = "タスクの提案をするよ",
				FontSize = 60,
				BackgroundColor = Color.White,
				TextColor = Color.Black,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};
		}
	}
}