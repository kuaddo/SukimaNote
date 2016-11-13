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
		Label taskCountLimitLabel = new Label { Text = SharedData.TaskCountLimit.ToString() };
		Label maxShowLabel		  = new Label { Text = SharedData.MaxShow.ToString() };
		Switch notificationSwitch = new Switch { IsToggled = SharedData.IsNotify };
		ObservableCollection<PlaceData> pList = new ObservableCollection<PlaceData>();

		public SettingPage(RootPage rootPage)
		{
			Title = "設定";

			// タスクの保存件数の設定
			var taskCountLimitSlider = new Slider { Maximum = 50, Minimum = 10, Value = int.Parse(taskCountLimitLabel.Text) };	// Maximumを先に設定しないとエラーが出る
			taskCountLimitSlider.ValueChanged += (sender, e) => { taskCountLimitLabel.Text = ((int)taskCountLimitSlider.Value).ToString(); };

			// 場所の設定
			foreach (var place in SharedData.placeList) { pList.Add(new PlaceData { Place = place }); }
			var placeListView = new ListView
			{
				ItemsSource = pList,
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
						pList.Remove(selectedPlace);
					}
				}
			};
			var placeListEditor = new Editor { Text = ""};
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
					pList.Add(new PlaceData { Place = placeListEditor.Text });
					placeListEditor.Text = "";
				}
			};

			// TopPageのタスクの表示件数についての設定
			// Sliderの表示範囲を偶数にしないと誤差により1小さい値に初期値が設定されてしまうので1~9
			var maxShowSlider = new Slider { Maximum = 9, Minimum = 1, Value = int.Parse(maxShowLabel.Text)};
			maxShowSlider.ValueChanged += (sender, e) => { maxShowLabel.Text = ((int)maxShowSlider.Value).ToString(); };

			// 全ての設定の保存
			var saveButton = new Button { Text = "設定を保存する" };
			saveButton.Clicked += (sender, e) =>
			{
				saveSetting();
				DisplayAlert("Saved", "設定が保存されました", "OK");
			};

			Content = new StackLayout { Children = { taskCountLimitLabel, taskCountLimitSlider, placeListView, placeListEditor, addPlaceButton, maxShowLabel, maxShowSlider, notificationSwitch, saveButton} };
		}

		// 追加しようとしている場所が重複していないかを確認するメソッド
		private bool checkPlace(string place)
		{
			foreach(var p in pList)
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
			SharedData.TaskCountLimit = int.Parse(taskCountLimitLabel.Text);
			Application.Current.Properties["taskCountList"] = SharedData.TaskCountLimit;

			string saveText = "";
			SharedData.placeList.Clear();
			foreach (var placeData in pList)
			{
				SharedData.placeList.Add(placeData.Place);
				saveText += ":" + placeData.Place;
			}
			Application.Current.Properties["placeList"] = saveText;

			SharedData.MaxShow = int.Parse(maxShowLabel.Text);
			Application.Current.Properties["maxShow"] = SharedData.MaxShow;

			SharedData.IsNotify = notificationSwitch.IsToggled;
			Application.Current.Properties["isNotify"] = SharedData.IsNotify;
		}
	}

	// SettingPageのListViewのためのクラス
	public class PlaceData
	{
		public string Place { get; set; }
	}
}