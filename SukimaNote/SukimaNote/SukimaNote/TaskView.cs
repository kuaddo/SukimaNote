using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Xamarin.Forms;
using PCLStorage;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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
			cell.SetBinding(TextCell.DetailProperty, "Term");
			
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
	public class BasicTaskShowPage : ContentPage
	{
		public Label title	  = new Label { FontSize = 60 };
		public Label term     = new Label { FontSize = 35 };
		public Label restTime = new Label { FontSize = 25 };
		public Label unitTime = new Label { FontSize = 25 };
		public Label place	  = new Label { FontSize = 25 };
		public Label priority = new Label { FontSize = 25 };
		public Label remark   = new Label { FontSize = 10 };
		public Frame frame    = new Frame();

		public BasicTaskShowPage()
		{
			// 期限
			var sl1 = new StackLayout
			{
				Spacing = 10,
				Orientation = StackOrientation.Horizontal,
				Children =
				{
					new Label { Text = "期限:" },
					term
				}
			};

			// 予想残り時間
			var sl2 = new StackLayout
			{
				Children =
				{
					new Label { Text = "予想残り時間" },
					restTime
				}
			};

			// 最低単位作業時間
			var sl3 = new StackLayout
			{
				Children =
				{
					new Label { Text = "最低単位作業時間" },
					unitTime
				}
			};

			// 場所
			var sl4 = new StackLayout
			{
				Children =
				{
					new Label { Text = "場所" },
					place
				}
			};

			// 優先度
			var sl5 = new StackLayout
			{
				Children =
				{
					new Label { Text = "優先度" },
					priority
				}
			};

			// 備考
			var sl6 = new StackLayout
			{
				Children =
				{
					new Label { Text = "備考" },
					remark
				}
			};

			frame = new Frame
			{
				OutlineColor = Color.Silver,
				HasShadow = true,
				Padding = new Thickness(30, 30, 30, 30),
				Content = new StackLayout
				{
					Padding = new Thickness(10, 0, 0, 0),
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalOptions = LayoutOptions.FillAndExpand,
					Children =
					{
						title,
						sl1,
						new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 30, Children = { sl2, sl3 } },
						sl4,
						sl5,
						sl6
					}
				}
			};

			Content = frame;
		}

		// Labelの初期化をする
		public void Initialize(TaskData taskData)
		{
			title.Text = taskData.Title;
			term.Text = taskData.Term.ToString("F");
			restTime.Text = SharedData.restTimeList[taskData.RestTime];
			unitTime.Text = SharedData.unitTimeList[taskData.UnitTime];
			place.Text = taskData.Place;
			priority.Text = SharedData.priorityString[taskData.Priority];
			remark.Text = taskData.Remark;
		}
	}
}