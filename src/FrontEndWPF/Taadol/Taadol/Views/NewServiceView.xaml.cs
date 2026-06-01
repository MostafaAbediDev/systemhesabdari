using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Taadol.Controls;

namespace Taadol.Views
{
    public partial class NewServiceView : UserControl
    {
        public NewServiceView()
        {
            InitializeComponent();

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

       
        private void OnImageSelected(object sender, RoutedEventArgs e)
        {
            var imagePicker = sender as ImagePickerControl;
            if (imagePicker != null)
            {
                string imagePath = imagePicker.ImagePath;
                            }
        }
        private void OnImageRemoved(object sender, RoutedEventArgs e)
        {
        }
       

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {
                    }
    }
}