using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;
using PCLStorage;

namespace SukimaNote
{
	public class Main : Application
	{
		public Main()
		{
			// タスク読み込みに時間がかかることを考慮して、ローディングページを最初に読み込ませておく
			MainPage = new LoadingPage();
		}

		protected async override void OnStart()
		{
			// アプリ起動時にtaskListとplaceListを読み込んでおく
			//SharedData.MakePlaceList();
			//await deleteTaskData();
			await SharedData.MakeTaskDataListAsync();
			// リスト生成後にRootPageをMainPageに
			MainPage = new RootPage();
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}

		// PCLStrageの内容を全て削除するメソッド。デバッグ用
		private async Task deleteTaskData()
		{
			IFolder rootFolder = FileSystem.Current.LocalStorage;
			IFolder taskDataFolder = await rootFolder.CreateFolderAsync("taskDataFolder", CreationCollisionOption.OpenIfExists);    // 存在しなかったならば作成
			IList<IFile> deletefiles = await taskDataFolder.GetFilesAsync();
			await Task.WhenAll(deletefiles.Select(async file => await file.DeleteAsync()));
		}
	}

	public class RootPage : MasterDetailPage
	{
		public RootPage()
		{
			var menuPage = new MenuPage();
			// ListView Menuを選択した時にNavigateToメソッドにSelectedItemを渡す。MenuItemにキャストしている
			menuPage.Menu.ItemSelected += (sender, e) => { NavigateTo(e.SelectedItem as MenuData); menuPage.Menu.SelectedItem = null; };

			Master = menuPage;

			// 最初のページのセット。選択されたことにしてイベントを呼び出す
			menuPage.Menu.SelectedItem = new MenuData
			{
				TargetType = typeof(TopPage),
			};
		}

		// ページ遷移のメソッド
		public void NavigateTo(MenuData menu)
		{
			if (menu != null)
			{
				// menuPageのList<MenuItem>の選択肢をMenuItemで受け取る。インスタンス生成?
				ContentPage displayPage = (ContentPage)Activator.CreateInstance(menu.TargetType, this);

				// 各ページに移動するときにバーの色を再設定する
				Detail = new NavigationPage(displayPage)
				{
					BarBackgroundColor = Color.FromHex("3498DB"),
					BarTextColor = Color.White,
				};

				IsPresented = false;
			}
		}
	}

	// 左側のメニューページ
	public class MenuPage : ContentPage
	{
		public ListView Menu { get; set; }

		public MenuPage()
		{
			Title = "Menu";     // 必須
			BackgroundColor = Color.Gray;

			// ListView設定
			Menu = new MenuListView();

			var menuLabel = new ContentView
			{
				Padding = new Thickness(10, 20, 0, 5),
				Content = new Label
				{
					TextColor = Color.FromHex("333"),
					Text = "Menu",
					FontSize = 18,
				}
			};

			// タイトルのmenuLabelとListViewを並べている
			var layout = new StackLayout
			{
				Spacing = 0,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = { menuLabel, Menu },
			};

			Content = layout;
		}
	}

	// メニューに使う。ListViewを継承したクラス
	public class MenuListView : ListView
	{
		public MenuListView()
		{
			// インスタンス化してItemsSourceとして指定する
			List<MenuData> data = new MenuListData();
			ItemsSource = data;
			VerticalOptions = LayoutOptions.FillAndExpand;
			BackgroundColor = Color.Transparent;

			// TextCellの使用
			var cell = new DataTemplate(typeof(ImageCell));
			cell.SetBinding(TextCell.TextProperty, nameof(MenuData.Title));
			cell.SetBinding(ImageCell.ImageSourceProperty, nameof(MenuData.IconSource));

			ItemTemplate = cell;

			RowHeight = 70;
			WidthRequest = 100;
		}
	}

	// ListViewのデータ用のクラス。TargetTypeに遷移先ページを指定する
	public class MenuData
	{
		public string Title { get; set; }
		// ページクラスの指定をしている
		public Type TargetType { get; set; }
		public string IconSource { get; set; }
	}


	// ListViewのデータクラス。Masterに使うページを登録する
	public class MenuListData : List<MenuData>
	{
		public MenuListData()
		{
			this.Add(new MenuData()
			{
				Title = "トップページ",
				TargetType = typeof(TopPage),
				IconSource = "appIcon.png"
			});
			this.Add(new MenuData()
			{
				Title = "タスク一覧",
				TargetType = typeof(TaskListPage),
				IconSource = "icon.png"
			});
			this.Add(new MenuData()
			{
				Title = "本体設定",
				TargetType = typeof(SettingPage),
				IconSource = "setting.png"
			});
		}
	}

	public class LoadingPage : ContentPage
	{
		public LoadingPage()
		{
			Content = new StackLayout
			{
				Children =
				{
					new Label
					{
						Text = "Now Loading...",
						FontSize = 50,
						HorizontalOptions = LayoutOptions.CenterAndExpand,
						VerticalOptions = LayoutOptions.CenterAndExpand
					},
					new ActivityIndicator()
				}
			};
		}
	}
}