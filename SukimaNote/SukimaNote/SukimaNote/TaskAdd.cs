using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using PCLStorage;

namespace SukimaNote
{
	// タスクの追加の設定ページ
	public class TaskAddPage : ContentPage
	{
		private const int descriptionFontSize = 13;

		private Entry	   titleEntry		  = new Entry	   { BackgroundColor = Color.FromHex(MyColor.MainColor1), Keyboard = Keyboard.Text, FontSize = descriptionFontSize + 7 };
		private DatePicker deadlineDatePicker = new DatePicker { BackgroundColor = Color.FromHex(MyColor.MainColor1), HorizontalOptions = LayoutOptions.FillAndExpand };
		private TimePicker deadlineTimePicker = new TimePicker { BackgroundColor = Color.FromHex(MyColor.MainColor1), HorizontalOptions = LayoutOptions.FillAndExpand };
		private Picker	   timeToFinishPicker = new Picker	   { BackgroundColor = Color.FromHex(MyColor.MainColor1) };
		private Picker	   placePicker		  = new Picker	   { BackgroundColor = Color.FromHex(MyColor.MainColor1) };
		private Picker	   priorityPicker	  = new Picker	   { BackgroundColor = Color.FromHex(MyColor.MainColor1) };
		private Editor	   remarkEditor		  = new Editor	   { BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = 180, FontSize = descriptionFontSize + 7 };

		private TaskData taskData;

		public TaskAddPage()
		{
			Title = "タスク追加";
			
			// タスクのデータ入力部分
			var title		 = makeTitleStackLayout();
			var deadline	 = makeDeadlineStackLayout();
			var timeToFinish = makeTimeToFinishStackLayout();
			var place		 = makePlaceStackLayout();
			var priority	 = makePriorityStackLayout();
			var remark		 = makeRemarkStackLayout();

			// 日にちと時刻設定以外をバインディング
			taskData = new TaskData();
			BindingContext = taskData;

			titleEntry		  .SetBinding(Entry.TextProperty,			nameof(taskData.Title));
			timeToFinishPicker.SetBinding(Picker.SelectedIndexProperty, nameof(taskData.TimeToFinish));
			placePicker		  .SetBinding(Picker.SelectedIndexProperty, nameof(taskData.Place));
			priorityPicker	  .SetBinding(Picker.SelectedIndexProperty, nameof(taskData.Priority));
			remarkEditor	  .SetBinding(Editor.TextProperty,			nameof(taskData.Remark));

			// Deadlineの初期値
			deadlineDatePicker.Date = new DateTime(DateTime.Now.Ticks + TimeSpan.TicksPerDay);		// 次の日
			deadlineTimePicker.Time = new TimeSpan(DateTime.Now.Ticks - DateTime.Now.Date.Ticks);   // 時刻は同じ

			// セーブのスタックレイアウト
			var save1 = makeSaveStackLayout();
			var save2 = makeSaveStackLayout();

			// 基本設定、追加設定を分けて配置
			var minimumLabel = new Label { Text = "基本設定", FontSize = descriptionFontSize + 12, HorizontalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.FromHex(MyColor.MainColor3), TextColor = Color.White };
			var optionLabel  = new Label { Text = "追加設定", FontSize = descriptionFontSize + 12, HorizontalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.FromHex(MyColor.MainColor3), TextColor = Color.White };

			var minimum = new StackLayout
			{
				BackgroundColor = Color.FromHex(MyColor.MainColor2),
				Padding = new Thickness(0, 0, 0, 40),
				Children = { minimumLabel, title, deadline, timeToFinish, save1 }
			};
			var option = new StackLayout
			{
				BackgroundColor = Color.FromHex(MyColor.MainColor2),
				Padding = new Thickness(0, 0, 0, 40),
				Children = { optionLabel, place, priority, remark, save2 }
			};

			Content = new ScrollView
			{
				BackgroundColor = Color.FromHex(MyColor.MainColor2),
				Content = new StackLayout
				{
					// iOSのみ上部に空白を取る。Navigationだから必要ない?
					//Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0),
					Children = { minimum, option }
				}
			};
		}

