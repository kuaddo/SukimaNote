using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace SukimaNote
{
	// タスクの設定項目のプロパティのクラス
	class Data
	{
		string Title { get; set; }  // タイトル
		int RestTime { get; set; }  // 予想残り時間
		int UnitTime { get; set; }  // 最低単位作業時間
		DateTime Term { get; set; }	// 期限。int型ではなく時間を表す型を探す
		string Remark { get; set; } // 備考
	}

	// タスクの詳細画面を描画するページ
	public class ShowTask : ContentPage
	{

	}

	// タスクの追加の設定ページ
	public class AddTaskPage : ContentPage
	{
		public AddTaskPage()
		{
			// NavigationBarを非表示に
			NavigationPage.SetHasNavigationBar(this, false);

			// 「戻る」ボタン
			var backButton = new Button
			{
				Text = "Back",
				FontSize = 30,
				BackgroundColor = Color.Aqua,
				HorizontalOptions = LayoutOptions.Start,
			};
			// タスクを追加しないで戻る
			backButton.Clicked += async (sender, e) =>
			{
				await Navigation.PopAsync();
			};

			// 「完了」ボタン
			var finishButton = new Button
			{
				Text = "Finish",
				FontSize = 30,
				BackgroundColor = Color.Aqua,
				HorizontalOptions = LayoutOptions.End,
			};

			// backとfinishの間を埋めるラベル
			var fillLabel = new Label
			{
				BackgroundColor = Color.Navy,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
			};

			// 最上部のバー
			var topBar = new StackLayout
			{
				BackgroundColor = Color.Navy,
				Orientation = StackOrientation.Horizontal,
				Children = { backButton, fillLabel, finishButton },
				VerticalOptions = LayoutOptions.Start,
				Spacing = 5,
			};


			// タイトル入力
			var titleEntry = new Entry
			{
				Keyboard = Keyboard.Text,
				Placeholder = "タイトル",
				BackgroundColor = Color.Green
			};

			// 予想残り時間入力
			var restTime = new StackLayout
			{
				Children =
				{
					new Label { Text = "予想残り時間" },
					new TimePicker { Format = "T", BackgroundColor = Color.Green },
				}
			};

			// 最低単位作業時間入力
			var unitTime = new StackLayout
			{
				Children =
				{
					new Label { Text = "最低単位作業時間" },
					new TimePicker { Format = "T", BackgroundColor = Color.Green },
				}
			};

			// 期限入力
			var term = new StackLayout
			{
				Children =
				{
					new Label { Text = "期限" },
					new DatePicker { Format = "D", BackgroundColor = Color.Green }
				}
			};

			// 備考入力
			var remark = new StackLayout
			{
				Children =
				{
					new Label { Text = "備考" },
					// 高さを直接数値指定ではなくFillのように指定する方法がわからない
					new Editor { BackgroundColor = Color.Green, HeightRequest = 250}
				}
			};
				
			var input = new StackLayout
			{
				Spacing = 3,
				Children = { titleEntry, restTime, unitTime, term, remark },
			};

			var layout = new StackLayout
			{
				// iOSのみ上部に空白を取る
				Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0),
				Children = { topBar, input}
			};

			Content = layout;
		}
	}

	// test
	public class testPage : ContentPage
	{
		public testPage()
		{
			Title = "TestPage";

			var button = new Button
			{
				Text = "test",
				FontSize = 30,
			};
			button.Clicked += async (sender, e) =>
			{
				await Navigation.PushAsync(new AddTaskPage());
			};

			Content = button;
		}
	}
}