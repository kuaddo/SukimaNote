using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using PCLStorage;

namespace SukimaNote
{
	// タスクの追加の設定ページ
	public class TaskAddPage : ContentPage
	{
		private const int fontSize = 20;
		private const int inputSize = 50;

		private Entry	   titleEntry		  = new Entry	   { BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize, Keyboard = Keyboard.Text, FontSize = 30 };
		private DatePicker deadlineDatePicker = new DatePicker { BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize, HorizontalOptions = LayoutOptions.FillAndExpand };
		private TimePicker deadlineTimePicker = new TimePicker { BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize, HorizontalOptions = LayoutOptions.FillAndExpand };
		private Picker	   timeToFinishPicker = new Picker	   { BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize };
		private Picker	   placePicker		  = new Picker	   { BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize };
		private Picker	   priorityPicker	  = new Picker	   { BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize };
		private Editor	   remarkEditor		  = new Editor	   { BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = 500, FontSize = fontSize };

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

			taskData = new TaskData();
			BindingContext = taskData;

			// 日にちと時刻設定以外をバインディング
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
			var minimumLabel = new Label { Text = "基本設定", FontSize = 30, HorizontalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.FromHex(MyColor.MainColor3), TextColor = Color.White };
			var optionLabel = new Label { Text = "追加設定", FontSize = 30, HorizontalOptions = LayoutOptions.Fill,
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
			var titleSupplementary = new Label { Text = "予定にわかり易い名前をつけてください", FontSize = fontSize, IsVisible = false };
			var titleTGR = new TapGestureRecognizer();
			titleTGR.Tapped += (sender, e) =>
			{
				titleSupplementary.IsVisible = !titleSupplementary.IsVisible;
			};
			var titleImage = new Image { Source = "question.png", WidthRequest = fontSize, HeightRequest = fontSize };
			titleImage.GestureRecognizers.Add(titleTGR);

			return new StackLayout
			{
				Children =
				{
					new StackLayout
					{
						Padding = new Thickness(5, 0, 0, 0),
						Spacing = 7,
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							new Label { Text = "タイトル　　　　　　", FontSize = fontSize},
							titleImage,
							titleSupplementary
						}
					},
					titleEntry
				}
			};
		}
		private StackLayout makeDeadlineStackLayout()
		{
			var deadlineSupplementary = new Label { Text = "作業の期限を入力してください", FontSize = fontSize, IsVisible = false };
			var deadlineTGR = new TapGestureRecognizer();
			deadlineTGR.Tapped += (sender, e) =>
			{
				deadlineSupplementary.IsVisible = !deadlineSupplementary.IsVisible;
			};
			var deadlineImage = new Image { Source = "question.png", WidthRequest = fontSize, HeightRequest = fontSize };
			deadlineImage.GestureRecognizers.Add(deadlineTGR);

			return new StackLayout
			{
				Children =
				{
					new StackLayout
					{
						Padding = new Thickness(5, 0, 0, 0),
						Spacing = 7,
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							new Label { Text = "期限　　　　　　　　", FontSize = fontSize},
							deadlineImage,
							deadlineSupplementary
						}
					},
					new StackLayout
					{
						Spacing = 7,
						Orientation = StackOrientation.Horizontal,
						Children = { deadlineDatePicker, deadlineTimePicker }
					}
				}
			};
		}
		private StackLayout makeTimeToFinishStackLayout()
		{
			foreach (var time in SharedData.timeToFinishList) { timeToFinishPicker.Items.Add(time); }
			var timeToFinishSupplementary = new Label { Text = "作業完了に必要な時間を入力してください", FontSize = fontSize, IsVisible = false };
			var timeToFinishTGR = new TapGestureRecognizer();
			timeToFinishTGR.Tapped += (sender, e) =>
			{
				timeToFinishSupplementary.IsVisible = !timeToFinishSupplementary.IsVisible;
			};
			var timeToFinishImage = new Image { Source = "question.png", WidthRequest = fontSize, HeightRequest = fontSize };
			timeToFinishImage.GestureRecognizers.Add(timeToFinishTGR);

			return new StackLayout
			{
				Children =
				{
					new StackLayout
					{
						Padding = new Thickness(5, 0, 0, 0),
						Spacing = 7,
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							new Label { Text = "予想作業時間　　　　", FontSize = fontSize},
							timeToFinishImage,
							timeToFinishSupplementary
						}
					},
					timeToFinishPicker
				}
			};
		}
		private StackLayout makePlaceStackLayout()
		{
			foreach (var p in SharedData.placeList) { placePicker.Items.Add(p); };
			var placeSupplementary = new Label { Text = "作業できる場所を選択してください", FontSize = fontSize, IsVisible = false };
			var placeTGR = new TapGestureRecognizer();
			placeTGR.Tapped += (sender, e) =>
			{
				placeSupplementary.IsVisible = !placeSupplementary.IsVisible;
			};
			var placeImage = new Image { Source = "question.png", WidthRequest = fontSize, HeightRequest = fontSize };
			placeImage.GestureRecognizers.Add(placeTGR);

			return new StackLayout
			{
				Children =
				{
					new StackLayout
					{
						Padding = new Thickness(5, 0, 0, 0),
						Spacing = 7,
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							new Label { Text = "場所　　　　　　　　", FontSize = fontSize},
							placeImage,
							placeSupplementary
						}
					},
					placePicker
				}
			};
		}
		private StackLayout makePriorityStackLayout()
		{
			foreach (var p in SharedData.priorityList) { priorityPicker.Items.Add(p); };
			var prioritySupplementary = new Label { Text = "優先度を選択してください", FontSize = fontSize, IsVisible = false };
			var priorityTGR = new TapGestureRecognizer();
			priorityTGR.Tapped += (sender, e) =>
			{
				prioritySupplementary.IsVisible = !prioritySupplementary.IsVisible;
			};
			var priorityImage = new Image { Source = "question.png", WidthRequest = fontSize, HeightRequest = fontSize };
			priorityImage.GestureRecognizers.Add(priorityTGR);

			return new StackLayout
			{
				Children =
				{
					new StackLayout
					{
						Padding = new Thickness(5, 0, 0, 0),
						Spacing = 7,
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							new Label { Text = "優先度　　　　　　　", FontSize = fontSize},
							priorityImage,
							prioritySupplementary
						}
					},
					priorityPicker
				}
			};
		}
		private StackLayout makeRemarkStackLayout()
		{
			var remarkSupplementary = new Label { Text = "メモとして記録しておきたいことを入力してください", FontSize = fontSize, IsVisible = false };
			var remarkTGR = new TapGestureRecognizer();
			remarkTGR.Tapped += (sender, e) =>
			{
				remarkSupplementary.IsVisible = !remarkSupplementary.IsVisible;
			};
			var remarkImage = new Image { Source = "question.png", WidthRequest = fontSize, HeightRequest = fontSize };
			remarkImage.GestureRecognizers.Add(remarkTGR);

			return new StackLayout
			{
				Children =
				{
					new StackLayout
					{
						Padding = new Thickness(5, 0, 0, 0),
						Spacing = 7,
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							new Label { Text = "備考　　　　　　　　", FontSize = fontSize},
							remarkImage,
							remarkSupplementary
						}
					},
					remarkEditor
				}
			};
		}
		private StackLayout makeSaveStackLayout()
		{
			// 「完了」ボタン。データの保存をする
			var saveButton = new Button
			{
				Text = "Save",
				FontSize = 40,
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
				Children = { new Label { HorizontalOptions = LayoutOptions.FillAndExpand }, saveButton }
			};
		}

		// 与えられたTaskDataをファイルに保存して、taskListに追加するメソッド。makeSaveStackLayoutに使う
		private async Task saveTaskAsync(TaskData taskData)
		{
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IFile file = await rootFolder.CreateFileAsync(taskData.Title + ".txt", CreationCollisionOption.GenerateUniqueName);
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
	}
}