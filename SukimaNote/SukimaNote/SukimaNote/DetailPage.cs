using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace SukimaNote
{
	// スケジュールの設定をする画面？詳細未定
	public class SchedulePage : ContentPage
	{
		public SchedulePage()
		{
			Title = "スケジュール設定";
			Content = new Label
			{
				Text = "スケジュールの設定をするよ",
				FontSize = 60,
				BackgroundColor = Color.White,
				TextColor = Color.Black,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};
		}
	}

	// 本体設定をする。詳細未定
	public class SettingPage : ContentPage
	{
		public SettingPage()
		{
			Title = "本体設定";
			Content = new Label
			{
				Text = "本体の詳細設定をするよ",
				FontSize = 60,
				BackgroundColor = Color.White,
				TextColor = Color.Black,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};
		}
	}
}