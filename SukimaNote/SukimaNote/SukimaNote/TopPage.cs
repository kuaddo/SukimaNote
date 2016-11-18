using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using PCLStorage;

namespace SukimaNote
{
	// タスクの提案がされるページ。ユーザーが考慮する要素を指定できる
	public class TopPage : BasicTaskShowPage
	{
		List<TaskData> orderedTaskList = new List<TaskData>();
		RootPage rootPage;

		private Button next = new Button { Text = "NEXT", BackgroundColor = Color.FromHex("EDAC6B") };
		private Button back = new Button { Text = "BACK", BackgroundColor = Color.FromHex("EDAC6B") };
		private Label taskCountLabel = new Label { HorizontalOptions = LayoutOptions.Center };

		private int position = 0;   // 表示しているorderedTaskListのindex
		private int taskCount = 0;	// 表示可能なタスクの数

		// TODO: TaskDetailのレイアウトを流用する。
		public TopPage(RootPage rp)
		{
			Title = "トップページ";
			rootPage = rp;		// メソッドで利用可能にする。

			// 表示可能なタスクのリストを作る
			pickUpList();
			taskCount = orderedTaskList.Count;  // タスクの最大ページ数

			// 登録されたタスクが0個の時。時間が余ったらレイアウトを凝ったものにする
			if (taskCount == 0)
			{
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

			shiftSetting();						// NEXT BACKボタンの初期化
			Initialize(orderedTaskList[0]);     // frameの初期化
			setToolBarItem();					// ツールバーアイテムの初期化

			// タスクの表示切り替えのUI
			next.Clicked += (sender, e) =>
			{
				position++;
				shiftSetting();
				Initialize(orderedTaskList[position]);
			};
			back.Clicked += (sender, e) =>
			{
				position--;
				shiftSetting();
				Initialize(orderedTaskList[position]);
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

			// 進捗度の設定。表示可能なタスクの数の変動が起こらないため、ページの再描画は行わない
			pSave.Clicked += async (sender, e) =>
			{
				var taskData = orderedTaskList[position];
				taskData.Progress = (int)pSlider.Value;

				IFile updateFile = await SharedData.searchFileAsync(taskData);
				await updateFile.WriteAllTextAsync(SharedData.makeSaveString(taskData));

				// ページをめくれるように戻す
				shiftSetting();
				setPFrame.IsVisible = false;
			};

			// Gridでページの4/5がタスクの表示に使えるように調整
			var grid = new Grid();

			grid.Children.Add(frame, 0, 1, 0, 4);
			grid.Children.Add(shift, 0, 1, 4, 5);

			Content = grid;
		}

		// ツールバーアイテムを設定するメソッド
		private void setToolBarItem()
		{
			// タスクを追加するツールバーアイテム
			var addTaskItem = new ToolbarItem
			{
				Text = "タスクの追加",
				Priority = 1,
				Icon = "pencil.png",
				Order = ToolbarItemOrder.Primary
			};
			addTaskItem.Clicked += async (sender, e) =>
			{
				await Navigation.PushAsync(new TaskAddPage(rootPage, null));
			};

			// タスクを削除するツールバーアイテム
			var deleteTaskItem = new ToolbarItem
			{
				Text = "タスクの削除",
				Priority = 2,
				Icon = "garbageBox.png",
				Order = ToolbarItemOrder.Primary
			};
			deleteTaskItem.Clicked += async (sender, e) =>
			{
				if (await DisplayAlert("Caution", orderedTaskList[position].Title + "を削除しますか?", "YES", "NO"))
				{
					await SharedData.deleteTaskAsync(orderedTaskList[position]);
					regenerateTopPage();
				}
			};

			// 進捗度を設定するツールバーアイテム
			var setProgressItem = new ToolbarItem
			{
				Text = "進捗度の設定",
				Priority = 1,
				Order = ToolbarItemOrder.Secondary
			};
			setProgressItem.Clicked += (sender, e) =>
			{
				setPFrame.IsVisible = true;
				pSlider.Value = orderedTaskList[position].Progress;

				// ページをめくれないようにする
				next.IsEnabled = false;
				back.IsEnabled = false;
			};

			// タスクを完了させるツールバーアイテム
			var finishTaskItem = new ToolbarItem
			{
				Text = "タスクの完了",
				Priority = 2,
				Order = ToolbarItemOrder.Secondary
			};
			finishTaskItem.Clicked += async (sender, e) =>
			{
				var taskData = orderedTaskList[position];
				taskData.Closed = true;
				// ファイルも更新
				IFile updateFile = await SharedData.searchFileAsync(taskData);
				await updateFile.WriteAllTextAsync(SharedData.makeSaveString(taskData));
				regenerateTopPage();
			};

			// Taskを編集するツールバーアイテム
			var editTaskItem = new ToolbarItem
			{
				Text = "タスクの編集",
				Priority = 3,
				Order = ToolbarItemOrder.Secondary
			};
			editTaskItem.Clicked += async (sender, e) =>
			{
				await Navigation.PushAsync(new TaskAddPage(rootPage, orderedTaskList[position]));
			};

			ToolbarItems.Add(addTaskItem);
			ToolbarItems.Add(deleteTaskItem);
			ToolbarItems.Add(setProgressItem);
			ToolbarItems.Add(finishTaskItem);
			ToolbarItems.Add(editTaskItem);
		}
		// shiftのボタンとラベルの設定をするメソッド。タスクが1つ以上の場合でのみ使用される
		private void shiftSetting()
		{
			if (taskCount == 1)
			{
				next.IsEnabled = false;
				back.IsEnabled = false;
			}
			else if (position == 0)
			{
				next.IsEnabled = true;
				back.IsEnabled = false;
			}
			else if (position == taskCount - 1)
			{
				next.IsEnabled = false;
				back.IsEnabled = true;
			}
			else
			{
				next.IsEnabled = true;
				back.IsEnabled = true;
			}
			taskCountLabel.Text = string.Format("{0}/{1}", position + 1, taskCount);

			return;
		}
		// TopPageを再生成して、ページの再描画をするメソッド
		private void regenerateTopPage()
		{
			var menuData = new MenuData()
			{
				Title = "トップページ",
				TargetType = typeof(TopPage),
			};
			rootPage.NavigateTo(menuData);
		}
		// 引数で考慮する要素を受け取り、優先度を元にListを作るメソッド。タスクが一つでも存在するときに呼び出す
		private void pickUpList()
		{
            // 期限過ぎと完了済みのタスクの排除
            var selectedTaskList = SharedData.taskList
                                   .Where(task => task.MinutesByDeadline >= 0 && !task.Closed)
                                   .Select(task => task);

			// TopPageの要素として取り出す数
			int pickUpCount = Math.Min(selectedTaskList.Count(), SharedData.MaxShow);

			orderedTaskList = selectedTaskList
				              .OrderBy(task => task.Deadline.Ticks)
                              .GroupBy(task => task.DaysByDeadline)
                              .OptimizeTaskData()
				              .Take(pickUpCount)
                              .ToList();
		}       
    }
}