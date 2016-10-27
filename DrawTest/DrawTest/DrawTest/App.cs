using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace DrawTest
{
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            MainPage = GetMainPage();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public static Page GetMainPage()
        {
            var layout = new StackLayout();

            var boxView =
                new ExtendedBoxView
                {
                    StrokeColor = Color.White,
                    FillColor = Color.Black,
                };

            layout.Children.Add(boxView); // 丸

            // Radiusの値をスライダーで変更する
            var sliderAngle = new Slider { Maximum = 100, Minimum = 0, };
            var labelAngle = new Label();
            sliderAngle.ValueChanged +=
                (s, a) =>
                {
                    boxView.Angle = (int)sliderAngle.Value;
                    labelAngle.Text = string.Format($"Angle = {boxView.Angle}");
                };
            layout.Children.Add(labelAngle);
            layout.Children.Add(sliderAngle);

            // BorderWidthの値をスライダーで変更する
            var sliderBorderWidth = new Slider { Maximum = 1.0, Minimum = 0.1, };
            var labelBorderWidth = new Label();
            sliderBorderWidth.ValueChanged +=
                (s, a) =>
                {
                    boxView.BorderWidth = (float)sliderBorderWidth.Value;
                    labelBorderWidth.Text = string.Format($"BorderWidth = {boxView.BorderWidth}");
                };
            layout.Children.Add(labelBorderWidth);
            layout.Children.Add(sliderBorderWidth);

            return new ContentPage
            {
                // iPhone用に上に余白を取る
                Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0),
                Content = layout,
            };
        }
    }

    public class ExtendedBoxView : BoxView
    {
        // 境界の色
        public Color StrokeColor { get; set; }

        // 塗りつぶし色
        public Color FillColor { get; set; }

        // 境界の幅（0（線） ~ 1（完全な円）で設定）
        private float borderWidth = 1.0f;
        public float BorderWidth
        {
            get
            {
                return borderWidth;
            }
            set
            {
                base.OnPropertyChanged("BorderWidth");
                borderWidth = 1.0f - value;
            }
        }

        // 角丸（0 ~ 50%）
        private int angle = 0;
        public int Angle
        {
            get
            {
                return angle;
            }
            set
            {
                base.OnPropertyChanged("Radius");
                angle = 360 * value / 100;
            }
        }


        // コンストラクタ
        public ExtendedBoxView(int widthRequest = 100, int heightRequest = 100)
        {
            // デフォルト値でサイズとレイアウトを設定
            WidthRequest = widthRequest;
            HeightRequest = heightRequest;
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;
        }
    }
}
