using Xamarin.Forms;

namespace SukimaNote
{
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
}
