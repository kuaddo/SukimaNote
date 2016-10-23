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
	// タスクのListViewを作成
	public class TaskListView : ListView
	{
		public TaskListView()
		{
			ItemsSource = SharedData.taskList;
			ItemTemplate = makeDataTemplate();
		}

		private DataTemplate makeDataTemplate()
		{
			return new DataTemplate(() =>
			{
				var checkBox = new CheckBoxImage { IsClosed = true };
				checkBox.SetBinding(CheckBoxImage.IsClosedProperty, nameof(TaskData.Closed), BindingMode.TwoWay);
				var checkTGR = new TapGestureRecognizer();
				checkTGR.Tapped += (sender, e) =>
				{
					checkBox.IsClosed = !checkBox.IsClosed;
				};
				checkBox.GestureRecognizers.Add(checkTGR);
				var title = new Label();
				title.SetBinding(Label.TextProperty, nameof(TaskData.Title));
				var deadline = new Label();
				deadline.SetBinding(Label.TextProperty, nameof(TaskData.DeadlineString));
				var progress = new Label();
				progress.SetBinding(Label.TextProperty, nameof(TaskData.ProgressString));
				var view = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					Spacing = 20,
					Padding = new Thickness(10, 0, 0, 0),
					Children =
					{
						checkBox,
						new StackLayout { Children = { title, deadline } },
						progress
					}
				};
				return new ViewCell { View = view };
			});
		}
	}

	// タスクの一覧を描画するページ
	public class TaskListPage : ContentPage
	{
		public TaskListPage()
		{
			Title = "タスク一覧";
			var listView = new TaskListView();

			// 詳細ページに移行
			listView.ItemSelected += (sender, e) =>
			{
				var newPage = new TaskDetailPage(e.SelectedItem as TaskData);
				Navigation.PushAsync(newPage);
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
			// 全て削除。
			// TODO: タスク一覧のページで部分的に削除する機能を実装する
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
	}

	// タスクの詳細画面を描画するページ
	public class TaskDetailPage : BasicTaskShowPage
	{
		public TaskDetailPage(TaskData taskData)
		{
			Title = taskData.Title;
			Initialize(taskData);
		}
	}

	// TopPage DetailPageでのレイアウトの基本となるページ。そのまま使うことは多分ない
	// 縦向き7/10程度の大きさで丁度いいレイアウトになる
	public class BasicTaskShowPage : ContentPage
	{
		protected Label title		 = new Label { FontSize = 55, HorizontalOptions = LayoutOptions.Start,  VerticalOptions = LayoutOptions.CenterAndExpand };
		protected Label restTime	 = new Label { FontSize = 35, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, BackgroundColor = Color.Red};
		protected Label deadline	 = new Label { FontSize = 35, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
		protected Label timeToFinish = new Label { FontSize = 45, VerticalOptions = LayoutOptions.CenterAndExpand };
		protected Label place		 = new Label { FontSize = 40, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
		protected Label priority	 = new Label { FontSize = 40, HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
		protected Label progress	 = new Label { FontSize = 45 };
		protected Label remark		 = new Label { FontSize = 30 };
		protected Frame frame		 = new Frame { OutlineColor = Color.Silver, HasShadow = true };

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
			var sl4 = new StackLayout
			{
				BackgroundColor = Color.Maroon,
				Children =
				{
					progress,
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
		}
	}
}