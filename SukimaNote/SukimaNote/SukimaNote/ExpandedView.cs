using Xamarin.Forms;

namespace SukimaNote
{
	// チェックボックスに用いるView。タップで画像の切り替えができる。
	public class CheckBoxImage : Image
	{
		// IsClosedのBindableProperty
		public readonly static BindableProperty IsClosedProperty = BindableProperty.Create<CheckBoxImage, bool>(
			o => o.IsClosed,			// getter
			false,						// 初期値
			BindingMode.TwoWay,			// とりあえずTwoWay
			null,
			OnIsClosedPropertyChanged); // プロパティ変更イベントハンドラ

		// 追加で実装したプロパティ。値によって２つの画像を切り替える。
		public bool IsClosed
		{
			get { return (bool)GetValue(IsClosedProperty); }
			set { SetValue(IsClosedProperty, value); }
		}

		// IsClosedで切り替える画像のソース
		private const string trueSource  = "checkA.png";
		private const string falseSource = "checkB.png";

		// IsClosed変更後イベントハンドラ
		private static void OnIsClosedPropertyChanged(BindableObject bindable, bool oldValue, bool newValue)
		{
			var image = bindable as CheckBoxImage;
			if (image == null)
			{
				return;
			}
			// 変化後の真偽値によって表示する画像を決定する。
			if (newValue == true)
				image.Source = trueSource;
			else
				image.Source = falseSource;
		}
	}

	// 値の範囲が0~100のプログレスバー
	public class TaskProgressBar : ProgressBar
	{
		// TaskProgressのBindableProperty
		public readonly static BindableProperty TaskProgressProperty = BindableProperty.Create<TaskProgressBar, int>(
			o => o.TaskProgress,        // getter
			0,						    // 初期値
			BindingMode.TwoWay,         // とりあえずTwoWay
			null,
			OnTaskProgressPropertyChanged); // プロパティ変更イベントハンドラ

		// 追加で実装したプロパティ。
		public int TaskProgress
		{
			get { return (int)GetValue(TaskProgressProperty); }
			set { SetValue(TaskProgressProperty, value); }
		}

		// TaskProgress変更後イベントハンドラ
		private static void OnTaskProgressPropertyChanged(BindableObject bindable, int oldValue, int newValue)
		{
			var taskProgressBar = bindable as TaskProgressBar;
			if (taskProgressBar != null)
			{
				taskProgressBar.Progress = newValue / 100.0;
			}
		}
	}


	// 進捗度を円形のプログレスバーで表示するView
	public class RoundProgressBar : BoxView
	{
		public Color StrokeColor { get; set; }  // 線の色
		public float StrokeWidth { get; set; }  // 線の幅(0(完全な円) ~ 1)

		// 角度(0 ~ 100)。バインディング可能(変更される側のみ)
		private int angle = 0;
		public int Angle
		{
			get { return angle; }
			set
			{
				angle = value;
				base.OnPropertyChanged(nameof(Angle));
			}
		}

		// コンストラクタ
		public RoundProgressBar(int widthRequest = 100, int heightRequest = 100)
		{
			// デフォルト値でサイズとレイアウトを設定
			WidthRequest = widthRequest;
			HeightRequest = heightRequest;
			HorizontalOptions = LayoutOptions.Center;
			VerticalOptions = LayoutOptions.Center;
		}
	}

	public class BicoloredBoxView : BoxView
	{
		public Color LeftColor { get; set; }
		public Color RightColor { get; set; }
		public int ShadowSize { get; set; }
		
		public static readonly BindableProperty RatioProperty = BindableProperty.Create<BicoloredBoxView, int>(p => p.Ratio, 0);
		public int Ratio
		{
			get { return (int)GetValue(RatioProperty); }
			set { SetValue(RatioProperty, value); }
		}

		public BicoloredBoxView()
		{
			WidthRequest = 100;
			HeightRequest = 100;
			ShadowSize = 0;
		}
	}
}
