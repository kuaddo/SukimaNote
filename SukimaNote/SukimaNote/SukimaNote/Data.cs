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
	// タスクの設定項目のプロパティのクラス
	public class TaskData
	{
		// 最低限
		public string	Title		 { get; set; }	// タイトル
		public DateTime Deadline	 { get; set; }  // 期限
		public int		TimeToFinish { get; set; }  // 予想作業時間(インデックスで保存)	

		// 追加オプション
		public string	Place		 { get; set; }  // 場所
		public SharedData.priority		Priority	 { get; set; }  // 優先度(SharedDataのenumを使用)
		public int		Progress	 { get; set; }  // 進捗度(0~100の整数値)
		public string	Remark		 { get; set; }  // 備考
	}

	// アプリ内の様々な場所に使うTaskに関しての静的データ、メソッドのクラス
	public static class SharedData
	{
		// 予想作業時間で使う時間のリスト
		public static List<string> timeToFinishList = new List<string>
			{ "5分", "10分", "15分", "20分", "30分", "45分", "1時間", "1.5時間", "2時間", "2.5時間", "3時間", "4時間", "5時間", "6時間", "6時間以上"};
		// 場所で使うList
		public static ObservableCollection<string> placeList = new ObservableCollection<string>{ "指定無し" };
		// 優先度で使うenum
		public enum	priority { 低い, 普通, 高い };
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

						Remark = propertyArray[4],
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