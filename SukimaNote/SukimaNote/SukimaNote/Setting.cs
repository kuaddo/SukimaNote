using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace SukimaNote
{
	// 本体設定をする。詳細未定
	public class SettingPage : ContentPage
	{
		// Maximumを先に設定しないとエラーが出る
		Slider taskCountLimitSlider = new Slider { Maximum = 30, Minimum = 10, Value = SharedData.TaskCountLimit };  
		// Sliderの表示範囲を偶数にしないと誤差により1小さい値に初期値が設定されてしまうので1~9
		Slider maxShowSlider		= new Slider { Maximum = 9,  Minimum = 1,  Value = SharedData.MaxShow };
		ObservableCollection<PlaceData> placeList = new ObservableCollection<PlaceData>();
		Editor placeListEditor = new Editor { Text = "", BackgroundColor = Color.Gray, HorizontalOptions = LayoutOptions.FillAndExpand };
		Switch notificationSwitch = new Switch { IsToggled = SharedData.IsNotify };

		public SettingPage(RootPage rootPage)
		{
			Title = "設定";
			BackgroundColor = Color.FromHex(MyColor.BackgroundColor);

			// 全ての設定の保存
			var saveButton = new Button { Text = "設定を保存する" };
			saveButton.Clicked += (sender, e) =>
			{
				saveSetting();
				DisplayAlert("Saved", "設定が保存されました", "OK");
			};

			Content = new StackLayout { Children = { makeTaskCountLimit(), makeMaxShow(), makePlaceList(), notificationSwitch, saveButton} };
		}

		// 追加しようとしている場所が重複していないかを確認するメソッド
		private bool checkPlace(string place)
		{
			foreach(var p in placeList)
			{
				if (p.Place == place)
				{
					return false;
				}
			}
			return true;
		}
		// 設定を保存するメソッド
		private void saveSetting()
		{
			SharedData.TaskCountLimit = (int)taskCountLimitSlider.Value;
			Application.Current.Properties["taskCountLimit"] = SharedData.TaskCountLimit;

			SharedData.MaxShow = (int)maxShowSlider.Value;
			Application.Current.Properties["maxShow"] = SharedData.MaxShow;

			string saveText = "";
			SharedData.placeList.Clear();
			foreach (var placeData in placeList)
			{
				SharedData.placeList.Add(placeData.Place);
				saveText += ":" + placeData.Place;
			}
			Application.Current.Properties["placeList"] = saveText;

			SharedData.IsNotify = notificationSwitch.IsToggled;
			Application.Current.Properties["isNotify"] = SharedData.IsNotify;
		}
		// タスクの保存件数の設定レイアウトの作成
		private StackLayout makeTaskCountLimit()
		{
			// 重さを考慮して30まで
			var label = new Label { Text = "タスクの上限数:", TextColor = Color.Black };
			var countLabel = new Label { Text = SharedData.TaskCountLimit.ToString(), TextColor = Color.Black };
			taskCountLimitSlider.ValueChanged += (sender, e) => { countLabel.Text = ((int)taskCountLimitSlider.Value).ToString(); };

			return new StackLayout
			{
				Children =
				{
					new StackLayout { Orientation = StackOrientation.Horizontal, Children = { label, countLabel} },
					taskCountLimitSlider
				}
			};
		}
		// TopPageの表示件数の設定レイアウトの作成
		private StackLayout makeMaxShow()
		{
			var label = new Label { Text = "トップページの表示件数:", TextColor = Color.Black };
			var countLabel = new Label { Text = SharedData.MaxShow.ToString(), TextColor = Color.Black };
			maxShowSlider.ValueChanged += (sender, e) => { countLabel.Text = ((int)maxShowSlider.Value).ToString(); };

			return new StackLayout
			{
				Children =
				{
					new StackLayout { Orientation = StackOrientation.Horizontal, Children = { label, countLabel} },
					maxShowSlider
				}
			};
		}
		// 場所の設定レイアウトの作成
		private StackLayout makePlaceList()
		{
			var label = new Label { Text = "場所の一覧", TextColor = Color.Black };
			foreach (var place in SharedData.placeList) { placeList.Add(new PlaceData { Place = place }); }
			var placeListView = new ListView
			{
				ItemsSource = placeList,
				ItemTemplate = new DataTemplate(typeof(TextCell))
			};
			placeListView.ItemTemplate.SetBinding(TextCell.TextProperty, nameof(PlaceData.Place));
			placeListView.ItemSelected += async (sender, e) =>
			{
				var selectedPlace = e.SelectedItem as PlaceData;
				if (selectedPlace.Place != "指定無し")
				{
					if (await DisplayAlert("Caution", selectedPlace.Place + "を削除しますか?", "YES", "NO"))
					{
						placeList.Remove(selectedPlace);
					}
				}
			};

			var addPlaceButton = new Button { Text = "追加" };
			addPlaceButton.Clicked += (sender, e) =>
			{
				if (placeListEditor.Text == "")
					DisplayAlert("Error", "追加する場所を入力してください", "OK");
				else if (placeListEditor.Text.IndexOf(":") >= 0)
					DisplayAlert("Error", "場所に半角のセミコロン : は使えません", "OK");
				else if (!checkPlace(placeListEditor.Text))
					DisplayAlert("Error", "重複して場所を登録できません", "OK");
				else
				{
					placeList.Add(new PlaceData { Place = placeListEditor.Text });
					placeListEditor.Text = "";
				}
			};

			return new StackLayout
			{
				Children =
				{
					label,
					placeListView,
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						Children = { placeListEditor, addPlaceButton}
					}
				}
			};
		}
	}

	// SettingPageのListViewのためのクラス
	public class PlaceData
	{
		public string Place { get; set; }
	}
}