		// ページに配置するスタックレイアウトを作成するメソッド
		private StackLayout makeTitleStackLayout()
		{
			var grid = makeSupplementaryGrid("タイトル", "作業のわかり易い名前");
			return new StackLayout { Children = { grid, titleEntry } };
		}
		private StackLayout makeDeadlineStackLayout()
		{
			// GridでPickerを正確に半分にして表示
			var deadlineGrid = new Grid();
			deadlineGrid.Children.Add(deadlineDatePicker, 0, 1, 0, 1);
			deadlineGrid.Children.Add(deadlineTimePicker, 1, 2, 0, 1);
			var grid = makeSupplementaryGrid("期限", "作業を完了させたい日時");
			return new StackLayout { Children =	{ grid, deadlineGrid }	};
		}
		private StackLayout makeTimeToFinishStackLayout()
		{
			foreach (var time in SharedData.timeToFinishList) { timeToFinishPicker.Items.Add(time); }
			var grid = makeSupplementaryGrid("予想作業時間", "作業に必要な時間");
			return new StackLayout { Children = { grid, timeToFinishPicker } };
		}
		private StackLayout makePlaceStackLayout()
		{
			foreach (var p in SharedData.placeList) { placePicker.Items.Add(p); };
			var grid = makeSupplementaryGrid("場所", "作業できる場所");
			return new StackLayout { Children = { grid, placePicker } };
		}
		private StackLayout makePriorityStackLayout()
		{
			foreach (var p in SharedData.priorityList) { priorityPicker.Items.Add(p); };
			var grid = makeSupplementaryGrid("優先度", "作業の優先度");
			return new StackLayout { Children = { grid, priorityPicker } };
		}
		private StackLayout makeRemarkStackLayout()
		{
			var grid = makeSupplementaryGrid("備考", "メモとして記録しておきたいこと");
			return new StackLayout { Children = { grid, remarkEditor } };
		}
		private StackLayout makeSaveStackLayout()
		{
			// 「完了」ボタン。データの保存をする
			var saveButton = new Button
			{
				Text = "Save",
				FontSize = descriptionFontSize + 13,
				BackgroundColor = Color.FromHex(MyColor.MainColor1),
			};
			saveButton.Clicked += async (sender, e) =>
			{
				if (titleEntry.Text == "")
				{
					await DisplayAlert("Error", "タイトルを入力てください", "OK");
				}
				else if (titleEntry.Text.IndexOf(":") >= 0)
				{
					await DisplayAlert("Error", "タイトルに半角のセミコロン : は使えません", "OK");
				}
				else if (remarkEditor.Text.IndexOf(":") >= 0)
				{
					await DisplayAlert("Error", "備考に半角のセミコロン : は使えません", "OK");
				}
				else if (deadlineDatePicker.Date.Ticks + deadlineTimePicker.Time.Ticks < DateTime.Now.Ticks)
				{
					await DisplayAlert("Error", "期限が過去に設定されています", "OK");
				}
				else
				{
					taskData.Deadline = new DateTime(deadlineDatePicker.Date.Ticks + deadlineTimePicker.Time.Ticks);
					await saveTaskAsync(taskData);
					// 元の画面に戻す
					//await Navigation.PopAsync();
				}
			};
			return new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(0, 10, 0, 0),
				Children = { new Label { HorizontalOptions = LayoutOptions.FillAndExpand }, saveButton }
			};
		}

		// 与えられたTaskDataをファイルに保存して、taskListに追加するメソッド。makeSaveStackLayoutに使う
		private async Task saveTaskAsync(TaskData taskData)
		{
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IFolder taskDataFolder = await rootFolder.CreateFolderAsync("taskDataFolder", CreationCollisionOption.OpenIfExists);	// 存在しなかったならば作成

			IFile file = await taskDataFolder.CreateFileAsync(taskData.Title + ".txt", CreationCollisionOption.GenerateUniqueName);
			await file.WriteAllTextAsync(taskData.Title + ':' +
										 taskData.Deadline.Ticks.ToString() + ':' +
										 taskData.TimeToFinish.ToString() + ':' +
										 taskData.Place.ToString() + ':' +
										 taskData.Priority.ToString() + ':' +
										 taskData.Progress.ToString() + ':' +
										 taskData.Remark + ':' +
										 taskData.Closed.ToString() + ':');
			SharedData.taskList.Add(taskData);
			await DisplayAlert("Saved", taskData.Title + "が保存されました。", "OK");
		}
		// 補足説明のレイアウト作成
		private Grid makeSupplementaryGrid(string title, string sup)
		{
			var supplementary = new Label { Text = sup, FontSize = descriptionFontSize, IsVisible = false };
			var TGR = new TapGestureRecognizer();	// タップの判別
			TGR.Tapped += (sender, e) =>
			{
				supplementary.IsVisible = !supplementary.IsVisible;
			};
			var image = new Image { Source = "question.png", WidthRequest = descriptionFontSize, HeightRequest = descriptionFontSize };
			image.GestureRecognizers.Add(TGR);

			var grid = new Grid { Padding = new Thickness(5, 0, 0, 0) };
			grid.Children.Add(new Label { Text = title, FontSize = descriptionFontSize }, 0, 3, 0, 1);
			grid.Children.Add(new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 7, Children = { image, supplementary } }, 3, 10, 0, 1);

			return grid;
		}
	}
}