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

			var cell = new DataTemplate(typeof(TextCell));
			cell.SetBinding(TextCell.TextProperty, "Title");
			cell.SetBinding(TextCell.DetailProperty, "Deadline");
			
			ItemTemplate = cell;
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
					IList<IFile> deletefiles = await rootFolder.GetFilesAsync();
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
			Title = "タスク詳細:" + taskData.Title;
			Initialize(taskData);
		}
	}

	// TopPage DetailPageでのレイアウトの基本となるページ。そのまま使うことは多分ない
	// 縦向き7/10程度の大きさで丁度いいレイアウトになる
	public class BasicTaskShowPage : ContentPage
	{
		public Label title		  = new Label { FontSize = 60 };
		public Label restTime	  = new Label { FontSize = 60, Text = "restTime" ,HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center};
		public Label deadline	  = new Label { FontSize = 35 };
		public Label timeToFinish = new Label { FontSize = 25 };
		public Label place		  = new Label { FontSize = 25 };
		public Label priority	  = new Label { FontSize = 25 };
		public Label progress	  = new Label { FontSize = 25 };
		public Label remark		  = new Label { FontSize = 10 };
		public Frame frame		  = new Frame { OutlineColor = Color.Silver, HasShadow = true };

		public BasicTaskShowPage()
		{
			// タイトルと残り時間
			var sl1 = new StackLayout
			{
				BackgroundColor = Color.Pink,
				Orientation = StackOrientation.Horizontal,
				Children = { title, restTime }
			};

			// 場所
			var sl2 = new StackLayout
			{
				BackgroundColor = Color.Olive,
				Children =
				{
					new Label { Text = "場所:" },
					place
				}
			};

			// 優先度
			var sl3 = new StackLayout
			{
				BackgroundColor = Color.Lime,
				Orientation = StackOrientation.Horizontal,
				Children =
				{
					new Label { Text = "優先度:", FontSize = 50, VerticalOptions = LayoutOptions.Center },
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
					new Label { Text = "備考" },
					new ScrollView { Content = remark }
				}
			};

			var grid = new Grid();
			grid.Children.Add(sl1, 0, 10, 0, 3);
			grid.Children.Add(deadline, 0,  6, 3, 5);
			grid.Children.Add(sl2, 0,  6, 5, 7);
			grid.Children.Add(sl3, 0, 6, 7, 9);
			grid.Children.Add(sl4, 6, 10, 3, 9);
			grid.Children.Add(sl5, 0, 10, 9, 14);

			frame.Content = grid;

			Content = frame;
		}

		// Labelの初期化をする
		public void Initialize(TaskData taskData)
		{
			title.Text = taskData.Title;
			if (taskData.Deadline < DateTime.Now) { restTime.Text = "期限を過ぎています"; }
			else if (taskData.Deadline.Date == DateTime.Now.Date) { restTime.Text = "残り" + taskData.Deadline.TimeOfDay.Hours + "時間です"; }
			else { restTime.Text = "残り" + (taskData.Deadline.Date - DateTime.Now.Date) + "日です"; }
			deadline.Text = taskData.Deadline.ToString("F");
			timeToFinish.Text = SharedData.timeToFinishList[taskData.TimeToFinish];
			place.Text = SharedData.placeList[taskData.Place];
			priority.Text = SharedData.priorityList[taskData.Priority];
			progress.Text = taskData.Progress.ToString() + "%";
			remark.Text = taskData.Remark;
		}
	}
}