using System;
using System.Windows;
using System.Windows.Controls;
using Taadol.Controls;

namespace Taadol.Views
{
    public partial class NewCompanyView : UserControl
    {
        public NewCompanyView()
        {
            InitializeComponent();
        }

        private void UniqueIdToggle_SelectionChanged(object sender, bool isFirstSelected)
        {
                                }

        private void BranchActive_Changed(object sender, RoutedEventArgs e)
        {
                    }

        private void DatePicker_DateChanged(object sender, RoutedEventArgs e)
        {
            var picker = sender as PersianDatePickerControl;
            if (picker == null || !picker.SelectedDate.HasValue) return;

            DateTime selected = picker.SelectedDate.Value;
            Console.WriteLine($"تاریخ تاسیس: {selected:yyyy/MM/dd}");
        }

        private void OnImageSelected(object sender, RoutedEventArgs e)
        {
                    }

        private void OnImageRemoved(object sender, RoutedEventArgs e)
        {
                    }

        private void ImagePickerControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}