using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using Xamarin.Forms;
using PCLStorage;
using System.Threading.Tasks;

namespace SukimaNote
{
	// TaskListViewのCell
	public class TaskListViewCell : ViewCell
	{
		public const int fontSize = 25;

		public TaskListViewCell(TaskListPage taskListPage)
		{
			var checkBox	  = new CheckBoxImage { IsClosed = true };
			var title		  = new Label { FontFamily = "syunkasyuutouBB.ttf", TextColor = Color.Black, FontSize = fontSize + 5 };
			var deadline	  = new Label { FontFamily = "syunkasyuutouBB.ttf", TextColor = Color.Black, FontSize = fontSize - 10 };
			var progress	  = new Label { FontFamily = "syunkasyuutouBB.ttf", TextColor = Color.Black, FontSize = fontSize + 10, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
			var postItView    = new PostItView { Color = Color.Red, ShadowSize = 7};
			var checkTGR	  = new TapGestureRecognizer();

			// バインディングでTaskDataの一部を取得するためだけにあるLabel。配置しないと使えないので見えなくしている
			var PLabel = new BLabel { IsVisible = false };
			var BPLabel = new BLabel { IsVisible = false };
			var FNLabel = new BLabel { IsVisible = false };
			checkTGR.Tapped += async (sender, e) =>
			{
				if (checkBox.IsClosed == true)
					if (!(await taskListPage.DisplayAlert("Caution", "タスクを未完了に戻しますか?", "YES", "NO")))
						return;

				checkBox.IsClosed = !checkBox.IsClosed;
				IFile updateFile = await SharedData.searchFileAsync(new TaskData { FileName = FNLabel.String });
				var text = await updateFile.ReadAllTextAsync();
				string[] propertyArray = text.Split(':');
				if (checkBox.IsClosed)
				{
					BPLabel.Int = PLabel.Int;   // BeforeProgressに値を退避
					PLabel.Int = 100;
				}
				else
				{
					if (BPLabel.Int >= 0 && BPLabel.Int <= 100)
						PLabel.Int = BPLabel.Int;
					else
						PLabel.Int = 99;
				}
				propertyArray[5] = PLabel.Int.ToString();
				propertyArray[7] = checkBox.IsClosed.ToString();
				string newText = "";
				for(int i = 0; i < propertyArray.Count() - 1; i++) { newText += propertyArray[i] + ":"; } newText += propertyArray[propertyArray.Count() - 1];
				await updateFile.WriteAllTextAsync(newText);
			};
			checkBox.GestureRecognizers.Add(checkTGR);

			var actionDelete = new MenuItem
			{
				Text = "Delete",
				IsDestructive = true,	// iOSでメニューアイテムを赤色にする
			};
			actionDelete.Clicked += async (sender, e) =>
			{
				var taskData = (sender as MenuItem).CommandParameter as TaskData;
				if (await taskListPage.DisplayAlert("Caution", taskData.Title + "を削除しますか?", "YES", "NO"))
					await SharedData.deleteTaskAsync(taskData);
			};

			// ViewとTaskDataのバインディング
			checkBox	 .SetBinding(CheckBoxImage.IsClosedProperty, nameof(TaskData.Closed), BindingMode.TwoWay);
			title		 .SetBinding(Label.TextProperty,			 nameof(TaskData.Title));
			deadline	 .SetBinding(Label.TextProperty,			 nameof(TaskData.DeadlineString));
			progress	 .SetBinding(Label.TextProperty,			 nameof(TaskData.ProgressString));
			postItView   .SetBinding(PostItView.ColorProperty,		 nameof(TaskData.PriorityColor));
			PLabel.SetBinding(BLabel.IntProperty, nameof(TaskData.Progress), BindingMode.TwoWay);
			BPLabel.SetBinding(BLabel.IntProperty, nameof(TaskData.BeforeProgress), BindingMode.TwoWay);
			FNLabel.SetBinding(BLabel.StringProperty,			 nameof(TaskData.FileName), BindingMode.TwoWay);

			// コンテキストアクションに追加
			actionDelete.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));
			ContextActions.Add(actionDelete);

			// レイアウト
			var sl = new StackLayout { Children = { title, deadline }, Spacing = 0 };
			var view = new Grid();  // 列0~25 行0~5
			view.Children.Add(postItView,   0, 28,  0, 5);
			view.Children.Add(checkBox,		 1,  4,  1, 4);
			view.Children.Add(sl,			 4,  20, 0, 5);
			view.Children.Add(progress,		 20, 28, 0, 5);
			// 配置しないと使えないみたいなので、見えないけど端の方に設置
			view.Children.Add(FNLabel, 27, 28, 4, 5);
			view.Children.Add(PLabel, 27, 28, 4, 5);
			view.Children.Add(BPLabel, 27, 28, 4, 5);

