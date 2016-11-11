using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace SukimaNote
{
	// 本体設定をする。詳細未定
	public class SettingPage : ContentPage
	{
		Label taskCountLimitLabel = new Label { Text = SharedData.taskCountLimit.ToString() };

		public SettingPage(RootPage rootPage)
		{
			Title = "設定";

			var taskCountLimitSlider = new Slider { Maximum = 50, Minimum = 10, Value = int.Parse(taskCountLimitLabel.Text) };
			taskCountLimitSlider.ValueChanged += (sender, e) => { taskCountLimitLabel.Text = ((int)taskCountLimitSlider.Value).ToString(); };

			var saveButton = new Button { Text = "設定を保存する" };
			saveButton.Clicked += (sender, e) =>
			{
				saveSetting();
			};

			Content = new StackLayout { Children = { taskCountLimitLabel, taskCountLimitSlider, saveButton } };
		}

		private void saveSetting()
		{
			Application.Current.Properties["taskCountList"] = int.Parse(taskCountLimitLabel.Text);
			SharedData.taskCountLimit = int.Parse(taskCountLimitLabel.Text);
		}
	}
}