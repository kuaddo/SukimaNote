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
	// タスクのListViewを作成。アクセスレベルが全てpublicだけど仕方ない
	public class TaskListView : ListView
	{
		//　タスクのリストを静的に保持。ObservableCollectionを使うとAddした時に自動更新ができる
		public static ObservableCollection<TaskData> taskList = new ObservableCollection<TaskData>();

		public TaskListView()
		{
			// 引っ張って更新を可能にする
			// IsPullToRefreshEnabled = true;

			ItemsSource = taskList;

			var cell = new DataTemplate(typeof(TextCell));
			cell.SetBinding(TextCell.TextProperty, "Title");
			cell.SetBinding(TextCell.DetailProperty, "Term");
			
			ItemTemplate = cell;
		}

		// 非同期メソッドで実装するしかなかった。
		// ConfigureAwait(false)で移行の処理をワーカースレッドに行わせることで、デッドロックを回避
		// UIの処理はメインスレッドしかできないようなので、ここではビジネスロジックのみを実行
		// usingでリソースを開放するためにstreamを使用
		public async static Task MakeTaskDataListAsync()
		{
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IList<IFile> files = await rootFolder.GetFilesAsync().ConfigureAwait(false);
			var taskDataArray = await Task.WhenAll(files.Select(async file => {
				using (Stream stream = await file.OpenAsync(FileAccess.Read))
				using (StreamReader sr = new StreamReader(stream))
				{
					string allText = sr.ReadToEnd();
					string[] propertyArray = allText.Split(':');
					return new TaskData
					{
						Title = propertyArray[0],
						RestTime = int.Parse(propertyArray[1]),
						Term = new DateTime(long.Parse(propertyArray[2])),
						UnitTime = int.Parse(propertyArray[3]),
						Remark = propertyArray[4],
					};
				}
			})).ConfigureAwait(false);

			taskList = new ObservableCollection<TaskData>(taskDataArray);
		}
	}

	// タスクの一覧を描画するページ
	public class TaskListPage : ContentPage
	{
		public TaskListPage()
		{
			Title = "タスク一覧";
			var listView = new TaskListView();
			// 引っ張って更新の処理。必要無くなったが残しておく
			/*listView.Refreshing += async (sender, e) =>
			{
				//await TaskListView.MakeTaskDataListAsync();
				listView.ItemsSource = TaskListView.taskList;
				listView.EndRefresh();
			};*/

			// 詳細ページに移行
			listView.ItemSelected += (sender, e) =>
			{
				TaskData taskData = e.SelectedItem as TaskData;
				var newPage = new TaskDetailPage();
				newPage.title.Text += taskData.Title;
				newPage.restTime.Text += TaskData.timeList[taskData.RestTime];
				newPage.term.Text += taskData.Term.ToString();
				newPage.unitTime.Text += TaskData.timeList[taskData.UnitTime];
				newPage.remark.Text += taskData.Remark;
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
				// TODO: NavigationではなくてMasterDetailのItemを入れ替えるように遷移させる
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
				if (TaskListView.taskList.Count == 0)
				{
					await DisplayAlert("Error", "タスクが存在しません", "OK");
				}
				else if (await DisplayAlert("Caution", "タスクを全て削除しますか?", "YES", "NO"))
				{
					// 一つづつ削除
					IFolder rootFolder = FileSystem.Current.LocalStorage;
					IList<IFile> deletefiles = await rootFolder.GetFilesAsync();
					await Task.WhenAll(deletefiles.Select(async file => await file.DeleteAsync()));
					// Clearメソッドを利用するとその下の行の代入が出来なくなる
					TaskListView.taskList = new ObservableCollection<TaskData>();
					listView.ItemsSource = TaskListView.taskList;
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
	public class TaskDetailPage : ContentPage
	{
		// 形式はTaskAddPageと同様
		// TODO: TaskDataプロパティを使いデータを受け取り、Labelはコンストラクタに記述する
		public Label title = new Label { Text = "Title: ", FontSize = 20 };
		public Label restTime = new Label { Text = "RestTime: ", FontSize = 20 };
		public Label term = new Label { Text = "Term: ", FontSize = 20 };
		public Label unitTime = new Label { Text = "UnitTime: ", FontSize = 20 };
		public Label remark = new Label { Text = "Remark: ", FontSize = 20 };

		public TaskDetailPage()
		{
			// プロパティを参照すれば正しく表示される
			Title = "タスク詳細(" + this.title.Text + ")";
			Content = new StackLayout { Children = { title, restTime, term, unitTime, remark } };
		}
	}
}