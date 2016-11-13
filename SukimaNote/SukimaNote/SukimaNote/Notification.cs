using System;
using PCLStorage;
using Xamarin.Forms;



namespace SukimaNote
{
	public class Notification
	{
		public async void OnStartAsync()
		{

			int[,] table = new int[7, 24];
			// appのルートフォルダを取得
			IFolder rootFolder = FileSystem.Current.LocalStorage;

			// ルート直下にサブフォルダを作成
			IFolder tableFolder = await rootFolder.CreateFolderAsync("table", CreationCollisionOption.OpenIfExists);

			// table.txtが存在するかを確認する
			ExistenceCheckResult res = await tableFolder.CheckExistsAsync("table.txt");

			if (res == ExistenceCheckResult.FileExists)
			{
				// table.txtが存在する場合
				// table.txtを読み込んで、配列に格納する
				// まず文字列変数に文字を格納してから、値の取り出しを行おうかなー
				IFile readFile = await tableFolder.GetFileAsync("table.txt");

				string readStr = await readFile.ReadAllTextAsync();

				// csv形式のファイルを読み出す

				string[] datas = readStr.Split(',');
				for (int i = 0; i < datas.Length && i < 7 * 24; i++)
				{
					table[i / 24, i % 24] = int.Parse(datas[i]);
				}

			}
			else {
				// table.txtが存在しない場合
				// 配列を初期化する
				for (int i = 0; i < 7; i++)
				{
					for (int j = 0; j < 24; j++)
					{
						table[i,j] = 0;
					}
				}
			}
			// 配列の用意が完了する
			// 現在時刻を取得し、曜日と時間を用意する
			DateTime LogInTime = DateTime.Now;
			int LogInHour = LogInTime.Hour;
			if (LogInHour > 23) LogInHour = 23;
			if (LogInHour < 0) LogInHour = 0;
			int LogInDayOfWeek = 0; // セグメンテーション違反を防ぐため初期値0
			switch (LogInTime.DayOfWeek)
			{
				case DayOfWeek.Sunday: LogInDayOfWeek = 0; break;
				case DayOfWeek.Monday: LogInDayOfWeek = 1; break;
				case DayOfWeek.Tuesday: LogInDayOfWeek = 2; break;
				case DayOfWeek.Wednesday: LogInDayOfWeek = 3; break;
				case DayOfWeek.Thursday: LogInDayOfWeek = 4; break;
				case DayOfWeek.Friday: LogInDayOfWeek = 5; break;
				case DayOfWeek.Saturday: LogInDayOfWeek = 6; break;
				default: break;
			}

			// 時刻と曜日の取得が完了

			// 配列の値を変更する

			table[LogInDayOfWeek, LogInHour] += 3; // 起動した時間と曜日が指す位置の値を増加
			// 他の曜日の同じ時間の値も増加
			for (int i = 0; i < 7; i++)
			{
				table[i, LogInHour] += 1; 
			}

			// オーバーフローを防ぐために要素の値がある一定値以上になったら全部の要素の値を減らしましょうっていう処理
			if (checkOver(table))
			{
				for (int i = 0; i < 7; i++)
				{
					for (int j = 0; j < 24; j++)
					{
						if (table[LogInDayOfWeek, LogInHour] > 1000)
						{
							table[LogInDayOfWeek, LogInHour] -= 1000;

						}
						else {
							table[LogInDayOfWeek, LogInHour] = 0;
						}
					}
				}
			}

			// オーバーフロー防止処理終了

			// 通知時刻決定処理
			// 何時に通知したいかを決めて、その差分を計算するメソッドに通知したい時間を投げる <-- まずはこのメソッドで実装します...
			int NotificationDOW = LogInDayOfWeek + 1;
			if (NotificationDOW == 7)
			{
				NotificationDOW = 0;
			}
			int NotificationHour = 0;
			for (int i = 0; i < 24; i++)
			{
				if (table[NotificationDOW, i] > table[NotificationDOW, NotificationHour])
				{
					NotificationHour = i;
				}
			}

			// 翌日の通知を投げる時刻を決定した

			// 配列保存用の文字列を作成する
			// csv形式のような形で保存
			// ルート下のフォルダtableのファイルtable.txtに上書きする。
			string saveTable = "";

			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 24; j++)
				{
					saveTable = saveTable + table[i, j].ToString();
					if (i != 6 || j != 23)
					{
						saveTable = saveTable + ",";
					}
				}
			}

			IFile tablefile = await tableFolder.CreateFileAsync("table.txt", CreationCollisionOption.ReplaceExisting);
			await tablefile.WriteAllTextAsync(saveTable);


			// 通知時間までの差分を求めよう(とりあえずint型)
			int interval = 3600 * (24 - LogInHour + NotificationHour); // これで通知時間までのざっくりした時間が求まる
			String NotificationStr = "スキマNote: スキマ時間でタスクの確認と作業をしよう！";
			int NotificationId = 1000; // OnStartNotificationには1000番台のidを割り当てます

			// 以下のコードで、プラットフォームごとに処理分岐を行う。
			var obj = DependencyService.Get<IMakeNotification>();
			// 必要な情報を渡して、通知を作成させる。
			obj.make("スキマNote", NotificationStr, NotificationId, interval);
		}

		// オーバーフロー防止のチェックに使用
		private bool checkOver(int[,] t)
		{
			foreach(var value in t)
			{
				if (value > 10000)
					return true;
			}
			return false;
		}
	}


	// インターフェースを宣言し、プラットフォームごとに実装
	public interface IMakeNotification
	{
		void make(string title, string text, int id, int interval);
	}

}

