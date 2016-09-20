using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using PCLStorage;
using System.Threading.Tasks;

namespace SukimaNote
{
	/* TaskAdd.csのプロパティのクラスを使用
	public class TaskData
	{
		public string Title { get; set; }       // タイトル
		public int RestTime { get; set; }       // 予想残り時間
		public int UnitTime { get; set; }       // 最低単位作業時間
		public DateTime Term { get; set; }      // 期限。int型ではなく時間を表す型を探す
		public string Remark { get; set; }      // 備考
	}*/

	// タスクのListViewを作成
	public class TaskListView : ListView
	{
		public TaskListView()
		{
			// Listが返るまで待機
			// TODO: 待つ時間を指定して、何かあったときにフリーズしないようにする
			ItemsSource = MakeTaskListDataAsync().Result;

			var cell = new DataTemplate(typeof(TextCell));
			cell.SetBinding(TextCell.TextProperty, "Title");

			ItemTemplate = cell;
		}

		// 非同期メソッドで実装するしかなかった。
		// ConfigureAwait(false)で移行の処理をワーカースレッドに行わせることで、デッドロックを回避
		// UIの処理はメインスレッドしかできないようなので、ここではビジネスロジックのみを実行
		// staticにしてアプリ起動時にTaskListを作成し、タスクの削除・追加をした時のみに更新するべきか？
		private async static Task<List<TaskData>> MakeTaskListDataAsync()
		{
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IList<IFile> files = await rootFolder.GetFilesAsync().ConfigureAwait(false);
			var taskDataArray = await Task.WhenAll(files.Select(async file => {
				var allText = await file.ReadAllTextAsync();
				var propertyArray = allText.Split(':');
				return new TaskData { Title = propertyArray[0],
									  RestTime = int.Parse(propertyArray[1]),
									  UnitTime = int.Parse(propertyArray[2]),
									  Term = new DateTime(long.Parse(propertyArray[3])),
									  Remark = propertyArray[4],
									};
			})).ConfigureAwait(false);
			return taskDataArray.ToList();
		}
	}

	// タスクの一覧を描画するページ
	public class TaskListPage : ContentPage
	{
		IFolder rootFolder = FileSystem.Current.LocalStorage;

		public TaskListPage()
		{
			Title = "タスク一覧";
			var listView = new TaskListView();
			listView.ItemSelected += (sender, e) =>
			{
				TaskData taskData = e.SelectedItem as TaskData;
				var newPage = new TaskDetailPage();
				newPage.title.Text += taskData.Title;
				newPage.restTime.Text += newPage.ar[taskData.RestTime];
				newPage.unitTime.Text += newPage.ar[taskData.UnitTime];
				newPage.term.Text += taskData.Term.ToString();
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
			// 全て削除。タスク一覧のページで部分的に削除する機能を実装する
			allDeleteButton.Clicked += async (sender, e) =>
			{
				// 一つづつ削除
				IList<IFile> deletefiles = await rootFolder.GetFilesAsync();
				foreach (var delete in deletefiles)
				{
					await delete.DeleteAsync();
				}
			};

			Content = new StackLayout
			{
				Children = { listView, allDeleteButton, shiftButton	}
			};
		}
	}

	// タスクの詳細画面を描画するページ
	public class TaskDetailPage : ContentPage
	{
		public List<string> ar = Enumerable.Range(1, 60).Select(n => string.Format("{0}分", n)).ToList();

		// 形式はTaskAddPageと同様
		// TODO: プロパティに書き直す
		public Label title = new Label { Text = "Title: ", FontSize = 20 };
		public Label restTime = new Label { Text = "RestTime: ", FontSize = 20 };
		public Label unitTime = new Label { Text = "UnitTime: ", FontSize = 20 };
		public Label term = new Label { Text = "Term: ", FontSize = 20 };
		public Label remark = new Label { Text = "Remark: ", FontSize = 20 };

		public TaskDetailPage()
		{
			Content = new StackLayout { Children = { title, restTime, unitTime, term, remark } };
		}
	}
}