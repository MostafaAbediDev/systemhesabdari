// Views/NewProductView.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Taadol.Views
{
    public partial class NewProductView : UserControl
    {
        public NewProductView()
        {
            InitializeComponent();

            TabPricing.Checked += (s, e) =>
            {
                TabInventory.IsChecked = false;
                TabTax.IsChecked = false;
                SwitchTab(0);
            };

            TabInventory.Checked += (s, e) =>
            {
                TabPricing.IsChecked = false;
                TabTax.IsChecked = false;
                SwitchTab(1);
            };

            TabTax.Checked += (s, e) =>
            {
                TabPricing.IsChecked = false;
                TabInventory.IsChecked = false;
                SwitchTab(2);
            };
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
        private void SwitchTab(int index)
        {
            PricingContent.Visibility = index == 0 ? Visibility.Visible : Visibility.Collapsed;
            InventoryContent.Visibility = index == 1 ? Visibility.Visible : Visibility.Collapsed;
            TaxContent.Visibility = index == 2 ? Visibility.Visible : Visibility.Collapsed;
        }
        private void TabTax_Checked(object sender, RoutedEventArgs e)
        {
            if (TabPricing == null || TabInventory == null) return;

            TabPricing.IsChecked = false;
            TabInventory.IsChecked = false;

            PricingContent.Visibility = Visibility.Collapsed;
            InventoryContent.Visibility = Visibility.Collapsed;
            TaxContent.Visibility = Visibility.Visible;
        }





        private void TabInventory_Checked(object sender, RoutedEventArgs e)
        {
            if (TabPricing == null || TabTax == null) return;

            TabPricing.IsChecked = false;
            TabTax.IsChecked = false;

            PricingContent.Visibility = Visibility.Collapsed;
            InventoryContent.Visibility = Visibility.Visible;
            TaxContent.Visibility = Visibility.Collapsed;
        }
        private void TabPricing_Checked(object sender, RoutedEventArgs e)
        {
            if (TabInventory == null || TabTax == null) return;

            // بقیه تب‌ها رو غیرفعال کن
            TabInventory.IsChecked = false;
            TabTax.IsChecked = false;

            // محتوا رو نمایش بده
            PricingContent.Visibility = Visibility.Visible;
            InventoryContent.Visibility = Visibility.Collapsed;
            TaxContent.Visibility = Visibility.Collapsed;
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {

        }

        private void TabInventory_Checked_1(object sender, RoutedEventArgs e)
        {

        }

        private void TabInventory_Checked_2(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged_3(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}