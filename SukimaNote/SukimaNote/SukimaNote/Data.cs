using System;
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
        int      Place             { get; set; } // (index)
		int      Priority          { get; set; } // (index)
		int      Progress          { get; set; } // 進捗度（0 ~ 100%）
        string   Remark            { get; set; } // 備考
		bool	 Closed			   { get; set; } // タスクが終了済みかどうか

		int		 RestMinutes	   { get; }      // 進捗度を考慮した作業の残り時間
		int		 DaysByDeadline	   { get; }
		int		 HoursByDeadline   { get; }
		int		 MinutesByDeadline { get; }

		// TaskViewPageのCellに使用
		
	}

    // タスクの設定項目のプロパティのクラス
    public class TaskData : ITaskData, INotifyPropertyChanged
	{
        // 定数
        private const int MaxProgress = 100;
        private const int MinProgress = 0;

		// プロパティに用いる変数。
		private string	 title		  = "Task";
		private DateTime deadline;
		private int		 timeToFinish = 1;
		private int		 place		  = 0;
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
				OnPropertyChanged(nameof(HoursByDeadline));
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
		public int		Place
		{
			get { return place; }
			set { SetProperty(ref place, value); }
		}
		public int		Priority
		{
			get { return priority; }
			set { SetProperty(ref priority, value); }
		}
		public int		Progress
		{
			get { return progress; }
			set
			{
				SetProperty(ref progress, value);
				OnPropertyChanged(nameof(RestMinutes));
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
			set { SetProperty(ref closed, value); }
		}

		public int		RestMinutes		  => TimeToFinish * (MaxProgress - Progress) / 100;
		public int		DaysByDeadline	  => (int)new TimeSpan(Deadline.Ticks - DateTime.Now.Ticks).TotalDays;
		public int		HoursByDeadline	  => (int)new TimeSpan(Deadline.Ticks - DateTime.Now.Ticks).TotalHours;
		public int		MinutesByDeadline => (int)new TimeSpan(Deadline.Ticks - DateTime.Now.Ticks).TotalMinutes;

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

	// 評価得点に対するフラグの列挙
	public enum TaskDataFlags
    {
        // 残り時間表示順序の昇降フラグ（フラグがたっていると昇順/そうでなければ降順）
        RestTimeOrderByAscending = 1,

		// 進捗度考慮フラグ（フラグがたっていれば進捗度 0 のものに倍率を掛ける）  
		Progress = 2,
    }

	// ユーザの入力・選択オプションでの実装すべきもの
	interface IUserOption
    {
        // TaskDataに受け渡す項目
        TaskDataFlags FlagsList       { get; set; }　// TaskDataに受け渡すフラグ
        int           CurrentFreeTime { get; set; }  // ユーザの今使える時間
        int           DesignatedPlace { get; set; }  // ユーザの指定場所：デフォルトでは"指定なし"
    }

    // ユーザの入力・選択オプション
    public class UserOption : IUserOption
    {
        // IUserOptionで指定されたプロパティ
        public TaskDataFlags FlagsList       { get; set; } = TaskDataFlags.RestTimeOrderByAscending | TaskDataFlags.Progress;
		public int           CurrentFreeTime { get; set; }
        public int           DesignatedPlace { get; set; } = 0;
	}

    // アプリ内の様々な場所に使うTaskに関しての静的データ、メソッドのクラス
    public static class SharedData
	{
		// 予想作業時間で使う時間のリスト
		public static List<string> timeToFinishList = new List<string>
			{ "5分", "10分", "15分", "20分", "30分", "45分", "1時間", "1.5時間", "2時間", "2.5時間", "3時間", "4時間", "5時間", "6時間", "6時間以上"};
		// 場所で使うList
		public static ObservableCollection<string> placeList = new ObservableCollection<string>{ "指定無し" };
		// 優先度で使うList
		public static List<string> priorityList = new List<string> { "低い", "普通", "高い" };
		// タスクのリスト。ObservableCollectionを使うとAddした時に自動更新ができる
		public static ObservableCollection<TaskData> taskList = new ObservableCollection<TaskData>();


		// taskListにファイルから読み込んだタスクのリストを反映させるメソッド。アプリスタート時に使用
		// ConfigureAwait(false)で移行の処理をワーカースレッドに行わせることで、デッドロックを回避
		// UIの処理はメインスレッドしかできないようなので、ここではビジネスロジックのみを実行
		// usingでリソースを開放するためにstreamを使用。あまりうまく行かなかったが、起動時に一度だけ読み込むためリソース不足にならない
		public async static Task MakeTaskDataListAsync()
		{
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IFolder taskDataFolder = await rootFolder.CreateFolderAsync("taskDataFolder", CreationCollisionOption.OpenIfExists);    // 存在しなかったならば作成
			IList<IFile> files = await taskDataFolder.GetFilesAsync().ConfigureAwait(false);
			var taskDataArray = await Task.WhenAll(files.Select(async file => {
				using (Stream stream = await file.OpenAsync(FileAccess.Read))
				using (StreamReader sr = new StreamReader(stream))
				{
					string allText = sr.ReadToEnd();
					string[] propertyArray = allText.Split(':');
					return new TaskData
					{
						Title		 = propertyArray[0],
						Deadline	 = new DateTime(long.Parse(propertyArray[1])),
						TimeToFinish = int.Parse(propertyArray[2]),
						Place		 = int.Parse(propertyArray[3]),
						Priority	 = int.Parse(propertyArray[4]),
						Progress	 = int.Parse(propertyArray[5]),
						Remark		 = propertyArray[6],
						Closed		 = Convert.ToBoolean(propertyArray[7])
					};
				}
			})).ConfigureAwait(false);

			taskList = new ObservableCollection<TaskData>(taskDataArray);
		}

		// Properties Dictionaryから場所の設定データを読み込む
		public static void MakePlaceList()
		{
			if (Application.Current.Properties.ContainsKey("PlaceList"))
			{
				placeList = Application.Current.Properties["PlaceList"] as ObservableCollection<string>;
			}
		}
	}
}