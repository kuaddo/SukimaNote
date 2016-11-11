using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using PCLStorage;
using System.Threading.Tasks;
// TODO: usingの整理をする

namespace SukimaNote
{
	// getのみのプロパティにする
	public class MyColor
	{
		private static string _mainColor1 = "FCF1D3";	// TaskAdd入力部分　"FF9129"
		private static string _mainColor2 = "FFFFFF";	// TaskAdd背景 "FF8119"
		private static string _mainColor3 = "EF7109";
		private static string _subColor = "FFFAFA";

		public static string MainColor1
		{
			get { return _mainColor1; }
		}
		public static string MainColor2
		{
			get { return _mainColor2; }
		}
		public static string MainColor3
		{
			get { return _mainColor3; }
		}
		public static string SubColor
		{
			get { return _subColor; }
		}
	}

	// テーマカラーの設定ページ
	public class ColorSettingPage : ContentPage
	{

	}
}