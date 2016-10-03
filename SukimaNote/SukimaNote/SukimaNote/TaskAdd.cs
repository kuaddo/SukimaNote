using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using PCLStorage;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SukimaNote
{
	// タスクの追加の設定ページ
	public class TaskAddPage : ContentPage
	{
		const int fontSize = 20;
		const int inputSize = 50;

		public TaskAddPage()
		{
			Title = "タスク追加";

			// タスクのデータ入力部分
			// タイトル入力
			var titleEntry = new Entry { Keyboard = Keyboard.Text, BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize, FontSize = 30};
			var titleSupplementary = new Label { Text = "予定にわかり易い名前をつけてください", FontSize = fontSize, IsVisible = false };
			var titleTGR = new TapGestureRecognizer();
			titleTGR.Tapped += (sender, e) =>
			{
				titleSupplementary.IsVisible = !titleSupplementary.IsVisible;
			};
			var titleImage = new Image { Source = "question.png", WidthRequest = fontSize, HeightRequest = fontSize };
			titleImage.GestureRecognizers.Add(titleTGR);

			var title = new StackLayout
			{
				Children =
				{
					new StackLayout
					{
						Padding = new Thickness(5, 0, 0, 0),
						Spacing = 7,
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							new Label { Text = "タイトル　　　　　　", FontSize = fontSize},
							titleImage,
							titleSupplementary
						}
					},
					titleEntry
				}
			};

			// 期限入力
			var termDatePicker = new DatePicker { BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize, HorizontalOptions = LayoutOptions.FillAndExpand };
			var termTimePicker = new TimePicker { BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize, HorizontalOptions = LayoutOptions.FillAndExpand };
			var termSupplementary = new Label { Text = "作業の期限を入力してください", FontSize = fontSize, IsVisible = false };
			var termTGR = new TapGestureRecognizer();
			termTGR.Tapped += (sender, e) =>
			{
				termSupplementary.IsVisible = !termSupplementary.IsVisible;
			};
			var termImage = new Image { Source = "question.png", WidthRequest = fontSize, HeightRequest = fontSize };
			termImage.GestureRecognizers.Add(termTGR);

			var term = new StackLayout
			{
				Children =
				{
					new StackLayout
					{
						Padding = new Thickness(5, 0, 0, 0),
						Spacing = 7,
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							new Label { Text = "期限　　　　　　　　", FontSize = fontSize},
							termImage,
							termSupplementary
						}
					},
					new StackLayout
					{
						Spacing = 7,
						Orientation = StackOrientation.Horizontal,
						Children = { termDatePicker, termTimePicker }
					}
				}
			};

			// 予想残り時間入力
			var restTimePicker = new Picker { BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize };
			foreach (var time in SharedData.restTimeList) { restTimePicker.Items.Add(time); }
			var restTimeSupplementary = new Label { Text = "作業完了に必要な時間を入力してください", FontSize = fontSize, IsVisible = false };
			var restTimeTGR = new TapGestureRecognizer();
			restTimeTGR.Tapped += (sender, e) =>
			{
				restTimeSupplementary.IsVisible = !restTimeSupplementary.IsVisible;
			};
			var restTimeImage = new Image { Source = "question.png", WidthRequest = fontSize, HeightRequest = fontSize };
			restTimeImage.GestureRecognizers.Add(restTimeTGR);

			var restTime = new StackLayout
			{
				Children =
				{
					new StackLayout
					{
						Padding = new Thickness(5, 0, 0, 0),
						Spacing = 7,
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							new Label { Text = "予想残り時間　　　　", FontSize = fontSize},
							restTimeImage,
							restTimeSupplementary
						}
					},
					restTimePicker
				}
			};

			// 最低単位作業時間入力
			var unitTimePicker = new Picker { BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize };
			foreach (var time in SharedData.unitTimeList) { unitTimePicker.Items.Add(time); }
			var unitTimeSupplementary = new Label { Text = "少なくとも作業実行に必要な時間を入力してください", FontSize = fontSize, IsVisible = false };
			var unitTimeTGR = new TapGestureRecognizer();
			unitTimeTGR.Tapped += (sender, e) =>
			{
				unitTimeSupplementary.IsVisible = !unitTimeSupplementary.IsVisible;
			};
			var unitTimeImage = new Image { Source = "question.png", WidthRequest = fontSize, HeightRequest = fontSize };
			unitTimeImage.GestureRecognizers.Add(unitTimeTGR);

			var unitTime = new StackLayout
			{
				Children =
				{
					new StackLayout
					{
						Padding = new Thickness(5, 0, 0, 0),
						Spacing = 7,
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							new Label { Text = "最低単位作業時間　　", FontSize = fontSize},
							unitTimeImage,
							unitTimeSupplementary
						}
					},
					unitTimePicker
				}
			};

			// 場所


			// 優先度


			// 備考入力
			var remarkEditor = new Editor { BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = 500, FontSize = fontSize };
			var remarkSupplementary = new Label { Text = "メモとして記録しておきたいことを入力してください", FontSize = fontSize,IsVisible = false };
			var remarkTGR = new TapGestureRecognizer();
			remarkTGR.Tapped += (sender, e) =>
			{
				remarkSupplementary.IsVisible = !remarkSupplementary.IsVisible;
			};
			var remarkImage = new Image { Source = "question.png", WidthRequest = fontSize, HeightRequest = fontSize };
			remarkImage.GestureRecognizers.Add(remarkTGR);
			
			var remark = new StackLayout
			{
				Children =
				{
					new StackLayout
					{
						Padding = new Thickness(5, 0, 0, 0),
						Spacing = 7,
						Orientation = StackOrientation.Horizontal,
						Children =
						{
							new Label { Text = "備考　　　　　　　　", FontSize = fontSize},
							remarkImage,
							remarkSupplementary
						}
					},
					remarkEditor
				}
			};

			// それぞれの初期値
			titleEntry.Text = "Task";															// 初期値はいらない？
			termDatePicker.Date = new DateTime(DateTime.Now.Ticks + TimeSpan.TicksPerDay);		// 次の日
			termTimePicker.Time = new TimeSpan(DateTime.Now.Ticks - DateTime.Now.Date.Ticks);   // 時刻は同じ
			restTimePicker.SelectedIndex = 1;                                                   // インデックスで指定
			unitTimePicker.SelectedIndex = 0;
			// 場所
			// 優先度
			remarkEditor.Text = "";
	
			// varで作ったインスタンスをコピーする方法がわからない
			// もう少しスマートな書き方があると思う
			var saveArray = new StackLayout[2];
			for (int i = 0; i < 2; i++)
			{
				// 「完了」ボタン。データの保存をする
				var saveButton = new Button
				{
					Text = "Save",
					FontSize = 40,
					BackgroundColor = Color.FromHex(MyColor.MainColor1),
				};
				saveButton.Clicked += async (sender, e) =>
				{
					if (titleEntry.Text == "")
					{
						await DisplayAlert("Error", "タイトルを入力てください", "OK");
					}
					else if (titleEntry.Text.IndexOf(":") >= 0)
					{
						await DisplayAlert("Error", "タイトルに半角のセミコロン : は使えません", "OK");
					}
					else if (remarkEditor.Text.IndexOf(":") >= 0)
					{
						await DisplayAlert("Error", "備考に半角のセミコロン : は使えません", "OK");
					}
					else if (termDatePicker.Date.Ticks + termTimePicker.Time.Ticks < DateTime.Now.Ticks)
					{
						await DisplayAlert("Error", "期限が過去に設定されています", "OK");
					}
					else
					{
						IFolder rootFolder = FileSystem.Current.LocalStorage;
						IFile file = await rootFolder.CreateFileAsync(titleEntry.Text + ".txt", CreationCollisionOption.GenerateUniqueName);
						await file.WriteAllTextAsync(titleEntry.Text + ':' +
													 (termDatePicker.Date.Ticks + termTimePicker.Time.Ticks).ToString() + ':' +       // long型のTicksの和ををstringにして保存。
													 restTimePicker.SelectedIndex + ':' +
													 unitTimePicker.SelectedIndex + ':' +
													 remarkEditor.Text + ':');
						SharedData.taskList.Add(new TaskData
						{
							Title = titleEntry.Text,
							Term = new DateTime(termDatePicker.Date.Ticks + termTimePicker.Time.Ticks),
							RestTime = restTimePicker.SelectedIndex,
							UnitTime = unitTimePicker.SelectedIndex,
							Remark = remarkEditor.Text,
						});
						await DisplayAlert("Saved", titleEntry.Text + "が保存されました。", "OK");
						// 元の画面に戻す
						//await Navigation.PopAsync();
					}
				};
				var save = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					Children = { new Label { HorizontalOptions = LayoutOptions.FillAndExpand }, saveButton }
				};

				saveArray[i] = save;
			}

			// 以下配置
			var minimumLabel = new Label { Text = "基本設定", FontSize = 30, HorizontalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.FromHex(MyColor.MainColor3), TextColor = Color.White };
			var optionLabel = new Label { Text = "追加設定", FontSize = 30, HorizontalOptions = LayoutOptions.Fill,
				BackgroundColor = Color.FromHex(MyColor.MainColor3), TextColor = Color.White };
			var minimum = new StackLayout
			{
				BackgroundColor = Color.FromHex(MyColor.MainColor2),
				Padding = new Thickness(0, 0, 0, 40),
				Children = { minimumLabel, title, term, restTime, saveArray[0] }
			};

			var option = new StackLayout
			{
				BackgroundColor = Color.FromHex(MyColor.MainColor2),
				Padding = new Thickness(0, 0, 0, 40),
				Children = { optionLabel, unitTime, remark, saveArray[1] }
			};

			Content = new ScrollView
			{
				BackgroundColor = Color.FromHex(MyColor.MainColor2),
				Content = new StackLayout
				{
					// iOSのみ上部に空白を取る。Navigationだから必要ない?
					//Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0),
					Children = { minimum, option }
				}
			};
		}
	}
}