using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace SukimaNote
{
	// 本体設定をする。詳細未定
	public class SettingPage : ContentPage
	{
		public SettingPage(RootPage rootPage)
		{
			Title = "設定";
			Content = new Label
			{
				Text = "本体の詳細設定とスケジュールの設定をするよ",
				FontSize = 60,
				BackgroundColor = Color.White,
				TextColor = Color.Black,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};
		}
	}
}