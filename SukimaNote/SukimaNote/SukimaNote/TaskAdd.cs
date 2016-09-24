using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using PCLStorage;

namespace SukimaNote
{
	// タスクの設定項目のプロパティのクラス
	public class TaskData
	{
		// 最低限
		public string Title { get; set; }		// タイトル
		public int RestTime { get; set; }       // 予想残り時間(インデックスで保存)	
		public DateTime Term { get; set; }      // 期限

		// 追加オプション
		public int UnitTime { get; set; }       // 最低単位作業時間(インデックスで保存)	
		public string Remark { get; set; }		// 備考
	}

	// タスクの追加の設定ページ
	public class TaskAddPage : ContentPage
	{
		IFolder rootFolder = FileSystem.Current.LocalStorage;
		const int fontSize = 20;
		const int inputSize = 50;

		public TaskAddPage()
		{
			Title = "タスク追加";
			// NavigationBarを非表示に
			NavigationPage.SetHasNavigationBar(this, false);

			// 1~60分までのリストの作成
			var ar = Enumerable.Range(1, 60).Select(n => string.Format("{0}分", n)).ToList();

			// タスクのデータ入力部分
			// タイトル入力
			var titleEntry = new Entry { Keyboard = Keyboard.Text, BackgroundColor = Color.Green, HeightRequest = inputSize, FontSize = 30};
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

			// 予想残り時間入力
			var restTimePicker = new Picker { BackgroundColor = Color.Green, HeightRequest = inputSize };
			foreach (var a in ar) { restTimePicker.Items.Add(a); }
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

			// 期限入力
			var termDatePicker = new DatePicker { Format = "D", BackgroundColor = Color.Green, HeightRequest = inputSize };
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
					termDatePicker
				}
			};

			// 最低単位作業時間入力
			var unitTimePicker = new Picker { BackgroundColor = Color.Green, HeightRequest = inputSize };
			foreach (var a in ar) { unitTimePicker.Items.Add(a); }
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

			// 備考入力
			// 高さを直接数値指定ではなくFillのように指定する方法がわからない
			var remarkEditor = new Editor { BackgroundColor = Color.Green, HeightRequest = 400, FontSize = fontSize };
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
			titleEntry.Text = "Task";
			restTimePicker.SelectedIndex = 9;   // インデックスで指定
			termDatePicker.Date = new DateTime(DateTime.Now.Ticks + TimeSpan.TicksPerDay);  // 次の日
			unitTimePicker.SelectedIndex = 9;
			remarkEditor.Text = "";

			var minimumLabel = new Label { Text = "基本設定", FontSize = 30, HorizontalOptions = LayoutOptions.Start, BackgroundColor = Color.White, TextColor = Color.Black };
			var optionLabel = new Label { Text = "追加設定", FontSize = 30, HorizontalOptions = LayoutOptions.Start, BackgroundColor = Color.White, TextColor = Color.Black };
			var minimum = new StackLayout
			{
				Children = { minimumLabel, title, restTime, term }
			};

			var option = new StackLayout
			{
				Children = { optionLabel, unitTime, remark }
			};

			// 「一時保存」ボタンを追加する。properties dictionaryに保存
			// タスクを追加しないで戻る

			// varで作ったインスタンスをコピーする方法がわからない
			// もう少しスマートな書き方があると思う
			var saveArray = new StackLayout[2];
			for (int i = 0; i < 2; i++)
			{
				// 「完了」ボタン。データの保存をする
				var saveButton = new Button
				{
					Text = "Save",
					FontSize = 30,
					BackgroundColor = Color.Aqua,
				};
				saveButton.Clicked += async (sender, e) =>
				{
					if (titleEntry.Text == "")
					{
						await DisplayAlert("エラー", "タイトルを入力てください", "OK");
					}
					else
					{
						IFile file = await rootFolder.CreateFileAsync(titleEntry.Text + ".txt", CreationCollisionOption.GenerateUniqueName);
						await file.WriteAllTextAsync(titleEntry.Text + ':' +
													 restTimePicker.SelectedIndex + ':' +
													 unitTimePicker.SelectedIndex + ':' +
													 termDatePicker.Date.Ticks.ToString() + ':' +       // long型のTicksをstringにして保存
													 remarkEditor.Text + ':');
					}
					await DisplayAlert("Save", titleEntry.Text + "が保存されました。", "OK");
				};
				var save = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					Children = { new Label { HorizontalOptions = LayoutOptions.FillAndExpand }, saveButton }
				};

				saveArray[i] = save;
			}

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					// iOSのみ上部に空白を取る。Navigationだから必要ない?
					//Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0),
					Children = { minimum, saveArray[0], option, saveArray[1] }
				}
			};
		}
	}
}