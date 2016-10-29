using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Xamarin.Forms;
using PCLStorage;
using System.Threading.Tasks;

namespace SukimaNote
{
	// TaskListViewのCell
	public class TaskListViewCell : ViewCell
	{
		public const int fontSize = 25;

		public TaskListViewCell(TaskListPage taskListPage)
		{
			// Cellを構成するView
			var priorityLabel = new Label { BackgroundColor = Color.Aqua };
			var checkBox	  = new CheckBoxImage { IsClosed = true };
			var title		  = new Label { FontSize = fontSize, BackgroundColor = Color.White };
			var deadline	  = new Label { FontSize = fontSize - 10, BackgroundColor = Color.Pink };
			var progress	  = new Label { FontSize = fontSize + 10, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, BackgroundColor = Color.Navy };
			var progressBar   = new TaskProgressBar { VerticalOptions = LayoutOptions.Center};
			var checkTGR	  = new TapGestureRecognizer();
			checkTGR.Tapped += (sender, e) => {	checkBox.IsClosed = !checkBox.IsClosed;	};
			checkBox.GestureRecognizers.Add(checkTGR);
			var actionDelete = new MenuItem
			{
				Text = "Delete",
				IsDestructive = true,	// iOSでメニューアイテムを赤色にする
			};
			actionDelete.Clicked += async (sender, e) =>
			{
				var taskData = (sender as MenuItem).CommandParameter as TaskData;
				if (await taskListPage.DisplayAlert("Caution", taskData.Title + "を削除しますか?", "YES", "NO"))
					taskListPage.deleteTask(taskData);
			};

			// ViewとTaskDataのバインディング
			priorityLabel.SetBinding(Label.BackgroundColorProperty,			 nameof(TaskData.PriorityColor));
			checkBox	 .SetBinding(CheckBoxImage.IsClosedProperty,		 nameof(TaskData.Closed), BindingMode.TwoWay);
			title		 .SetBinding(Label.TextProperty,					 nameof(TaskData.Title));
			deadline	 .SetBinding(Label.TextProperty,					 nameof(TaskData.DeadlineString));
			progress	 .SetBinding(Label.TextProperty,					 nameof(TaskData.ProgressString));
			progressBar  .SetBinding(TaskProgressBar.TaskProgressProperty,   nameof(TaskData.Progress));

			// コンテキストアクションに追加
			actionDelete.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
			ContextActions.Add(actionDelete);

			// レイアウト
			var sl = new StackLayout { Children = { title, deadline }, Spacing = 0 };
			var view = new Grid();  // 列0~25 行0~5
			view.Children.Add(priorityLabel, 0,  1,  0, 7);
			view.Children.Add(checkBox,		 1,  3,  1, 6);
			view.Children.Add(sl,			 3,  20, 0, 6);
			view.Children.Add(progress,		 20, 27, 0, 6);
			view.Children.Add(progressBar,   3,  27, 6, 7);

			View = view;
		}
	}

