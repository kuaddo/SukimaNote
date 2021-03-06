﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
using PCLStorage;

namespace SukimaNote
{
    // タスクの情報一覧の実装すべきもの
    interface ITaskData
    {
        string   Title             { get; set; } // タスクのタイトル
        DateTime Deadline          { get; set; } // 期限
        int      TimeToFinish      { get; set; } // 予想作業時間(index)
        string   Place             { get; set; } // (index)
		int      Priority          { get; set; } // (index)
		int      Progress          { get; set; } // 進捗度（0 ~ 100%）
        string   Remark            { get; set; } // 備考
		bool	 Closed			   { get; set; } // タスクが終了済みかどうか

		int		 RestMinutes	   { get; }      // 進捗度を考慮した作業の残り時間
		int		 DaysByDeadline	   { get; }
		int		 HoursByDeadline   { get; }
		int		 MinutesByDeadline { get; }

		// TaskViewPageのCellに使用
		string	 DeadlineString    { get; }		 // フォーマット"F"の日付表示
		string	 ProgressString	   { get; }		 // '%'を末尾につけたProgress
		Color	 PriorityColor	   { get; }		 // Priorityに対応した色

		string   FileName		   { get; set; } // 保存されているファイル名。タスクの内容が変化しても変更されることはない
		int		 BeforeProgress	   { get; set; } // タスクを完了させる直前のProgressを保存。ファイルには残さない
	}

    // タスクの設定項目のプロパティのクラス
    public class TaskData : ITaskData, INotifyPropertyChanged
	{
        // 定数
        private const int MinProgress = 0, MaxProgress = 100;
        private const int MinPriority = 0, MaxPriority = 2;

		// プロパティに用いる変数。
		private string	 title		  = "";
		private DateTime deadline;
		private int		 timeToFinish = 1;
		private string	 place		  = "指定無し";
		private int		 priority	  = 1;
		private int		 progress	  = MinProgress;
		private string	 remark		  = "";				// stringのデフォルト値はnullになってしまうため
		private bool	 closed		  = false;

		// プロパティの変更時に呼び出される
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		// ITaskDataで指定されたプロパティ
		public string	Title
		{
			get { return title; }
			set { SetProperty(ref title, value); }
		}
		public DateTime Deadline
		{
			get { return deadline; }
			set
			{
				SetProperty(ref deadline, value);
				OnPropertyChanged(nameof(DaysByDeadline));
				OnPropertyChanged(nameof(HoursByDeadline));
				OnPropertyChanged(nameof(MinutesByDeadline));
				OnPropertyChanged(nameof(DeadlineString));
			}
		}
		public int		TimeToFinish
		{
			get { return timeToFinish; }
			set
			{
				SetProperty(ref timeToFinish, value);
				OnPropertyChanged(nameof(RestMinutes));
			}
		}
		public string	Place
		{
			get { return place; }
			set { SetProperty(ref place, value); }
		}
		public int		Priority
		{
			get { return priority; }
			set
			{
				if (value >= MinPriority && value <= MaxPriority)
				{
					SetProperty(ref priority, value);
					OnPropertyChanged(nameof(PriorityColor));
				}
			}
		}
		public int		Progress
		{
			get { return progress; }
			set
			{
				if (value >= MinProgress && value <= MaxProgress)
				{
					SetProperty(ref progress, value);
					OnPropertyChanged(nameof(RestMinutes));
					OnPropertyChanged(nameof(ProgressString));
				}
			}
		}
		public string	Remark
		{
			get { return remark; }
			set { SetProperty(ref remark, value); }
		}
		public bool		Closed
		{
			get { return closed; }
			set
			{
				SetProperty(ref closed, value);
			}
		}

		public int		RestMinutes		  => TimeToFinish * (MaxProgress - Progress) / 100;
		public int		DaysByDeadline	  => (int)new TimeSpan(Deadline.Ticks - DateTime.Now.Ticks).TotalDays;
		public int		HoursByDeadline	  => (int)new TimeSpan(Deadline.Ticks - DateTime.Now.Ticks).TotalHours;
		public int		MinutesByDeadline => (int)new TimeSpan(Deadline.Ticks - DateTime.Now.Ticks).TotalMinutes;

