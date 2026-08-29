using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Taadol.Controls
{
    public partial class PersianDatePickerControl : UserControl
    {
        private readonly PersianCalendar _persianCalendar = new PersianCalendar();
        private int _displayedYear;
        private int _displayedMonth;
        private bool _isInternalChange = false;

        // اسامی ماه‌های شمسی
        private static readonly string[] MonthNames = {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };

        #region Dependency Properties (پراپرتی‌های بایندینگ)

        // ۱. پراپرتی تاریخ میلادی (برای ذخیره در دیتابیس)
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(nameof(SelectedDate), typeof(DateTime?), typeof(PersianDatePickerControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateChanged));

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public event RoutedEventHandler DateChanged;

        private void RaiseDateChanged()
        {
            DateChanged?.Invoke(this, new RoutedEventArgs());
        }

        // ۲. پراپرتی رشته تاریخ شمسی (مانند ۱۴۰۳/۰۷/۲۵ برای نمایش یا فیلتر)
        public static readonly DependencyProperty PersianDateStringProperty =
            DependencyProperty.Register(nameof(PersianDateString), typeof(string), typeof(PersianDatePickerControl),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPersianDateStringChanged));

        public string PersianDateString
        {
            get => (string)GetValue(PersianDateStringProperty);
            set => SetValue(PersianDateStringProperty, value);
        }

        #endregion

        public PersianDatePickerControl()
        {
            InitializeComponent();
            InitializeCurrentDate();

            // رجیستر کردن رویدادهای کیبورد برای ناوبری سریع‌تر
            YearTextBox.PreviewKeyDown += DateTextBox_PreviewKeyDown;
            MonthTextBox.PreviewKeyDown += DateTextBox_PreviewKeyDown;
            DayTextBox.PreviewKeyDown += DateTextBox_PreviewKeyDown;

            // هندل کردن بازنشانی فیلدها هنگام خروج فوکوس (Padding با صفر)
            MonthTextBox.LostFocus += MonthTextBox_LostFocus;
            DayTextBox.LostFocus += DayTextBox_LostFocus;
            YearTextBox.LostFocus += YearTextBox_LostFocus;
        }

        private void InitializeCurrentDate()
        {
            var now = DateTime.Now;
            _displayedYear = _persianCalendar.GetYear(now);
            _displayedMonth = _persianCalendar.GetMonth(now);
        }

        /// <summary>
        /// متد عمومی برای پاک کردن و بازنشانی کامل دیت‌پیکر از بیرون (رفع خطای کامپایل)
        /// </summary>
        public void Clear()
        {
            _isInternalChange = true;
            SelectedDate = null;
            PersianDateString = string.Empty;
            ClearTextBoxes();
            HideError();
            _isInternalChange = false;
        }

        #region Callbacks (هماهنگ‌سازی دو طرفه بایندینگ)

        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PersianDatePickerControl)d;
            if (control._isInternalChange) return;

            control._isInternalChange = true;
            if (e.NewValue is DateTime dt)
            {
                int py = control._persianCalendar.GetYear(dt);
                int pm = control._persianCalendar.GetMonth(dt);
                int pd = control._persianCalendar.GetDayOfMonth(dt);

                control.YearTextBox.Text = ToPersianDigits(py.ToString("D4"));
                control.MonthTextBox.Text = ToPersianDigits(pm.ToString("D2"));
                control.DayTextBox.Text = ToPersianDigits(pd.ToString("D2"));
                control.PersianDateString = $"{py:D4}/{pm:D2}/{pd:D2}";
                control.HideError();
            }
            else
            {
                control.ClearTextBoxes();
                control.PersianDateString = string.Empty;
            }
            control._isInternalChange = false;
        }

        private static void OnPersianDateStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PersianDatePickerControl)d;
            if (control._isInternalChange) return;

            string newStr = e.NewValue as string;
            if (string.IsNullOrWhiteSpace(newStr))
            {
                control._isInternalChange = true;
                control.SelectedDate = null;
                control.ClearTextBoxes();
                control._isInternalChange = false;
                return;
            }

            var match = Regex.Match(newStr, @"^(\d{4})/(\d{2})/(\d{2})$");
            if (match.Success)
            {
                int y = int.Parse(ToEnglishDigits(match.Groups[1].Value));
                int m = int.Parse(ToEnglishDigits(match.Groups[2].Value));
                int d1 = int.Parse(ToEnglishDigits(match.Groups[3].Value));

                if (control.IsValidPersianDate(y, m, d1))
                {
                    control._isInternalChange = true;
                    control.YearTextBox.Text = ToPersianDigits(y.ToString("D4"));
                    control.MonthTextBox.Text = ToPersianDigits(m.ToString("D2"));
                    control.DayTextBox.Text = ToPersianDigits(d1.ToString("D2"));
                    control.SelectedDate = control._persianCalendar.ToDateTime(y, m, d1, 0, 0, 0, 0);
                    control._isInternalChange = false;
                    control.HideError();
                    control.RaiseDateChanged();
                    return;
                }
            }
            control.ShowError("تاریخ وارد شده نامعتبر است");
        }

        #endregion

        #region TextBoxes Navigation & Validation (مدیریت کیبورد و فوکوس)

        private void YearTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NormalizeTextBoxDigits(YearTextBox);
            if (YearTextBox.Text.Length == 4)
            {
                MonthTextBox.Focus();
                MonthTextBox.SelectAll();
                UpdateFromTextBoxes();
            }
        }

        private void MonthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NormalizeTextBoxDigits(MonthTextBox);
            if (MonthTextBox.Text.Length == 2)
            {
                DayTextBox.Focus();
                DayTextBox.SelectAll();
                UpdateFromTextBoxes();
            }
            else if (MonthTextBox.Text.Length == 1 && int.TryParse(MonthTextBox.Text, out int m) && m > 1)
            {
                MonthTextBox.Text = "0" + m;
                DayTextBox.Focus();
                DayTextBox.SelectAll();
                UpdateFromTextBoxes();
            }
        }

        private void DayTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NormalizeTextBoxDigits(DayTextBox);
            if (DayTextBox.Text.Length == 2)
            {
                UpdateFromTextBoxes();
            }
            else if (DayTextBox.Text.Length == 1 && int.TryParse(DayTextBox.Text, out int d) && d > 3)
            {
                DayTextBox.Text = "0" + d;
                UpdateFromTextBoxes();
            }
        }

        private void DateTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            if (e.Key == Key.Back && string.IsNullOrEmpty(textBox.Text))
            {
                if (textBox == DayTextBox)
                {
                    MonthTextBox.Focus();
                    MonthTextBox.Select(MonthTextBox.Text.Length, 0);
                    e.Handled = true;
                }
                else if (textBox == MonthTextBox)
                {
                    YearTextBox.Focus();
                    YearTextBox.Select(YearTextBox.Text.Length, 0);
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Left && textBox.CaretIndex == 0)
            {
                if (textBox == DayTextBox)
                {
                    MonthTextBox.Focus();
                    MonthTextBox.CaretIndex = MonthTextBox.Text.Length;
                    e.Handled = true;
                }
                else if (textBox == MonthTextBox)
                {
                    YearTextBox.Focus();
                    YearTextBox.CaretIndex = YearTextBox.Text.Length;
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Right && textBox.CaretIndex == textBox.Text.Length)
            {
                if (textBox == YearTextBox)
                {
                    MonthTextBox.Focus();
                    MonthTextBox.CaretIndex = 0;
                    e.Handled = true;
                }
                else if (textBox == MonthTextBox)
                {
                    DayTextBox.Focus();
                    DayTextBox.CaretIndex = 0;
                    e.Handled = true;
                }
            }
        }

        private void DateTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = e.Text.Any(character => !char.IsDigit(character));
        }

        private static string ToPersianDigits(string value)
        {
            return value
                .Replace('0', '۰').Replace('1', '۱').Replace('2', '۲')
                .Replace('3', '۳').Replace('4', '۴').Replace('5', '۵')
                .Replace('6', '۶').Replace('7', '۷').Replace('8', '۸')
                .Replace('9', '۹');
        }

        private static string ToEnglishDigits(string value)
        {
            return value
                .Replace('۰', '0').Replace('۱', '1').Replace('۲', '2')
                .Replace('۳', '3').Replace('۴', '4').Replace('۵', '5')
                .Replace('۶', '6').Replace('۷', '7').Replace('۸', '8')
                .Replace('۹', '9')
                .Replace('٠', '0').Replace('١', '1').Replace('٢', '2')
                .Replace('٣', '3').Replace('٤', '4').Replace('٥', '5')
                .Replace('٦', '6').Replace('٧', '7').Replace('٨', '8')
                .Replace('٩', '9');
        }

        private static void NormalizeTextBoxDigits(TextBox textBox)
        {
            var normalized = ToEnglishDigits(textBox.Text);
            if (normalized == textBox.Text)
                return;

            var caretIndex = textBox.CaretIndex;
            textBox.Text = normalized;
            textBox.CaretIndex = Math.Min(caretIndex, normalized.Length);
        }


        private void YearTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(YearTextBox.Text) && YearTextBox.Text.Length < 4)
            {
                if (int.TryParse(YearTextBox.Text, out int y))
                {
                    if (y < 100) y += 1400;
                    YearTextBox.Text = ToPersianDigits(y.ToString("D4"));
                    UpdateFromTextBoxes();
                }
            }
        }

        private void MonthTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MonthTextBox.Text) && MonthTextBox.Text.Length == 1)
            {
                MonthTextBox.Text = ToPersianDigits("0" + ToEnglishDigits(MonthTextBox.Text));
                UpdateFromTextBoxes();
            }
        }

        private void DayTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(DayTextBox.Text) && DayTextBox.Text.Length == 1)
            {
                DayTextBox.Text = ToPersianDigits("0" + ToEnglishDigits(DayTextBox.Text));
                UpdateFromTextBoxes();
            }
        }

        #endregion

        #region Core Processing Logic

        private void UpdateFromTextBoxes()
        {
            if (string.IsNullOrWhiteSpace(YearTextBox.Text) ||
                string.IsNullOrWhiteSpace(MonthTextBox.Text) ||
                string.IsNullOrWhiteSpace(DayTextBox.Text))
            {
                return;
            }

            if (int.TryParse(ToEnglishDigits(YearTextBox.Text), out int y) &&
                int.TryParse(ToEnglishDigits(MonthTextBox.Text), out int m) &&
                int.TryParse(ToEnglishDigits(DayTextBox.Text), out int d))
            {
                if (IsValidPersianDate(y, m, d))
                {
                    HideError();
                    _isInternalChange = true;
                    SelectedDate = _persianCalendar.ToDateTime(y, m, d, 0, 0, 0, 0);
                    PersianDateString = ToPersianDigits($"{y:D4}/{m:D2}/{d:D2}");
                    _isInternalChange = false;
                    RaiseDateChanged();
                }
                else
                {
                    ShowError("تاریخ نامعتبر است");
                }
            }
        }

        private bool IsValidPersianDate(int year, int month, int day)
        {
            if (year < 1200 || year > 1500) return false;
            if (month < 1 || month > 12) return false;
            if (day < 1) return false;

            try
            {
                int daysInMonth = _persianCalendar.GetDaysInMonth(year, month);
                return day <= daysInMonth;
            }
            catch
            {
                return false;
            }
        }

        private void ClearTextBoxes()
        {
            YearTextBox.Text = string.Empty;
            MonthTextBox.Text = string.Empty;
            DayTextBox.Text = string.Empty;
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            ErrorMessage.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Calendar PopUp Renderer (رندر کردن پاپ‌آپ تقویم شمسی)

        private void CalendarIcon_Click(object sender, MouseButtonEventArgs e)
        {
            if (SelectedDate != null)
            {
                _displayedYear = _persianCalendar.GetYear(SelectedDate.Value);
                _displayedMonth = _persianCalendar.GetMonth(SelectedDate.Value);
            }
            else
            {
                InitializeCurrentDate();
            }

            RenderCalendar();
            CalendarPopup.IsOpen = true;
        }

        private void RenderCalendar()
        {
            CalendarDaysGrid.Children.Clear();
            CalendarMonthLabel.Text = $"{MonthNames[_displayedMonth - 1]} {_displayedYear}";

            DateTime firstOfMonth = _persianCalendar.ToDateTime(_displayedYear, _displayedMonth, 1, 0, 0, 0, 0);
            DayOfWeek dow = _persianCalendar.GetDayOfWeek(firstOfMonth);

            int offset = ((int)dow + 1) % 7;

            for (int i = 0; i < offset; i++)
            {
                CalendarDaysGrid.Children.Add(new Border());
            }

            int totalDays = _persianCalendar.GetDaysInMonth(_displayedYear, _displayedMonth);
            var today = DateTime.Now;
            int todayY = _persianCalendar.GetYear(today);
            int todayM = _persianCalendar.GetMonth(today);
            int todayD = _persianCalendar.GetDayOfMonth(today);

            int selY = SelectedDate.HasValue ? _persianCalendar.GetYear(SelectedDate.Value) : 0;
            int selM = SelectedDate.HasValue ? _persianCalendar.GetMonth(SelectedDate.Value) : 0;
            int selD = SelectedDate.HasValue ? _persianCalendar.GetDayOfMonth(SelectedDate.Value) : 0;

            for (int day = 1; day <= totalDays; day++)
            {
                var dayButton = new Button
                {
                    Content = day.ToString(),
                    Style = (Style)Resources["CalendarDayButtonStyle"],
                    Tag = day
                };

                if (_displayedYear == selY && _displayedMonth == selM && day == selD)
                {
                    dayButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2667FF"));
                    dayButton.Foreground = Brushes.White;
                }
                else if (_displayedYear == todayY && _displayedMonth == todayM && day == todayD)
                {
                    dayButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2667FF"));
                    dayButton.BorderThickness = new Thickness(1);
                    dayButton.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2667FF"));
                }

                dayButton.Click += DayButton_Click;
                CalendarDaysGrid.Children.Add(dayButton);
            }
        }

        private void DayButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int day)
            {
                _isInternalChange = true;
                SelectedDate = _persianCalendar.ToDateTime(_displayedYear, _displayedMonth, day, 0, 0, 0, 0);
                PersianDateString = ToPersianDigits($"{_displayedYear:D4}/{_displayedMonth:D2}/{day:D2}");
                _isInternalChange = false;
                RaiseDateChanged();

                YearTextBox.Text = ToPersianDigits(_displayedYear.ToString("D4"));
                MonthTextBox.Text = ToPersianDigits(_displayedMonth.ToString("D2"));
                DayTextBox.Text = ToPersianDigits(day.ToString("D2"));

                HideError();
                CalendarPopup.IsOpen = false;
            }
        }

        #endregion

        #region Calendar Navigation & Today

        private void CalendarPrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (_displayedMonth == 1)
            {
                _displayedMonth = 12;
                _displayedYear--;
            }
            else
            {
                _displayedMonth--;
            }
            RenderCalendar();
        }

        private void CalendarNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_displayedMonth == 12)
            {
                _displayedMonth = 1;
                _displayedYear++;
            }
            else
            {
                _displayedMonth++;
            }
            RenderCalendar();
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            var now = DateTime.Now;
            int y = _persianCalendar.GetYear(now);
            int m = _persianCalendar.GetMonth(now);
            int d = _persianCalendar.GetDayOfMonth(now);

            _isInternalChange = true;
            SelectedDate = now.Date;
            PersianDateString = ToPersianDigits($"{y:D4}/{m:D2}/{d:D2}");
            _isInternalChange = false;
            RaiseDateChanged();

            YearTextBox.Text = ToPersianDigits(y.ToString("D4"));
            MonthTextBox.Text = ToPersianDigits(m.ToString("D2"));
            DayTextBox.Text = ToPersianDigits(d.ToString("D2"));

            HideError();
            CalendarPopup.IsOpen = false;
        }

        private void CalendarLabel_Click(object sender, MouseButtonEventArgs e)
        {
        }

        #endregion
    }
}