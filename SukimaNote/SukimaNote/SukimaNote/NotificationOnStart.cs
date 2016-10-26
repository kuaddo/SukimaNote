using System;
using PCLStorage;



namespace SukimaNote
{
	public class NotificationOnStart
	{
		private int[,] table = new int[7,24];
		NotificationOnStart()
		{
			FuncAsync();
		}

		public async void FuncAsync()
		{
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


			// 配列の値を変更する



		}
	}

}