	// タスクの一覧を描画するページ
	public class TaskListPage : ContentPage
	{
		public TaskListPage()
		{
			Title = "タスク一覧";
			var listView = new ListView
			{
				ItemsSource = SharedData.taskList,
				ItemTemplate = new DataTemplate(() => new TaskListViewCell(this)),
				RowHeight = (int)(TaskListViewCell.fontSize * 2.4)
			};

			// 詳細ページに移行
			listView.ItemSelected += (sender, e) =>
			{
				if (e.SelectedItem != null)
				{
					var newPage = new TaskDetailPage(e.SelectedItem as TaskData);
					listView.SelectedItem = null;	// nullにすることで同じアイテムを連続選択できる
					Navigation.PushAsync(newPage);
				}
			};
	
			// TaskAddPageへ遷移するボタン
			var shiftButton = new Button
			{
				Text = "タスクの追加ページヘ移行",
				FontSize = 30,
				BackgroundColor = Color.Aqua,
			};
			shiftButton.Clicked += async (sender, e) =>
			{
				await Navigation.PushAsync(new TaskAddPage());
			};

			// タスクを全削除するボタン
			var allDeleteButton = new Button
			{
				Text = "全てのタスクの削除",
				FontSize = 30,
				BackgroundColor = Color.Aqua,
			};
			allDeleteButton.Clicked += async (sender, e) =>
			{
				if (SharedData.taskList.Count == 0)
				{
					await DisplayAlert("Error", "タスクが存在しません", "OK");
				}
				else if (await DisplayAlert("Caution", "タスクを全て削除しますか?", "YES", "NO"))
				{
					// ローカルストレージ上の削除
					IFolder rootFolder = FileSystem.Current.LocalStorage;
					IFolder taskDataFolder = await rootFolder.CreateFolderAsync("taskDataFolder", CreationCollisionOption.OpenIfExists);    // 存在しなかったならば作成
					IList<IFile> deletefiles = await taskDataFolder.GetFilesAsync();
					await Task.WhenAll(deletefiles.Select(async file => await file.DeleteAsync()));
					// 現在読み込まれているリストからの削除
					SharedData.taskList.Clear();
					await DisplayAlert("Deleted", "削除しました", "OK");
				}
			};

			Content = new StackLayout
			{
				Children = { listView, allDeleteButton, shiftButton }
			};
		}

		// コンテキストアクションでタスクの削除時に呼ばれるメソッド
		public async void deleteTask(TaskData taskData)
		{
			SharedData.taskList.RemoveAt(SharedData.taskList.IndexOf(taskData));
			var deleteFile = await SharedData.searchFileAsync(taskData);
			await deleteFile.DeleteAsync();
		}
	}

	// タスクの詳細画面を描画するページ
	public class TaskDetailPage : BasicTaskShowPage
	{
		public TaskDetailPage(TaskData taskData)
		{
			Title = taskData.Title;
			Initialize(taskData);

			var progressSlider = new Slider
			{
				Minimum = 0,
				Maximum = 100,
			};
			progressSlider.ValueChanged += (sender, e) =>
			{
				progress.Text = ((int)progressSlider.Value).ToString() + "%";
				roundProgressBar.Angle = (int)progressSlider.Value;
			};
			progressSlider.Value = taskData.Progress;
			var progressLabel = new Label { Text = "進捗", FontSize = 15 };
			var progressSave = new Button { Text = "save", FontSize = 15 };
			progressSave.Clicked += async (sender, e) =>
			{
				IFile updateFile = await SharedData.searchFileAsync(taskData);
				taskData.Progress = (int)progressSlider.Value;
				await updateFile.WriteAllTextAsync(SharedData.makeSaveString(taskData));
			};

			var grid = new Grid();
			grid.Children.Add(frame, 0, 10, 0, 9);
			grid.Children.Add(progressLabel, 0, 2, 9, 10);
			grid.Children.Add(progressSave, 8, 10, 9, 10);
			grid.Children.Add(progressSlider, 2, 8, 9, 10);
			Content = grid;
		}
	}

