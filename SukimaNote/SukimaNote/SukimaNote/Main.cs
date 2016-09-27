using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace SukimaNote
{
	public class Main : Application
	{
		public Main()
		{
			// �^�X�N�ǂݍ��݂Ɏ��Ԃ������邱�Ƃ��l�����āA���[�f�B���O�y�[�W���ŏ��ɓǂݍ��܂��Ă���
			MainPage = new LoadingPage();
		}

		protected async override void OnStart()
		{
			// Handle when your app starts
			// �A�v���N������taskList��ǂݍ���ł���
			await TaskListView.MakeTaskDataListAsync();
			// taskList�ǂݍ��݌��5�b�҂�
			await Task.Delay(5000);
			// ���X�g�������RootPage��MainPage��
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
	}

	public class RootPage : MasterDetailPage
	{
		public RootPage()
		{
			var menuPage = new MenuPage();
			// ListView Menu��I����������NavigateTo���\�b�h��SelectedItem��n���BMenuItem�ɃL���X�g���Ă���
			menuPage.Menu.ItemSelected += (sender, e) => NavigateTo(e.SelectedItem as MenuItem);

			Master = menuPage;

			// �ŏ��̃y�[�W�̃Z�b�g�B�I�����ꂽ���Ƃɂ��ăC�x���g���Ăяo��
			menuPage.Menu.SelectedItem = new MenuItem
			{
				TargetType = typeof(TaskListPage),
			};
		}

		// �y�[�W�J�ڂ̃��\�b�h
		void NavigateTo(MenuItem menu)
		{
			// menuPage��List<MenuItem>�̑I������MenuItem�Ŏ󂯎��B�C���X�^���X����?
			ContentPage displayPage = (ContentPage)Activator.CreateInstance(menu.TargetType);

			// �e�y�[�W�Ɉړ�����Ƃ��Ƀo�[�̐F���Đݒ肷��
			Detail = new NavigationPage(displayPage)
			{
				BarBackgroundColor = Color.FromHex("3498DB"),
				BarTextColor = Color.White,
				//Icon = "icon.png",
			};

			IsPresented = false;
		}
	}

	// �����̃��j���[�y�[�W
	public class MenuPage : ContentPage
	{
		public ListView Menu { get; set; }

		public MenuPage()
		{
			Title = "Menu";     // �K�{
			BackgroundColor = Color.Gray;

			// ListView�ݒ�
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

			// �^�C�g����menuLabel��ListView����ׂĂ���
			var layout = new StackLayout
			{
				Spacing = 0,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Children = { menuLabel, Menu },
			};

			Content = layout;
		}
	}

	// ���j���[�Ɏg���BListView���p�������N���X
	public class MenuListView : ListView
	{
		public MenuListView()
		{
			// �C���X�^���X������ItemsSource�Ƃ��Ďw�肷��
			List<MenuItem> data = new MenuListData();
			ItemsSource = data;
			VerticalOptions = LayoutOptions.FillAndExpand;
			BackgroundColor = Color.Transparent;

			// TextCell�̎g�p
			var cell = new DataTemplate(typeof(TextCell));
			cell.SetBinding(TextCell.TextProperty, "Title");

			ItemTemplate = cell;
		}
	}

	// ListView�̃f�[�^�p�̃N���X�BTargetType�ɑJ�ڐ�y�[�W���w�肷��
	public class MenuItem
	{
		public string Title { get; set; }
		// �y�[�W�N���X�̎w������Ă���
		public Type TargetType { get; set; }
	}


	// ListView�̃f�[�^�N���X�BMaster�Ɏg���y�[�W��o�^����
	public class MenuListData : List<MenuItem>
	{
		public MenuListData()
		{
			this.Add(new MenuItem()
			{
				Title = "�g�b�v�y�[�W",
				TargetType = typeof(TopPage),
			});
			this.Add(new MenuItem()
			{
				Title = "�^�X�N�ꗗ",
				TargetType = typeof(TaskListPage),
			});
			this.Add(new MenuItem()
			{
				Title = "�{�̐ݒ�",
				TargetType = typeof(SettingPage),
			});
			this.Add(new MenuItem()
			{
				Title = "�X�P�W���[���ݒ�",
				TargetType = typeof(SchedulePage),
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