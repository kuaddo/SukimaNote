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
		// listが正常に作成されたかのフラグ
		// Dispose処理がうまく行けば必要ないと思う
		public bool SuccessFlag { get; set; }

		public TaskListView()
		{
			// 待つ時間を指定して、何かあったときにフリーズしないようにする
			var task = MakeTaskListDataAsync();
			if (task.Wait(1000) == true)
			{
				ItemsSource = MakeTaskListDataAsync().Result;
				SuccessFlag = true;
			}
			else
			{
				ItemsSource = new List<TaskData>();
				SuccessFlag = false;
			}

			var cell = new DataTemplate(typeof(TextCell));
			cell.SetBinding(TextCell.TextProperty, "Title");
			cell.SetBinding(TextCell.DetailProperty, "Term");
			
			ItemTemplate = cell;
		}

		// 非同期メソッドで実装するしかなかった。
		// ConfigureAwait(false)で移行の処理をワーカースレッドに行わせることで、デッドロックを回避
		// UIの処理はメインスレッドしかできないようなので、ここではビジネスロジックのみを実行
		// usingを使うためにstreamを使用
		private async static Task<List<TaskData>> MakeTaskListDataAsync()
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
						UnitTime = int.Parse(propertyArray[2]),
						Term = new DateTime(long.Parse(propertyArray[3])),
						Remark = propertyArray[4],
					};
				}
			})).ConfigureAwait(false);

			// リソースを開放する。Disposeが実装されていないためGCにやってもらうしかない
			//GC.Collect();

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
			// 詳細ページに移行
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

			if (listView.SuccessFlag)
			{
				Content = new StackLayout
				{
					Children = { listView, allDeleteButton, shiftButton }
				};
			}
			else
			{
				Content = new StackLayout
				{
					Children = { new Label { Text = "読み込みに失敗しました", FontSize = 20, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand },
								 allDeleteButton,
								 shiftButton }
				};
			}
		}
	}

	// タスクの詳細画面を描画するページ
	public class TaskDetailPage : ContentPage
	{
		public List<string> ar = Enumerable.Range(1, 60).Select(n => string.Format("{0}分", n)).ToList();

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
			Content = new StackLayout { Children = { title, restTime, unitTime, term, remark } };
		}
	}
}