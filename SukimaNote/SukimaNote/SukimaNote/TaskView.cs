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
		private TaskListView(List<TaskData> taskDataList)
		{
			ItemsSource = taskDataList;

			var cell = new DataTemplate(typeof(TextCell));
			cell.SetBinding(TextCell.TextProperty, "Title");

			ItemTemplate = cell;
		}

		// 非同期メソッドで実装するしかなかった。ListViewのデータクラス
		public async static Task<TaskListView> BuildTaskListViewAsync()
		{
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IList<IFile> files = await rootFolder.GetFilesAsync().ConfigureAwait(false);
			var taskDataArray = await Task.WhenAll(files.Select(async file => {
				var title = await file.ReadAllTextAsync();
				return new TaskData { Title = title };
			})).ConfigureAwait(false);
			return new TaskListView(taskDataArray.ToList());
		}
	}

	// タスクの一覧を描画するページ
	public class TaskListPage : ContentPage
	{
		IFolder rootFolder = FileSystem.Current.LocalStorage;

		public TaskListPage()
		{
			Title = "タスク一覧";
			TaskListView listView = TaskListView.BuildTaskListViewAsync().Result;

			var updateButton = new Button
			{
				Text = "Update",
				FontSize = 30,
				BackgroundColor = Color.Aqua,
			};
			/*updateButton.Clicked += async (sender, e) =>
			{
				
			};*/
		
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
				Children =
				{
					listView,
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						Children = { updateButton, allDeleteButton }
					},
					shiftButton
				},
			};
		}
	}

	// タスクの詳細画面を描画するページ
	public class TaskDetailPage : ContentPage
	{

	}
}