			View = view;
		}
	}

	// タスクの一覧を描画するページ
	public class TaskListPage : ContentPage
	{
		public TaskListPage(RootPage rootPage)
		{
			Title = "タスク一覧";
			BackgroundColor = Color.FromHex(MyColor.BackgroundColor);

			var listView = new ListView
			{
				ItemsSource = SharedData.taskList,
				ItemTemplate = new DataTemplate(() => new TaskListViewCell(this)),
				SeparatorVisibility = SeparatorVisibility.None,
				RowHeight = (int)(TaskListViewCell.fontSize * 2.4)
			};

			// 詳細ページに移行
			listView.ItemSelected += (sender, e) =>
			{
				if (e.SelectedItem != null)
				{
					var newPage = new TaskDetailPage(e.SelectedItem as TaskData);
					listView.SelectedItem = null;	// nullにすることで同じアイテムを連続選択できる
					Navigation.PushAsync(newPage);
				}
			};

			// TaskAddPageへ遷移するツールバーアイテム
			var addTaskItem = new ToolbarItem
			{
				Text = "タスクの追加",
				Icon = "pencil.png",
			};
			addTaskItem.Clicked += async (sender, e) =>
			{
				await Navigation.PushAsync(new TaskAddPage(null, null));
			};
			// taskListをソートするツールバーアイテム
			var sortTaskList = new ToolbarItem
			{
				Text = "タスクのソート",
				Priority = 1,
				Order = ToolbarItemOrder.Secondary
			};
			sortTaskList.Clicked += async (sender, e) =>
			{
				var element = await DisplayActionSheet( "タスクのソート", "キャンセル", "", new string[] { "タイトル", "期限", "優先度", "進捗度" });
				if (element == null || element == "キャンセル")
					return;
				var order = await DisplayActionSheet("並び方の選択", "キャンセル", "", new string[] { "昇順", "降順" });
				if (order == null || order == "キャンセル")
					return;

				sort(element, order);
				// TaskListPageを再生成して画面を更新
				var menuData = new MenuData()
				{
					Title = "タスク一覧",
					TargetType = typeof(TaskListPage),
				};
				rootPage.NavigateTo(menuData);
			};
			// 完了済みを削除するツールバーアイテム
			var finishedDeleteItem = new ToolbarItem
			{
				Text = "完了済みを削除",
				Priority = 2,
				Order = ToolbarItemOrder.Secondary
			};
			finishedDeleteItem.Clicked += async (sender, e) =>
			{
				if (SharedData.taskList.Where(task => task.Closed).Count() == 0)
				{
					await DisplayAlert("Error", "完了済みのタスクが存在しません", "OK");
				}
				else if (await DisplayAlert("Caution", "完了済みのタスクを全て削除しますか?", "YES", "NO"))
				{
					TaskData deleteTask;
					while (SharedData.taskList.Where(task => task.Closed).Count() > 0)
					{
						deleteTask = SharedData.taskList.First(task => task.Closed);
						await SharedData.deleteTaskAsync(deleteTask);
					}
					//deleteTaskList.Select(async task => await SharedData.deleteTaskAsync(task));
					//await DisplayAlert("Deleted", deleteTaskList.ToArray()[0].Title, "OK");
					await DisplayAlert("Deleted", "削除しました", "OK");
				}
			};
			// タスクを全削除するツールバーアイテム
			var allDeleteItem = new ToolbarItem
			{
				Text = "タスクの全削除",
				Priority = 3,
				Order = ToolbarItemOrder.Secondary
			};
			allDeleteItem.Clicked += async (sender, e) =>
			{
				if (SharedData.taskList.Count == 0)
				{
					await DisplayAlert("Error", "タスクが存在しません", "OK");
				}
				else if (await DisplayAlert("Caution", "タスクを全て削除しますか?", "YES", "NO"))
				{
					if (SharedData.taskList.Count >= 10)
						if (!(await DisplayAlert("Caution", "タスクが" + SharedData.taskList.Count + "個あります。本当に全て削除しますか?", "YES", "NO")))
							return;
					await deleteAllTaskAsync();
					await DisplayAlert("Deleted", "削除しました", "OK");
				}
			};

			ToolbarItems.Add(addTaskItem);
			ToolbarItems.Add(sortTaskList);
			ToolbarItems.Add(finishedDeleteItem);
			ToolbarItems.Add(allDeleteItem);

			Content = new StackLayout
			{
				Children = { listView }
			};
		}

		// タスクの全削除
		private async Task deleteAllTaskAsync()
		{
			// ローカルストレージ上の削除
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IFolder taskDataFolder = await rootFolder.CreateFolderAsync("taskDataFolder", CreationCollisionOption.OpenIfExists);    // 存在しなかったならば作成
			IList<IFile> deletefiles = await taskDataFolder.GetFilesAsync();
			await Task.WhenAll(deletefiles.Select(async file => await file.DeleteAsync()));
			// 現在読み込まれているリストからの削除
			SharedData.taskList.Clear();
		}

		// TaskListをソート。非常に汚いが、解決に時間がかかりそうなので放置
		private void sort (string element, string order)
		{
			IOrderedEnumerable<TaskData> sortedTaskList = null;

			switch (element)
			{
				case "タイトル":
					if (order == "昇順")
						sortedTaskList = SharedData.taskList.OrderBy(task => task.Title);
					else
						sortedTaskList = SharedData.taskList.OrderByDescending(task => task.Title);
					break;
				case "期限":
					if (order == "昇順")
						sortedTaskList = SharedData.taskList.OrderBy(task => task.Deadline.Ticks);
					else
						sortedTaskList = SharedData.taskList.OrderByDescending(task => task.Deadline.Ticks);
					break;
				case "優先度":
					if (order == "昇順")
						sortedTaskList = SharedData.taskList.OrderBy(task => task.Priority);
					else
						sortedTaskList = SharedData.taskList.OrderByDescending(task => task.Priority);
					break;
				case "進捗度":
					if (order == "昇順")
					{
						sortedTaskList = SharedData.taskList.OrderBy(task => task.Progress);
					}
					else
						sortedTaskList = SharedData.taskList.OrderByDescending(task => task.Progress);
					break;
			}
			SharedData.taskList = new ObservableCollection<TaskData>(sortedTaskList);
		}
	}

	// タスクの詳細画面を描画するページ
	public class TaskDetailPage : BasicTaskShowPage
	{
		TaskData taskData;

		public TaskDetailPage(TaskData td)
		{
			taskData = td;

			// Taskを削除するツールバーアイテム
			var deleteTaskItem = new ToolbarItem
			{
				Text = "タスクの削除",
				Icon = "garbageBox.png",
			};
			deleteTaskItem.Clicked += async (sender, e) =>
			{

				if (await DisplayAlert("Caution", taskData.Title + "を削除しますか?", "YES", "NO"))
				{
					await SharedData.deleteTaskAsync(taskData);
					await Navigation.PopAsync();
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
				if (taskData.Closed)
				{
					DisplayAlert("Caution", "完了済みのタスクです", "OK");
				}
				else
				{
					setPFrame.IsVisible = true;
					taskData.BeforeProgress = taskData.Progress;
					pSlider.Value = taskData.Progress;
				}
			};
			// Taskを編集するツールバーアイテム
			var editTaskItem = new ToolbarItem
			{
				Text = "タスクの編集",
				Priority = 2,
				Order = ToolbarItemOrder.Secondary
			};
			editTaskItem.Clicked += async (sender, e) =>
			{
				if (taskData.Closed)
				{
					await DisplayAlert("Caution", "完了済みのタスクです", "OK");
				}
				else
				{
					await Navigation.PushAsync(new TaskAddPage(this, taskData));
				}
			};

			ToolbarItems.Add(deleteTaskItem);
			ToolbarItems.Add(setProgressItem);
			ToolbarItems.Add(editTaskItem);

			Content = makeContent();
		}

		public Grid makeContent()
		{
			Title = taskData.Title;
			Initialize(taskData);

			pSave.Clicked += async (sender, e) =>
			{
				if ((int)pSlider.Value == 100)
				{
					if (await DisplayAlert("Finished", "タスクを完了済みにしますか?", "YES", "NO"))
						taskData.Closed = true;
					else
						pSlider.Value = 99;
				}
				taskData.Progress = (int)pSlider.Value;

				IFile updateFile = await SharedData.searchFileAsync(taskData);
				await updateFile.WriteAllTextAsync(SharedData.makeSaveString(taskData));

				setPFrame.IsVisible = false;
			};

			var grid = new Grid();
			for (int i = 0; i < 5; i++)
				grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10, GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) });
			grid.Children.Add(frame, 0, 1, 0, 4);	
	
			return grid;
		}
	}

	// TopPage DetailPageでのレイアウトの基本となるページ
	public class BasicTaskShowPage : ContentPage
	{
		private const int descriptionFontSize = 15;

		protected Label title		 = new Label { TextColor = Color.Black, FontFamily = "syunkasyuutouBB.ttf", FontSize = descriptionFontSize + 13,
			HorizontalOptions = LayoutOptions.Start,		   VerticalOptions = LayoutOptions.CenterAndExpand };
		protected Label restTime	 = new Label { TextColor = Color.Black, /*FontFamily = "syunkasyuutouBB.ttf",*/ FontSize = descriptionFontSize + 5,
			HorizontalOptions = LayoutOptions.Center,		   VerticalOptions = LayoutOptions.Center };
		protected Label deadline	 = new Label { TextColor = Color.Black, FontFamily = "syunkasyuutouBB.ttf", FontSize = descriptionFontSize,
			HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
		protected Label timeToFinish = new Label { TextColor = Color.Black, FontFamily = "syunkasyuutouBB.ttf", FontSize = descriptionFontSize + 5,
			HorizontalOptions = LayoutOptions.Center,		   VerticalOptions = LayoutOptions.Center };
		protected Label place		 = new Label { TextColor = Color.Black, FontFamily = "syunkasyuutouBB.ttf", FontSize = descriptionFontSize + 5,
			HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
		protected Label priority	 = new Label { TextColor = Color.Black, FontFamily = "syunkasyuutouBB.ttf", FontSize = descriptionFontSize + 5,
			HorizontalOptions = LayoutOptions.CenterAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
		protected Label progress	 = new Label { TextColor = Color.Black, FontFamily = "syunkasyuutouBB.ttf", FontSize = descriptionFontSize * 2,
			HorizontalOptions = LayoutOptions.Fill,			   VerticalOptions = LayoutOptions.Fill,
			HorizontalTextAlignment = TextAlignment.Center,	   VerticalTextAlignment = TextAlignment.Center };
		protected Label remark		 = new Label { FontFamily = "syunkasyuutouBB.ttf", FontSize = descriptionFontSize, TextColor = Color.Black };
		protected Slider pSlider	 = new Slider { Maximum = 100, Minimum = 0, HorizontalOptions = LayoutOptions.FillAndExpand };
		protected Button pSave		 = new Button { Text = "save" };	// セーブの処理は各ページで記述
		protected Frame setPFrame    = new Frame { OutlineColor = Color.Silver, HasShadow = true };
		protected Frame frame		 = new Frame { HasShadow = true, Padding = new Thickness(5, 5, 5, 5) };

		// 背景色で内側の円を消しているのでColorは必須
		protected RoundProgressBar roundProgressBar = new RoundProgressBar { Color = Color.White, StrokeColor = Color.FromHex(MyColor.RoundProgressBarColor), StrokeWidth = 0.7f, WidthRequest = 90, HeightRequest = 90};

		// コンストラクタ
		public BasicTaskShowPage()
		{
			BackgroundColor = Color.FromHex(MyColor.BackgroundColor);
			makeContent();
		}

		// Labelの初期化をする
		protected void Initialize(TaskData taskData)
		{
			title.Text		  = taskData.Title;
			if		(taskData.Closed == true)		   { restTime.Text = "終了" + Environment.NewLine + "済み"; }
			else if (taskData.Deadline < DateTime.Now) { restTime.Text = "期限" + Environment.NewLine + "超過"; }
			else if (taskData.MinutesByDeadline < 60)  { restTime.Text = "残り" + Environment.NewLine + taskData.MinutesByDeadline + "分"; }
			else if (taskData.HoursByDeadline < 24)	   { restTime.Text = "残り" + Environment.NewLine + taskData.HoursByDeadline   + "時間"; }
			else									   { restTime.Text = "残り" + Environment.NewLine + taskData.DaysByDeadline    + "日"; }
			deadline.Text	  = taskData.Deadline.ToString("F");
			timeToFinish.Text = SharedData.timeToFinishList[taskData.TimeToFinish];
			place.Text		  = taskData.Place;
			priority.Text	  = SharedData.priorityList[taskData.Priority];
			progress.Text	  = taskData.Progress.ToString() + "%";
			remark.Text		  = taskData.Remark;

			roundProgressBar.Angle = taskData.Progress;
		}

		// Contentの作成
		private void makeContent()
		{
			const int rowCount = 13, columnCount = 20;
			var grid = new Grid { RowSpacing = 0, ColumnSpacing = 0 };
			for (int i = 0; i < rowCount; i++)
				grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(10, GridUnitType.Star) });
			for (int i = 0; i < columnCount; i++)
				grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) });

			// ノートの描画
			grid.Children.Add(new NoteBoxView { Color = Color.FromHex(MyColor.BackgroundColor), StrokeColor = Color.Gray, StrokeWidth = 3, EdgeSpaceRatio = 0.1, Row = rowCount }, 0, 20, 0, 13);

			// タイトル
			grid.Children.Add(new ContentView { Padding = new Thickness(0, 5, 0, 5), Content = new PostItView { Color = Color.Blue, ShadowSize = 5} }, 1, 15, 0, 3);
			grid.Children.Add(new ContentView { Padding = new Thickness(15, 7, 0, 7), Content = title }, 1, 15, 0, 3);

			// 残り時間
			grid.Children.Add(new ContentView { Padding = new Thickness(0, 5, 0, 0), Content = new BicoloredBoxView { LeftColor = Color.FromHex(MyColor.ButtonColor), ShadowSize = 6, Ratio = 100 } }, 15, 19, 0, 2);
			grid.Children.Add(restTime, 15, 19, 0, 2);

			// 期限
			grid.Children.Add(makeShadowGrid(Color.FromHex(MyColor.ButtonColor), "期限"), 1, 4, 3, 4);
			grid.Children.Add(deadline, 1, 12, 4, 5);

			// 場所
			grid.Children.Add(makeShadowGrid(Color.FromHex(MyColor.ButtonColor), "場所"), 1, 4, 5, 6);
			grid.Children.Add(place, 1, 12, 6, 7);

			// 優先度
			grid.Children.Add(makeShadowGrid(Color.FromHex(MyColor.ButtonColor), "優先度"), 1, 5, 7, 8);
			grid.Children.Add(priority, 1, 12, 8, 9);

			// 進捗度
			grid.Children.Add(new ContentView { Padding = new Thickness(0, 7, 0, 7), Content = new PostItView { Color = Color.Pink, ShadowSize = 5 }}, 12, 19, 3, 7);
			grid.Children.Add(new ContentView { Padding = new Thickness(3, 0, 0, 0), Content = new Grid { Children = { roundProgressBar, progress } } }, 12, 18, 3, 7);

			// 予想作業時間
			grid.Children.Add(makeShadowGrid(Color.FromHex(MyColor.ButtonColor), "作業時間"), 12, 17, 7, 8);
			grid.Children.Add(timeToFinish, 12, 19, 8, 9);

			// 備考
			grid.Children.Add(new ContentView { Padding = new Thickness(0, 0, 0, 10), Content = new PostItView { Color = Color.Green, ShadowSize = 5 } }, 1, 19, 9, 13);
			grid.Children.Add(new ScrollView { Content = remark } , 3, 19, 10, 12);
			grid.Children.Add(makeShadowGrid(Color.FromHex(MyColor.ButtonColor), "備考"), 1, 4, 9, 10);

			// 進捗度設定の際に表示する
			pSlider.ValueChanged += (sender, e) =>
			{
				progress.Text = ((int)pSlider.Value).ToString() + "%";
				roundProgressBar.Angle = (int)pSlider.Value;
			};
			setPFrame = new Frame
			{
				BackgroundColor = Color.Black.MultiplyAlpha(0.7d), // 透過
				IsVisible = false,
				Content = new StackLayout
				{
					VerticalOptions = LayoutOptions.Center,
					Children =
					{
						new Label { Text = "進捗度を設定してください", TextColor = Color.White, HorizontalOptions = LayoutOptions.Center },
						new StackLayout { Orientation = StackOrientation.Horizontal, Children = { pSlider, pSave} }
					}
				}
			};
			grid.Children.Add(setPFrame, 2, 18, 7, 12);

			frame.Content = grid;

			Content = frame;
		}

		// 要素の説明に使う影付きのgrid
		private Grid makeShadowGrid(Color color, string str)
		{
			var bBoxView = new BicoloredBoxView { LeftColor = color, ShadowSize = 6, Ratio = 100, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
			var label = new Label { Text = str, TextColor = Color.Black, FontSize = descriptionFontSize, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center };

			var grid = new Grid();
			grid.Children.Add(bBoxView);
			grid.Children.Add(label);

			return grid;
		}
	}
}