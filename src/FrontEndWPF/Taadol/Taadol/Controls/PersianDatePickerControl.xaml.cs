using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Taadol.Controls
{
    public partial class PersianDatePickerControl : UserControl
    {
        private readonly PersianCalendar _pc = new PersianCalendar();
        private bool _isLoaded = false;
        private bool _isUpdating = false;

        public static readonly DependencyProperty SelectedDateProperty =
    DependencyProperty.Register(
        nameof(SelectedDate),
        typeof(DateTime?),
        typeof(PersianDatePickerControl),
        new PropertyMetadata(null));

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public static readonly RoutedEvent DateChangedEvent =
    EventManager.RegisterRoutedEvent(
        nameof(DateChanged),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(PersianDatePickerControl));

        public event RoutedEventHandler DateChanged
        {
            add => AddHandler(DateChangedEvent, value);
            remove => RemoveHandler(DateChangedEvent, value);
        }

        public PersianDatePickerControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            _isUpdating = true;
            try
            {
                var now = DateTime.Now;
                PART_Year.Text = _pc.GetYear(now).ToString();
                PART_Month.Text = _pc.GetMonth(now).ToString("D2");
                PART_Day.Text = _pc.GetDayOfMonth(now).ToString("D2");
            }
            finally
            {
                _isUpdating = false;
            }
            TryUpdateDate();
        }

        private void YearTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded || _isUpdating) return;
            FilterNumeric(PART_Year, 4);
            TryUpdateDate();
        }

        private void MonthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded || _isUpdating) return;
            FilterNumeric(PART_Month, 2);
            TryUpdateDate();
        }

        private void DayTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded || _isUpdating) return;
            FilterNumeric(PART_Day, 2);
            TryUpdateDate();
        }

        private void DateTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var txt = sender as TextBox;
            if (txt != null) txt.SelectAll();
        }

        private void TryUpdateDate()
        {
            if (!_isLoaded) return;

            int year = 0;
            int month = 0;
            int day = 0;

            bool yearOk = int.TryParse(NormalizeDigits(PART_Year.Text), out year);
            bool monthOk = int.TryParse(NormalizeDigits(PART_Month.Text), out month);
            bool dayOk = int.TryParse(NormalizeDigits(PART_Day.Text), out day);
            if (!yearOk || !monthOk || !dayOk)
            {
                SelectedDate = null;
                return;
            }

            try
            {

                SelectedDate = _pc.ToDateTime(year, month, day, 0, 0, 0, 0);
                RaiseEvent(new RoutedEventArgs(DateChangedEvent, this));
            }
            catch
            {
                SelectedDate = null;
            }
        }
        private string NormalizeDigits(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            return input
                .Replace("۰", "0")
                .Replace("۱", "1")
                .Replace("۲", "2")
                .Replace("۳", "3")
                .Replace("۴", "4")
                .Replace("۵", "5")
                .Replace("۶", "6")
                .Replace("۷", "7")
                .Replace("۸", "8")
                .Replace("۹", "9")
                .Replace("٠", "0")
                .Replace("١", "1")
                .Replace("٢", "2")
                .Replace("٣", "3")
                .Replace("٤", "4")
                .Replace("٥", "5")
                .Replace("٦", "6")
                .Replace("٧", "7")
                .Replace("٨", "8")
                .Replace("٩", "9");
        }

        private void FilterNumeric(TextBox txt, int maxLen)
        {
            if (txt == null) return;

            string result = "";
            foreach (char c in txt.Text)
                if (char.IsDigit(c)) result += c;

            if (result.Length > maxLen)
                result = result.Substring(0, maxLen);

            if (result != txt.Text)
            {
                int caret = txt.CaretIndex;
                txt.Text = result;
                txt.CaretIndex = Math.Min(caret, result.Length);
            }
        }
    }
}
