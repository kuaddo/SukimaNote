using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SukimaNote
{
	// タスクの提案がされるページ。ユーザーが考慮する要素を指定できる
	public class TopPage : BasicTaskShowPage
	{
		List<TaskData> orderedTaskList = new List<TaskData>();

		private Button next = new Button { Text = "NEXT" };
		private Button back = new Button { Text = "BACK" };
		private Label taskCountLabel = new Label { HorizontalOptions = LayoutOptions.Center };

		// 基本的には最大5件表示。TODO: 設定で変えられるようにする(1~10)。場所を移すかも
		private int maxShow = 5;
		public int MaxShow
		{
			get { return maxShow; }
			set
			{
				if (value >= 1 && value <= 10)
				{
					maxShow = value;
				}
			}
		}

		// TODO: TaskDetailのレイアウトを流用する。
		public TopPage(RootPage rootPage)
		{
			Title = "トップページ";

			// 登録されたタスクが0個の時。時間が余ったらレイアウトを凝ったものにする
			if (SharedData.taskList.Count == 0)
			{
				// TODO: 別のページに遷移してからでないと、タスクが更新されない
				var shiftButton = new Button
				{
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					Text = "タスクの追加",
					FontSize = 40,
				};
				shiftButton.Clicked += async (sender, e) =>
				{
					await Navigation.PushAsync(new TaskAddPage(rootPage, null));
				};
				Content = shiftButton;
				return;
			}

			// 初期化
			pickUpList();
			int taskCount = orderedTaskList.Count;  // タスクの最大ページ数
			int page = 1;                           // 現在のタスクのページ数
			shiftSetting(page, taskCount);          // NEXT BACKボタンの初期化
			Initialize(orderedTaskList[0]);

			// ツールバーアイテム
			ToolbarItems.Clear();
			var addTaskItem = new ToolbarItem
			{
				Text = "タスクの追加",
				Priority = 1,
				Icon = "plus.png",
				Order = ToolbarItemOrder.Primary
			};
			addTaskItem.Clicked += async (sender, e) =>
			{
				await Navigation.PushAsync(new TaskAddPage(rootPage, null));
			};

			var deleteTaskItem = new ToolbarItem
			{
				Text = "タスクの削除",
				Priority = 2,
				Icon = "x.png",
				Order = ToolbarItemOrder.Primary
			};
			deleteTaskItem.Clicked += async (sender, e) =>
			{
				if (await DisplayAlert("Caution", orderedTaskList[page - 1].Title + "を削除しますか?", "YES", "NO"))
				{
					// 現在開いているページのTaskDataをtaskListから取得
					var taskData = SharedData.taskList[SharedData.taskList.IndexOf(orderedTaskList[page - 1])];
					await SharedData.deleteTaskAsync(taskData);

					// トップページを再生成して画面を更新
					var menuData = new MenuData()
					{
						Title = "トップページ",
						TargetType = typeof(TopPage),
					};
					rootPage.NavigateTo(menuData);
				}
			};

			var editTaskItem = new ToolbarItem
			{
				Text = "タスクの編集",
				Priority = 2,
				Order = ToolbarItemOrder.Secondary
			};
			editTaskItem.Clicked += async (sender, e) =>
			{
				// 現在開いているページのTaskDataをtaskListから取得
				var taskData = SharedData.taskList[SharedData.taskList.IndexOf(orderedTaskList[page - 1])];
				await Navigation.PushAsync(new TaskAddPage(rootPage, taskData));
			};

			ToolbarItems.Add(addTaskItem);
			ToolbarItems.Add(deleteTaskItem);
			ToolbarItems.Add(editTaskItem);

			// タスクの表示切り替えのUI。
			next.Clicked += (sender, e) =>
			{
				page++;
				shiftSetting(page, taskCount);
				Initialize(orderedTaskList[page - 1]);
			};
			back.Clicked += (sender, e) =>
			{
				page--;
				shiftSetting(page, taskCount);
				Initialize(orderedTaskList[page - 1]);
			};
			var shift = new StackLayout
			{
				HorizontalOptions = LayoutOptions.Center,
				Children =
				{
					taskCountLabel,
					new StackLayout
					{
						Spacing = 40,
						HorizontalOptions = LayoutOptions.Center,
						Orientation = StackOrientation.Horizontal,
						Children = { back, next }
					}
				}
			};

			// Gridでページの2/3がタスクの表示に使えるように調整
			var grid = new Grid();

			grid.Children.Add(frame, 0, 1, 0, 7);
			grid.Children.Add(shift, 0, 1, 7, 10);

			Content = grid;
		}

		// shiftのボタンとラベルの設定
		private void shiftSetting(int page, int taskCount)
		{
			if (taskCount == 1)
			{
				next.IsEnabled = false;
				back.IsEnabled = false;
			}
			else if (page == 1)
			{
				next.IsEnabled = true;
				back.IsEnabled = false;
			}
			else if (page == taskCount)
			{
				next.IsEnabled = false;
				back.IsEnabled = true;
			}
			else
			{
				next.IsEnabled = true;
				back.IsEnabled = true;
			}
			taskCountLabel.Text = string.Format("{0}/{1}", page, taskCount);

			return;
		}

		// 引数で考慮する要素を受け取り、優先度を元にListを作るメソッド。タスクが一つでも存在するときに呼び出す
		private void pickUpList()
		{
			// TODO: 時間制限などの条件による有効なタスクを考慮してから、表示可能なタスクの数を数えるようにする
			int taskCount = SharedData.taskList.Count;
			int pickUpCount = MaxShow;
			
			if (taskCount < MaxShow) pickUpCount = taskCount;
			orderedTaskList = new List<TaskData>(SharedData.taskList
				.OrderBy(task => task.Deadline.Ticks)
                .GroupBy(task => task.DaysByDeadline)
                .OptimizeTaskData()
				.Take(pickUpCount));
		}       
    }
}