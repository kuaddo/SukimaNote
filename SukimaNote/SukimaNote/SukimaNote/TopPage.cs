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
		private int _maxShow = 5;
		public int MaxShow
		{
			get { return _maxShow; }
			set
			{
				if (value >= 1 && value <= 10)
				{
					_maxShow = value;
				}
			}
		}

		// TODO: TaskDetailのレイアウトを流用する。
		public TopPage()
		{
			Title = "トップページ";

			// 登録されたタスクが0個の時。時間が余ったらレイアウトを凝ったものにする
			if (SharedData.taskList.Count == 0)
			{
				Content = new Label
				{
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					Text = "NO TASK",
					FontSize = 50,
				};
				return;
			}

			// 初期化
			pickUpList();
			int taskCount = orderedTaskList.Count;	// タスクの最大ページ数
			int page = 1;							// 現在のタスクのページ数
			shiftSetting(page, taskCount);			// NEXT BACKボタンの初期化
			Initialize(orderedTaskList[0]);

		
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

			// タスクに含める情報が確定したらちゃんと作る
			var option = new StackLayout
			{
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				BackgroundColor = Color.Green,
				Children =
				{
					new StackLayout { Orientation = StackOrientation.Horizontal, Children = { new Switch { HorizontalOptions = LayoutOptions.CenterAndExpand}, new Switch { HorizontalOptions = LayoutOptions.CenterAndExpand } } },
					new StackLayout { Orientation = StackOrientation.Horizontal, Children = { new Switch { HorizontalOptions = LayoutOptions.CenterAndExpand}, new Switch { HorizontalOptions = LayoutOptions.CenterAndExpand } } },
					new StackLayout { Orientation = StackOrientation.Horizontal, Children = { new Switch { HorizontalOptions = LayoutOptions.CenterAndExpand}, new Switch { HorizontalOptions = LayoutOptions.CenterAndExpand } } }
				}
			};

			// Gridでページの2/3がタスクの表示に使えるように調整
			var grid = new Grid();

			grid.Children.Add(frame	, 0, 1, 0, 7);
			grid.Children.Add(shift , 0, 1, 7, 8);
			grid.Children.Add(option, 0, 1, 8, 10);
	
			

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
				.OrderBy(task => CalculationOfPriority(task))
				.Take(pickUpCount));
		}

		// 優先度を計算するメソッド。優先度が大きいタスクほど数値が大きくなるようにする
		// けんたが書いたアルゴリズムをそのままここに入れる
		private long CalculationOfPriority(TaskData taskData)
		{
			return taskData.Term.Ticks;
		}
	}
}