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

			// 予想作業時間入力
			var timeToFinishPicker = new Picker { BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize };
			foreach (var time in SharedData.timeToFinishList) { timeToFinishPicker.Items.Add(time); }
			var timeToFinishSupplementary = new Label { Text = "作業完了に必要な時間を入力してください", FontSize = fontSize, IsVisible = false };
			var timeToFinishTGR = new TapGestureRecognizer();
			timeToFinishTGR.Tapped += (sender, e) =>
			{
				timeToFinishSupplementary.IsVisible = !timeToFinishSupplementary.IsVisible;
			};
			var timeToFinishImage = new Image { Source = "question.png", WidthRequest = fontSize, HeightRequest = fontSize };
			timeToFinishImage.GestureRecognizers.Add(timeToFinishTGR);

			var timeToFinish = new StackLayout
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
							new Label { Text = "予想作業時間　　　　", FontSize = fontSize},
							timeToFinishImage,
							timeToFinishSupplementary
						}
					},
					timeToFinishPicker
				}
			};

			// 場所
			var placePicker = new Picker { BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize };
			foreach (var p in SharedData.placeList) { placePicker.Items.Add(p); };
			var placeSupplementary = new Label { Text = "作業できる場所を選択してください", FontSize = fontSize, IsVisible = false };
			var placeTGR = new TapGestureRecognizer();
			placeTGR.Tapped += (sender, e) =>
			{
				placeSupplementary.IsVisible = !placeSupplementary.IsVisible;
			};
			var placeImage = new Image { Source = "question.png", WidthRequest = fontSize, HeightRequest = fontSize };
			placeImage.GestureRecognizers.Add(placeTGR);

			var place = new StackLayout
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
							new Label { Text = "場所　　　　　　　　", FontSize = fontSize},
							placeImage,
							placeSupplementary
						}
					},
					placePicker
				}
			};

			// 優先度
			var priorityButtonL = new Button { Text = "低い", BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize };
			var priorityButtonM = new Button { Text = "普通", BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize };
			var priorityButtonH = new Button { Text = "高い", BackgroundColor = Color.FromHex(MyColor.MainColor1), HeightRequest = inputSize };
			var priorityLabel = new Label { Text = "普通", HorizontalOptions = LayoutOptions.CenterAndExpand, FontSize = fontSize };
			priorityButtonL.Clicked += (sender, e) => { priorityLabel.Text = "低い"; };
			priorityButtonM.Clicked += (sender, e) => { priorityLabel.Text = "普通"; };
			priorityButtonH.Clicked += (sender, e) => { priorityLabel.Text = "高い"; };
			var prioritySupplementary = new Label { Text = "優先度を選択してください", FontSize = fontSize, IsVisible = false };
			var priorityTGR = new TapGestureRecognizer();
			priorityTGR.Tapped += (sender, e) =>
			{
				prioritySupplementary.IsVisible = !prioritySupplementary.IsVisible;
			};
			var priorityImage = new Image { Source = "question.png", WidthRequest = fontSize, HeightRequest = fontSize };
			priorityImage.GestureRecognizers.Add(priorityTGR);

			var priority = new StackLayout
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
							new Label { Text = "優先度　　　　　　　", FontSize = fontSize},
							priorityImage,
							prioritySupplementary
						}
					},
					new StackLayout
					{
						Padding = new Thickness(30, 0, 0, 0),
						Spacing = 10,
						Orientation = StackOrientation.Horizontal,
						Children = { priorityButtonL, priorityButtonM, priorityButtonH, priorityLabel}
					}
				}
			};

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
			timeToFinishPicker.SelectedIndex = 1;                                               // 10分を選択
			placePicker.SelectedIndex = 0;														// 指定無しを選択
			// 優先度
			remarkEditor.Text = "";																// 初期値は空白
	
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
													 timeToFinishPicker.SelectedIndex + ':' +
													 placePicker.SelectedIndex + ':' +
													 SharedData.priorityList.IndexOf(priorityLabel.Text) + ':' +					  // TODO: 気持ち悪いから直す
													 "0:" +																			  // 進捗度は0で保存
													 remarkEditor.Text + ':');
						SharedData.taskList.Add(new TaskData
						{
							Title = titleEntry.Text,
							Deadline = new DateTime(termDatePicker.Date.Ticks + termTimePicker.Time.Ticks),
							TimeToFinish = timeToFinishPicker.SelectedIndex,
							Place = placePicker.SelectedIndex,
							Priority = SharedData.priorityList.IndexOf(priorityLabel.Text),
							Progress = 0,
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
				Children = { minimumLabel, title, term, timeToFinish, saveArray[0] }
			};

			var option = new StackLayout
			{
				BackgroundColor = Color.FromHex(MyColor.MainColor2),
				Padding = new Thickness(0, 0, 0, 40),
				Children = { optionLabel, place, priority, remark, saveArray[1] }
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