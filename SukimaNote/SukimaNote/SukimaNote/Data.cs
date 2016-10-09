using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using PCLStorage;

namespace SukimaNote
{
    // タスクの情報一覧の実装すべきもの
    interface ITaskData
    {
        string   Title           { get; set; }
        DateTime Deadline        { get; set; } // 期限
        int      TimeToFinish    { get; set; } // 予想作業時間（min）
        int      Place           { get; set; }
        int      Priority        { get; set; }
        int      Progress        { get; set; } // 進捗度（0 ~ 100%）
        int      RestMinutes     { get; }      // 作業の残り時間
        int      HoursByDeadline { get; }      // このタスクの期限までの時間
        string   Remark          { get; set; } // 備考
    }

    // タスクの設定項目のプロパティのクラス
    public class TaskData : ITaskData
    {
        // 定数
        private const int MaxProgress = 100;
        private const int MinProgress = 0;

        // 変数のデフォルトの値一覧
        private int    place = int.Parse(SharedData.placeList[0]);
        private int    priority = int.Parse(SharedData.priorityList[0]);
        private int    progress = MinProgress;
        private string remark = "特になし";

        // ITaskDataで指定されたプロパティ
        public string   Title { get; set; }
        public DateTime Deadline { get; set; }
        public int      TimeToFinish { get; set; }
        public int      Place { get { return place; } set { place = value; } }
        public int      Priority { get { return priority; } set { priority = value; } }
        public int Progress
        {
            get { return progress; }
            set
            {
                if (value > MinProgress && value <= MaxProgress)
                {
                    progress = value;
                }
                else
                {
                    progress = 0;
                }
            }
        }
        public int RestMinutes { get { return TimeToFinish * (MaxProgress - Progress) / 100; } }
        public int HoursByDeadline
        {
            get
            {
                // 時間計測後、単位を合わせた値を返す
                int hoursByDeadLine = (int)((Deadline.Ticks - DateTime.Now.Ticks) / (1000 * 1000 * 36));
                return hoursByDeadLine / 1000;
            }
        }
        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }
    }
    
    // 評価得点に対するフラグの列挙
    public enum TaskDataFlags
    {
        // 残り時間表示順序の昇降フラグ（フラグがたっていると昇順/そうでなければ降順）
        RestTimeOrderByAscending = 1,

        // 進捗度考慮フラグ（フラグがたっていれば進捗度 0 のものに倍率を掛ける）  
        Progress = 2
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
        // 変数のデフォルト値
        private int place = int.Parse(SharedData.placeList[0]);

        // IUserOptionで指定されたプロパティ
        public TaskDataFlags FlagsList       { get; set; }
        public int           CurrentFreeTime { get; set; }
        public int           DesignatedPlace { get { return place; } set { place = value; } }
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
						Deadline = new DateTime(long.Parse(propertyArray[1])),
						TimeToFinish = int.Parse(propertyArray[2]),
						Place = int.Parse(propertyArray[3]),
						Priority = int.Parse(propertyArray[4]),
						Progress = int.Parse(propertyArray[5]),
						Remark = propertyArray[6],
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