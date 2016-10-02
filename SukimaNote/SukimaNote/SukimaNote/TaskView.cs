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
				TaskData taskData = e.SelectedItem as TaskData;
				var newPage = new TaskDetailPage();
				newPage.title.Text += taskData.Title;
				newPage.term.Text += taskData.Term.ToString();
				newPage.restTime.Text += SharedData.restTimeList[taskData.RestTime];
				newPage.unitTime.Text += SharedData.unitTimeList[taskData.UnitTime];
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
	public class TaskDetailPage : ContentPage
	{
		public Label title = new Label { Text = "Title: ", FontSize = 20 };
		public Label term = new Label { Text = "Term: ", FontSize = 20 };
		public Label restTime = new Label { Text = "RestTime: ", FontSize = 20 };
		public Label unitTime = new Label { Text = "UnitTime: ", FontSize = 20 };
		public Label remark = new Label { Text = "Remark: ", FontSize = 20 };

		// TODO: TaskDataを受け取ってViewを変化させるようにする。TopPageに流用する
		public TaskDetailPage()
		{
			// プロパティを参照すれば正しく表示される
			Title = "タスク詳細(" + this.title.Text + ")";
			Content = new StackLayout { Children = { title, term, restTime, unitTime, remark } };
		}
	}
}