	// TopPage DetailPageでのレイアウトの基本となるページ。そのまま使うことは多分ない
	// 縦向き7/10程度の大きさで丁度いいレイアウトになる
	public class BasicTaskShowPage : ContentPage
	{
		protected Label title		 = new Label { FontSize = 55, HorizontalOptions = LayoutOptions.Start,			 VerticalOptions = LayoutOptions.CenterAndExpand };
		protected Label restTime	 = new Label { FontSize = 20, HorizontalOptions = LayoutOptions.FillAndExpand,   VerticalOptions = LayoutOptions.CenterAndExpand, BackgroundColor = Color.Red};
		protected Label deadline	 = new Label { FontSize = 35, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
		protected Label timeToFinish = new Label { FontSize = 20, HorizontalOptions = LayoutOptions.Center };
		protected Label place		 = new Label { FontSize = 40, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
		protected Label priority	 = new Label { FontSize = 40, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
		protected Label progress	 = new Label { FontSize = 45, HorizontalOptions = LayoutOptions.Fill,			 VerticalOptions = LayoutOptions.Fill,
																  HorizontalTextAlignment = TextAlignment.Center,	 VerticalTextAlignment = TextAlignment.Center,   TextColor = Color.Black };
		protected Label remark		 = new Label { FontSize = 30 };
		protected Frame frame		 = new Frame { OutlineColor = Color.Silver, HasShadow = true };

		// 背景色で内側の円を消しているのでColorは必須
		protected RoundProgressBar roundProgressBar = new RoundProgressBar { Color = Color.Red, StrokeColor = Color.White, StrokeWidth = 0.7f};

		public BasicTaskShowPage()
		{
			// タイトルと残り時間
			var grid1 = new Grid { BackgroundColor = Color.Pink };
			grid1.Children.Add(title   , 0, 3, 0, 1);
			grid1.Children.Add(restTime, 3, 4, 0, 1);
			grid1.Padding = new Thickness(20, 0, 0, 0);

			// 期限
			var sl1 = new StackLayout
			{
				Children =
				{
					new Label { Text = "期限:", FontSize = 20 },
					deadline
				}
			};

			// 場所
			var sl2 = new StackLayout
			{
				BackgroundColor = Color.Olive,
				Children =
				{
					new Label { Text = "場所:", FontSize = 20 },
					place
				}
			};

			// 優先度
			var sl3 = new StackLayout
			{
				BackgroundColor = Color.Lime,
				Children =
				{
					new Label { Text = "優先度:", FontSize = 20 },
					priority
				}
			};

			// 予想作業時間と進捗度
			var progressView = new ContentView
			{
				Content = new Grid	// Girdで重ねて表示
				{
					Children = {roundProgressBar, progress}
				}
			};
			var sl4 = new StackLayout
			{
				BackgroundColor = Color.Maroon,
				Padding = new Thickness(0, 7, 0, 0),
				Spacing = 0,
				Children =
				{
					progressView,
					timeToFinish
				}
			};

			// 備考
			var sl5 = new StackLayout
			{
				BackgroundColor = Color.Aqua,
				Children =
				{
					new Label { Text = "備考:", FontSize = 20},
					new ScrollView { Content = remark }
				}
			};

			var grid = new Grid();
			grid.Children.Add(grid1, 0, 10, 0, 3);
			grid.Children.Add(sl1, 0,  6, 3, 5);
			grid.Children.Add(sl2, 0,  6, 5, 7);
			grid.Children.Add(sl3, 0, 6, 7, 9);
			grid.Children.Add(sl4, 6, 10, 3, 9);
			grid.Children.Add(sl5, 0, 10, 9, 14);

			frame.Content = grid;

			Content = frame;
		}

		// Labelの初期化をする
		protected void Initialize(TaskData taskData)
		{
			title.Text		  = taskData.Title;
			if		(taskData.Closed == true)		   { restTime.Text = "終了" + Environment.NewLine + "済み"; }
			else if (taskData.Deadline < DateTime.Now) { restTime.Text = ""; }
			else if (taskData.MinutesByDeadline < 60)  { restTime.Text = "残り" + Environment.NewLine + taskData.MinutesByDeadline + "分"; }
			else if (taskData.HoursByDeadline < 24)	   { restTime.Text = "残り" + Environment.NewLine + taskData.HoursByDeadline   + "時間"; }
			else									   { restTime.Text = "残り" + Environment.NewLine + taskData.DaysByDeadline    + "日"; }
			deadline.Text	  = taskData.Deadline.ToString("F");
			timeToFinish.Text = SharedData.timeToFinishList[taskData.TimeToFinish];
			place.Text		  = SharedData.placeList[taskData.Place];
			priority.Text	  = SharedData.priorityList[taskData.Priority];
			progress.Text	  = taskData.Progress.ToString() + "%";
			remark.Text		  = taskData.Remark;

			roundProgressBar.Angle = taskData.Progress;
		}
	}
}