		public string	DeadlineString	  => Deadline.ToString("F");
		public string	ProgressString	  => Progress.ToString() + "%";
		public Color	PriorityColor { get {
				switch (Priority)
				{
					case 0 : return Color.Blue;
					case 1 : return Color.Yellow;
					case 2 : return Color.Red;
					default: return Color.White;
				}		 
			} }

		public string	FileName	   { get; set; }
		public int	    BeforeProgress { get; set; } = -1;

		// ジェネリックで全ての型に対応。refで呼び出し元に反映させる
		private void SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (!Equals(storage, value))
			{
				storage = value;
				OnPropertyChanged(propertyName);
			}
		}
	}

    // アプリ内の様々な場所に使うTaskに関しての静的データ、メソッドのクラス
    public static class SharedData
	{
		// 予想作業時間で使う時間のリスト
		public static List<string> timeToFinishList = new List<string>
			{ "5分", "10分", "15分", "20分", "30分", "45分", "1時間", "1.5時間", "2時間", "2.5時間", "3時間", "4時間", "5時間", "6時間", "6時間以上"};
		// 場所で使うList
		public static List<string> placeList = new List<string>{ "指定無し" };
		// 優先度で使うList
		public static List<string> priorityList = new List<string> { "低い", "普通", "高い" };
		// タスクのリスト。ObservableCollectionを使うとAddした時に自動更新ができる
		public static ObservableCollection<TaskData> taskList = new ObservableCollection<TaskData>();
		// タスクの保存件数
		public static int TaskCountLimit { get; set; } = 30;
		// トップページの表示件数
		public static int MaxShow { get; set; } = 5;
		// 通知をするかどうか
		public static bool IsNotify { get; set; } = true; 


		// taskListにファイルから読み込んだタスクのリストを反映させるメソッド。アプリスタート時に使用
		// ConfigureAwait(false)で移行の処理をワーカースレッドに行わせることで、デッドロックを回避
		// UIの処理はメインスレッドしかできないようなので、ここではビジネスロジックのみを実行
		// usingでリソースを開放するためにstreamを使用。あまりうまく行かなかったが、起動時に一度だけ読み込むためリソース不足にならない
		public async static Task MakeTaskDataListAsync()
		{
			TaskData taskData;

			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IFolder taskDataFolder = await rootFolder.CreateFolderAsync("taskDataFolder", CreationCollisionOption.OpenIfExists);    // 存在しなかったならば作成
			IList<IFile> files = await taskDataFolder.GetFilesAsync().ConfigureAwait(false);
			var taskDataArray = await Task.WhenAll(files.Select(async file => {
				using (Stream stream = await file.OpenAsync(FileAccess.Read))
				using (StreamReader sr = new StreamReader(stream))
				{
					string allText = sr.ReadToEnd();
					string[] propertyArray = allText.Split(':');
					taskData = new TaskData
					{
						Title		 = propertyArray[0],
						Deadline	 = new DateTime(long.Parse(propertyArray[1])),
						TimeToFinish = int.Parse(propertyArray[2]),
						Place		 = propertyArray[3],
						Priority	 = int.Parse(propertyArray[4]),
						Progress	 = int.Parse(propertyArray[5]),
						Remark		 = propertyArray[6],
						Closed		 = Convert.ToBoolean(propertyArray[7]),
						FileName	 = file.Name
					};
					return taskData;
				}
			})).ConfigureAwait(false);

			taskList = new ObservableCollection<TaskData>(taskDataArray);
		}
		// ファイルに保存する文字列を引数のTaskDataから生成する
		// 一つのファイルあたりの書き込み量が少ないので、部分的に変更せずにこのメソッドで全て変更する
		public static string makeSaveString(TaskData taskData)
		{
			return taskData.Title + ':' +
				   taskData.Deadline.Ticks.ToString() + ':' +
				   taskData.TimeToFinish.ToString() + ':' +
				   taskData.Place + ':' +
				   taskData.Priority.ToString() + ':' +
				   taskData.Progress.ToString() + ':' +
				   taskData.Remark + ':' +
				   taskData.Closed.ToString();
		}
		// TaskDataに対応するファイルを返す。見つからなかったならnull。ファイル名はタスク生成時から変更しない
		public static async Task<IFile> searchFileAsync(TaskData taskData)
		{
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IFolder taskDataFolder = await rootFolder.CreateFolderAsync("taskDataFolder", CreationCollisionOption.OpenIfExists);        // 存在しなかったならば作成
			return await taskDataFolder.GetFileAsync(taskData.FileName);
		}
		// 引数のTaskをtaskListとファイルから削除する
		public static async Task deleteTaskAsync(TaskData taskData)
		{
			var deleteFile = await searchFileAsync(taskData);
			await deleteFile.DeleteAsync();
			taskList.Remove(taskData);
		}
		// Properties Dictionaryから設定データを読み込む
		public static void makeSettingData()
		{
			// TaskCountLimitの取得
			if (Application.Current.Properties.ContainsKey("taskCountLimit"))
			{
				TaskCountLimit = (int)Application.Current.Properties["taskCountLimit"];
			}

			// placeListの取得
			if (Application.Current.Properties.ContainsKey("placeList"))
			{
				var saveText = Application.Current.Properties["placeList"] as string;
				placeList = new List<string>(saveText.Split(':'));
				while (placeList.IndexOf("") >= 0) { placeList.Remove(""); }	// 空白は削除
			}

			// MaxShowの取得
			if (Application.Current.Properties.ContainsKey("maxShow"))
			{
				MaxShow = (int)Application.Current.Properties["maxShow"];
			}

			// IsNotifyの取得
			if (Application.Current.Properties.ContainsKey("isNotify"))
			{
				IsNotify = (bool)Application.Current.Properties["isNotify"];
			}
		}
	}

	// サンプルのタスクリスト生成専用のクラス
	static class SampleClass
	{
		private const int MaxDateRange = 4;       // GenerateRandomTaskDateメソッドのswitch文のカバー範囲
		private const int MinTimeToFinish = 10;   // TimeToFinishの最小値
		private const int MaxTimeToFinish = 360;  // TimeToFinishの最大値
		private const int MaxPriorityRange = 3;   // Priorityの範囲の最大値
		private const int MaxProgressRange = 90;  // Progressの指定範囲の最大値

		// サンプルのタスクリスト生成メソッド、generateNumber数のサンプルを生成
		public static List<TaskData> GenerateSampleData(int generateNumber)
		{
			List<TaskData> sampleTaskDataList = new List<TaskData>();
			Random random = new Random();

			for (int i = 1; i <= generateNumber; i++)
			{
				sampleTaskDataList.Add
					(
						new TaskData
						{
							Title = "Task" + i,
							Deadline = GenerateRandomTaskDate(random.Next(MaxDateRange)),  // 後述メソッドのランダム日付付与 
							TimeToFinish = random.Next(MinTimeToFinish, MaxTimeToFinish),  // MinTimeToFinish ~ MaxTimeToFinish
							Priority = random.Next(MaxPriorityRange),                      // 0, 1, 2のいずれかを与える
							Progress = random.Next(MaxProgressRange),                      // 0 ~ (MaxProgressRange - 1)
						}
					);
			}

			return sampleTaskDataList;
		}

		// 乱数を使ってサンプルデータに日付を与えるメソッド
		private static DateTime GenerateRandomTaskDate(int randomNum)
		{
			DateTime today = DateTime.Now.AddHours(1);
			DateTime tomorrow = DateTime.Now.AddDays(1);
			DateTime dayAfterTomorrow = DateTime.Now.AddDays(2);   // 明後日の日付
			DateTime dayAfter2Days = DateTime.Now.AddDays(3);   // 明々後日の日付

			switch (randomNum)
			{
				case 0: return today;
				case 1: return tomorrow;
				case 2: return dayAfterTomorrow;
				case 3: return dayAfter2Days;
				default: return tomorrow;
			}
		}
	}
	// Linqの拡張メソッド用クラス
	public static class ExtendedMethodClass
    {
        // 評価関数メソッド
        public static IEnumerable<TaskData> OptimizeTaskData(this IEnumerable<IGrouping<int, TaskData>> taskList)
        {
            foreach (var tasks in taskList)
            {
                // 残り時間（日数）からのキー値と評価値をもとに昇順に並び替える
                var orderedTasks = tasks.OrderBy(task => task.RestMinutes / (float)Math.Pow(2, task.Priority + 1));
                foreach (var task in orderedTasks)
                {
                    // 一個ずつ返すことでIEnurableとの親和性を高めた
                    yield return task;
                }
            }
        }
    }
}