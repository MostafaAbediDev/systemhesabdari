using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using Taadol.Controls;

namespace Taadol.Views
{
    public partial class NewPersonView : UserControl
    {
        public NewPersonView()
        {
            InitializeComponent();
    //        var data = new ObservableCollection<CategoryItem>()
    //{
    //    new CategoryItem
    //    {
    //        Title = "هزینه ها",
    //        Children = new ObservableCollection<CategoryItem>
    //        {
    //            new CategoryItem { Title = "اجاره" },
    //            new CategoryItem { Title = "قبوض" }
    //        }
    //    },
    //    new CategoryItem
    //    {
    //        Title = "درآمد",
    //        Children = new ObservableCollection<CategoryItem>
    //        {
    //            new CategoryItem { Title = "حقوق" },
    //            new CategoryItem { Title = "پروژه" }
    //        }
    //    }
    //};

    //        CategorySelector.Items = data;
    //        CategorySelector.Filter(); // مهم 🔥
     }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Toggle_SelectionChanged(object sender, bool isFirstSelected)
        {
            if (isFirstSelected)
            {
            }
            else
            {
            }
        }
        private void MyDatePicker_DateChanged(object sender, RoutedEventArgs e)
        {
            var picker = sender as PersianDatePickerControl;
            if (picker == null || !picker.SelectedDate.HasValue) return;

            DateTime selected = picker.SelectedDate.Value;
            Console.WriteLine(selected.ToString("yyyy/MM/dd"));
        }


        private void Radio_Checked(object sender, RoutedEventArgs e)
        {
            var radio = sender as ToggleRadioControl;
            if (radio == null) return;

        }
        private void OnImageSelected(object sender, RoutedEventArgs e)
        {
        }

        private void OnImageRemoved(object sender, RoutedEventArgs e)
        {
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {
        }

        private void TextBox_TextChanged_3(object sender, TextChangedEventArgs e)
        {
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

            TabInventory.IsChecked = false;
            TabTax.IsChecked = false;

            PricingContent.Visibility = Visibility.Visible;
            InventoryContent.Visibility = Visibility.Collapsed;
            TaxContent.Visibility = Visibility.Collapsed;
        }

        private void ImagePickerControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}