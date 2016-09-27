using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SukimaNote
{
	public class TopPage : ContentPage
	{
		ObservableCollection<TaskData> orderedObservableCollection = new ObservableCollection<TaskData>();

		public TopPage()
		{
			Title = "トップページ";
			// ListViewで表示テスト
			var listView = new ListView();
			var cell = new DataTemplate(typeof(TextCell));
			cell.SetBinding(TextCell.TextProperty, "Title");
			cell.SetBinding(TextCell.DetailProperty, "Term");
			CalculationOfPriority();
			listView.ItemsSource = orderedObservableCollection;
			listView.ItemTemplate = cell;

			Content = listView;
		}

		// 引数で考慮する要素を受け取り、優先度を計算するメソッド。ObservableCollectionで通知を受け取る
		// けんたが書いたアルゴリズムをそのままここに入れる
		// TODO: アルゴリズム完成後に非同期処理に変更する
		private void CalculationOfPriority()
		{
			// TODO: 有効なタスクを考慮してから表示可能なタスクの数を数えるようにする
			// 今のところ期限の近い順にソートするだけなので、そのままCountで数を取得
			int taskCount = TaskListView.taskList.Count;

			// 数が3つ以上と仮定してテスト
			//if (taskCount >= 3)
			//{
			orderedObservableCollection = new ObservableCollection<TaskData>(TaskListView.taskList
																					     .OrderByDescending(task => task.Term.Ticks)
																					     .Take(3));
			//}  
			//else
				//return null;
		}
	}
}