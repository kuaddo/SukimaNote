using System;
using PCLStorage;

/*

namespace SukimaNote
{
	public class NotificationOnStart
	{
		private int[,] table = new int[7,24];
		NotificationOnStart()
		{
			async () =>
			{
				// ルートフォルダを取得
				IFolder rootfolder = FileSystem.Current.LocalStorage;
				// ルート直下にフォルダを作成（すでにある場合はそれをオープン）
				IFolder TableFolder = await IFolder.createFolderAsync("Table", CreationCollisionOption.OpenIfExists);
				// ファイルが存在するかを確かめる
				ExistenceCheckResult ret = TableFolder.CheckExistsAsync("Table.txt");
				if (ret.FileExists == ExistenceCheckResult.FileExists)
				{
					
				}

			}
		}


	}

}

*/