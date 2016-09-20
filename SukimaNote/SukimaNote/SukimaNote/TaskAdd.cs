using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using PCLStorage;

namespace SukimaNote
{
	// タスクの設定項目のプロパティのクラス
	public class TaskData
	{
		public string Title { get; set; }		// タイトル
		public int RestTime { get; set; }		// 予想残り時間
		public int UnitTime { get; set; }		// 最低単位作業時間
		public DateTime Term { get; set; }		// 期限。int型ではなく時間を表す型を探す
		public string Remark { get; set; }		// 備考
	}

	// タスクの追加の設定ページ
	public class TaskAddPage : ContentPage
	{
		IFolder rootFolder = FileSystem.Current.LocalStorage;

		public TaskAddPage()
		{
			Title = "タスク追加";
			// NavigationBarを非表示に
			NavigationPage.SetHasNavigationBar(this, false);

			// タスクのデータ入力部分
			// タイトル入力
			var titleEntry = new Entry { Keyboard = Keyboard.Text, BackgroundColor = Color.Green };
			var title = new StackLayout { Children = { new Label { Text = "タイトル"}, titleEntry } };

			// 1~60分までのリストの作成
			var ar = Enumerable.Range(1, 60).Select(n => string.Format("{0}分", n)).ToList();

			// 予想残り時間入力
			var restTimePicker = new Picker { BackgroundColor = Color.Green };
			foreach (var a in ar) { restTimePicker.Items.Add(a); }
			var restTime = new StackLayout { Children = { new Label { Text = "予想残り時間" }, restTimePicker} };

			// 最低単位作業時間入力
			var unitTimePicker = new Picker { BackgroundColor = Color.Green };
			foreach (var a in ar) { unitTimePicker.Items.Add(a); }
			var unitTime = new StackLayout { Children =	{ new Label { Text = "最低単位作業時間" }, unitTimePicker }	};

			// 期限入力
			var termDatePicker = new DatePicker { Format = "D", BackgroundColor = Color.Green };
			var term = new StackLayout { Children =	{ new Label { Text = "期限" }, termDatePicker} };

			// 備考入力
			// 高さを直接数値指定ではなくFillのように指定する方法がわからない
			var remarkEditor = new Editor { BackgroundColor = Color.Green, HeightRequest = 250 };
			var remark = new StackLayout { Children = {	new Label { Text = "備考" }, remarkEditor} };

			var input = new StackLayout { Spacing = 3, Children = { title, restTime, unitTime, term, remark },	};


			// 「戻る」ボタン
			var backButton = new Button
			{
				Text = "Back",
				FontSize = 30,
				BackgroundColor = Color.Aqua,
			};
			// タスクを追加しないで戻る
			backButton.Clicked += async (sender, e) =>
			{
				await Navigation.PopAsync();
			};

			// 「完了」ボタン。データの保存をする
			var saveButton = new Button
			{
				Text = "Save",
				FontSize = 30,
				BackgroundColor = Color.Aqua,
			};
			saveButton.Clicked += async (sender, e) =>
			{
				if (titleEntry.Text == "")
				{
					await DisplayAlert("エラー","タイトルを入力てください","OK");
				}
				else
				{
					IFile file = await rootFolder.CreateFileAsync(titleEntry.Text + ".txt", CreationCollisionOption.GenerateUniqueName );
					await file.WriteAllTextAsync(titleEntry.Text + '\n');
				}
				await DisplayAlert("Save", titleEntry.Text + "が保存されました。", "OK");
			};

			// test
			var fileListLabel = new Label { FontSize = 10, BackgroundColor = Color.Aqua };
			var searchButton = new Button
			{
				Text = "Search",
				FontSize = 30,
				BackgroundColor = Color.Aqua,
			};
			searchButton.Clicked += async (sender, e) =>
			{
				IList<IFile> fileList = await rootFolder.GetFilesAsync();
				foreach (var file in fileList)
				{
					fileListLabel.Text += file.Name + '\n';
				}
			};

			// test
			var loadButton = new Button
			{
				Text = "Load",
				FontSize = 30,
				BackgroundColor = Color.Aqua,
			};
			loadButton.Clicked += (sender, e) =>
			{
			};

			// backとsaveの間を埋めるラベル
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
				Children = { backButton, fillLabel, saveButton, searchButton,loadButton },
				VerticalOptions = LayoutOptions.Start,
				Spacing = 5,
			};

			// ページに配置
			Content = new StackLayout
			{
				// iOSのみ上部に空白を取る
				Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0),
				Children = { topBar, input, fileListLabel}
			};
		}
	}
}