using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PCLStorage;

namespace SukimaNote
{
	// タスクの設定項目のプロパティのクラス
	public class TaskData
	{
		// 最低限
		public string	Title    { get; set; }	// タイトル
		public DateTime Term	 { get; set; }  // 期限
		public int		RestTime { get; set; }  // 予想残り時間(インデックスで保存)	

		// 追加オプション
		public int		UnitTime { get; set; }  // 最低単位作業時間(インデックスで保存)
		public string	Place	 { get; set; }  // 場所
		public int		Priority { get; set; }  // 優先度(SharedDataのenumを使用)
		public string	Remark	 { get; set; }  // 備考
	}

	// アプリ内の様々な場所に使うTaskに関しての静的データ、メソッドのクラス
	public static class SharedData
	{
		// RestTimeで使う時間のリスト
		public static List<string> restTimeList = new List<string>
			{ "5分", "10分", "15分", "20分", "30分", "45分", "1時間", "1.5時間", "2時間", "2.5時間", "3時間", "4時間", "5時間", "6時間", "6時間以上"};
		// RestTimeで使う時間のリスト
		public static List<string> unitTimeList = new List<string>
			{ "指定無し", "5分", "10分", "15分", "20分", "30分", "45分", "60分", "90分" };
		// 場所で使うList
		public static ObservableCollection<string> placeList;
		// 優先度で使うenum
		public enum			   priority						 {  low  , middle,  high   };
		public static string[] priorityString = new string[] { "低い", "普通", "高い" };

		// 静的に保持したタスクのリスト。ObservableCollectionを使うとAddした時に自動更新ができる
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
						Term = new DateTime(long.Parse(propertyArray[1])),
						RestTime = int.Parse(propertyArray[2]),
						UnitTime = int.Parse(propertyArray[3]),
						Remark = propertyArray[4],
					};
				}
			})).ConfigureAwait(false);

			taskList = new ObservableCollection<TaskData>(taskDataArray);
		}
	}
}