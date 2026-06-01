using System;
using System.Windows;
using System.Windows.Controls;
using Taadol.Controls;

namespace Taadol.Views
{
    public partial class NewFinancialPeriodView : UserControl
    {
        public NewFinancialPeriodView()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                    }

        private void DatePicker_DateChanged(object sender, RoutedEventArgs e)
        {
            var picker = sender as PersianDatePickerControl;
            if (picker == null || !picker.SelectedDate.HasValue) return;

            DateTime selected = picker.SelectedDate.Value;
            Console.WriteLine($"تاریخ انتخاب‌شده: {selected:yyyy/MM/dd}");
        }

        private void IsCurrentPeriod_Changed(object sender, RoutedEventArgs e)
        {
                    }
    }
}