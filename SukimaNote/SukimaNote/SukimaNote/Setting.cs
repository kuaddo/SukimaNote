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
		Label taskCountLimitLabel = new Label { Text = SharedData.taskCountLimit.ToString() };
		ObservableCollection<PlaceData> pList = new ObservableCollection<PlaceData>();

		public SettingPage(RootPage rootPage)
		{
			Title = "設定";

			var taskCountLimitSlider = new Slider { Maximum = 50, Minimum = 10, Value = int.Parse(taskCountLimitLabel.Text) };
			taskCountLimitSlider.ValueChanged += (sender, e) => { taskCountLimitLabel.Text = ((int)taskCountLimitSlider.Value).ToString(); };

			var saveButton = new Button { Text = "設定を保存する" };


			foreach (var place in SharedData.placeList) { pList.Add(new PlaceData { Place = place }); }
			var placeListView = new ListView
			{
				ItemsSource = pList,
				ItemTemplate = new DataTemplate(typeof(TextCell))
			};
			placeListView.ItemTemplate.SetBinding(TextCell.TextProperty, nameof(PlaceData.Place));
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
			saveButton.Clicked += (sender, e) =>
			{
				saveSetting();
			};

			Content = new StackLayout { Children = { taskCountLimitLabel, taskCountLimitSlider, placeListView, placeListEditor, addPlaceButton, saveButton} };
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
			SharedData.taskCountLimit = int.Parse(taskCountLimitLabel.Text);
			Application.Current.Properties["taskCountList"] = SharedData.taskCountLimit;

			string saveText = "";
			SharedData.placeList.Clear();
			foreach (var placeData in pList)
			{
				SharedData.placeList.Add(placeData.Place);
				saveText += ":" + placeData.Place;
			}
			Application.Current.Properties["placeList"] = saveText;

		}
	}

	// SettingPageのListViewのためのクラス
	public class PlaceData
	{
		public string Place { get; set; }
	}
}