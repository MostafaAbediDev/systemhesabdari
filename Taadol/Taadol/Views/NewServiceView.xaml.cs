using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Taadol.Views
{
    public partial class NewServiceView : UserControl
    {
        public NewServiceView()
        {
            InitializeComponent();

            // تنظیم تب‌ها
            TabPricing.Checked += (s, e) =>
            {
                TabTax.IsChecked = false;
                SwitchTab(0);
            };

            TabTax.Checked += (s, e) =>
            {
                TabPricing.IsChecked = false;
                SwitchTab(1);
            };
        }

        private void SwitchTab(int index)
        {
            PricingContent.Visibility = index == 0 ? Visibility.Visible : Visibility.Collapsed;
            TaxContent.Visibility = index == 1 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TabPricing_Checked(object sender, RoutedEventArgs e)
        {
            if (TabTax == null) return;
            TabTax.IsChecked = false;
            PricingContent.Visibility = Visibility.Visible;
            TaxContent.Visibility = Visibility.Collapsed;
        }

        private void TabTax_Checked(object sender, RoutedEventArgs e)
        {
            if (TabPricing == null) return;
            TabPricing.IsChecked = false;
            PricingContent.Visibility = Visibility.Collapsed;
            TaxContent.Visibility = Visibility.Visible;
        }

        private void BtnAuto_Checked(object sender, RoutedEventArgs e)
        {
            if (BtnManual == null || SliderTransform == null) return;

            BtnManual.IsChecked = false;

            var anim = new DoubleAnimation
            {
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            SliderTransform.BeginAnimation(TranslateTransform.XProperty, anim);
        }

        private void BtnManual_Checked(object sender, RoutedEventArgs e)
        {
            if (BtnAuto == null || SliderTransform == null || SliderBg == null) return;

            BtnAuto.IsChecked = false;

            double sliderWidth = SliderBg.ActualWidth;

            var anim = new DoubleAnimation
            {
                To = sliderWidth,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };
            SliderTransform.BeginAnimation(TranslateTransform.XProperty, anim);
        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {
            // برای بارکد
        }
    }
}