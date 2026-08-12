using System;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        private static readonly string[] PersianMonthNames =
        {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };

        // ===== وضعیت تقویم پاپ‌آپ =====
        private int _calYear;
        private int _calMonth;
        private int _calSelYear;
        private int _calSelMonth;
        private int _calSelDay; // 0 = هیچ روزی انتخاب نشده

        public PersianDatePickerControl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            CalendarPopup.PlacementTarget = CalendarIconBorder;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _isLoaded = true;
        }

        /// <summary>پاک کردن کامل فیلدها (سال/ماه/روز) و مقدار SelectedDate.</summary>
        public void Clear()
        {
            _isUpdating = true;
            try
            {
                YearTextBox.Text = "";
                MonthTextBox.Text = "";
                DayTextBox.Text = "";
            }
            finally
            {
                _isUpdating = false;
            }
            SetCurrentValue(SelectedDateProperty, null);
        }

        private enum CalendarMode { Days, Months, Years }
        private CalendarMode _calMode = CalendarMode.Days;
        private int _calDecadeStart; // اولین سال نمایش‌داده‌شده در نمای سال‌ها

        // ===== حالت ظاهری آیتم‌های تقویم (برای هاورِ آگاه از انتخاب) =====
        private enum CalItemHighlight { Normal, Today, Selected }

        private sealed class CalItemState
        {
            public CalItemHighlight Highlight;
            public Brush BaseBg;
            public Brush BaseFg;
            public Brush BaseBorder;
            public Thickness BaseBorderThickness;
            public FontWeight BaseWeight;
        }

        private static Brush C(string hex) => new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));

        /// <summary>
        /// ساخت دکمه‌ی روز/ماه/سال با هاورِ کد محور:
        /// آیتم انتخاب‌شده روی هاور آبیِ مشخص می‌ماند و متنش سفید می‌ماند.
        /// </summary>
        private Button CreateCalendarItem(string content, double fontSize, CalItemHighlight highlight)
        {
            var btn = new Button
            {
                Style = (Style)FindResource("CalendarDayButtonStyle"),
                Content = content,
                FontSize = fontSize
            };

            var st = new CalItemState { Highlight = highlight };
            switch (highlight)
            {
                case CalItemHighlight.Selected:
                    st.BaseBg = C("#2667FF");
                    st.BaseFg = Brushes.White;
                    st.BaseBorder = Brushes.Transparent;
                    st.BaseBorderThickness = new Thickness(1);
                    st.BaseWeight = FontWeights.Bold;
                    break;
                case CalItemHighlight.Today:
                    st.BaseBg = C("#E6EEFF");
                    st.BaseFg = C("#2667FF");
                    st.BaseBorder = C("#2667FF");
                    st.BaseBorderThickness = new Thickness(1);
                    st.BaseWeight = FontWeights.Bold;
                    break;
                default:
                    st.BaseBg = Brushes.Transparent;
                    st.BaseFg = C("#404040");
                    st.BaseBorder = Brushes.Transparent;
                    st.BaseBorderThickness = new Thickness(1);
                    st.BaseWeight = FontWeights.Normal;
                    break;
            }

            ApplyBaseState(btn, st);
            btn.Tag = st;
            btn.MouseEnter += CalItem_MouseEnter;
            btn.MouseLeave += CalItem_MouseLeave;
            return btn;
        }

        private static void ApplyBaseState(Button btn, CalItemState st)
        {
            btn.Background = st.BaseBg;
            btn.Foreground = st.BaseFg;
            btn.BorderBrush = st.BaseBorder;
            btn.BorderThickness = st.BaseBorderThickness;
            btn.FontWeight = st.BaseWeight;
        }

        private void CalItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!(sender is Button btn) || !(btn.Tag is CalItemState st)) return;

            btn.Background = st.Highlight == CalItemHighlight.Selected ? C("#4A80FF")
                            : st.Highlight == CalItemHighlight.Today ? C("#D7E4FF")
                            : C("#E6EEFF");
        }

        private void CalItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!(sender is Button btn) || !(btn.Tag is CalItemState st)) return;
            ApplyBaseState(btn, st);
        }

        private void CalendarIcon_Click(object sender, MouseButtonEventArgs e)
        {
            // مقدار اولیه از فیلدها یا تاریخ امروز
            int y = 0, m = 0, d = 0;
            bool hasDate = int.TryParse(NormalizeDigits(YearTextBox.Text), out y)
                           && int.TryParse(NormalizeDigits(MonthTextBox.Text), out m)
                           && int.TryParse(NormalizeDigits(DayTextBox.Text), out d)
                           && m >= 1 && m <= 12 && d >= 1 && d <= 31;

            var now = DateTime.Now;
            int ty = _pc.GetYear(now);
            int tm = _pc.GetMonth(now);

            if (hasDate)
            {
                _calYear = y; _calMonth = m;
                _calSelYear = y; _calSelMonth = m; _calSelDay = d;
            }
            else
            {
                _calYear = ty; _calMonth = tm;
                _calSelDay = 0;
            }

            _calMode = CalendarMode.Days;
            ShowView(CalendarMode.Days);
            CalendarPopup.IsOpen = true;
        }

        /// <summary>تعویض نمای تقویم (روزها / ماه‌ها / سال‌ها).</summary>
        private void ShowView(CalendarMode mode)
        {
            _calMode = mode;
            CalendarDaysGrid.Visibility = mode == CalendarMode.Days ? Visibility.Visible : Visibility.Collapsed;
            CalendarMonthsGrid.Visibility = mode == CalendarMode.Months ? Visibility.Visible : Visibility.Collapsed;
            CalendarYearsGrid.Visibility = mode == CalendarMode.Years ? Visibility.Visible : Visibility.Collapsed;
            // در نماهای ماه/سال، ردیف روزهای هفته مخفی می‌شود
            // (Hidden نه Collapsed) تا فضایش حفظ شود و ارتفاع پاپ‌آپ کم نشود.
            CalendarWeekdayRow.Visibility = mode == CalendarMode.Days ? Visibility.Visible : Visibility.Hidden;
            // دکمه «امروز» در همه‌ی نماها ثابت و قابل‌کلیک می‌ماند.
            CalendarTodayButton.Visibility = Visibility.Visible;

            switch (mode)
            {
                case CalendarMode.Days: BuildCalendar(); break;
                case CalendarMode.Months: BuildMonths(); break;
                case CalendarMode.Years: BuildYears(); break;
            }
        }

        private void UpdateHeader()
        {
            switch (_calMode)
            {
                case CalendarMode.Days:
                    CalendarMonthLabel.Text = $"{PersianMonthNames[_calMonth - 1]} {ToPersianDigits(_calYear.ToString())}";
                    break;
                case CalendarMode.Months:
                    CalendarMonthLabel.Text = ToPersianDigits(_calYear.ToString());
                    break;
                case CalendarMode.Years:
                    CalendarMonthLabel.Text = $"{ToPersianDigits(_calDecadeStart.ToString())} تا {ToPersianDigits((_calDecadeStart + 11).ToString())}";
                    break;
            }
        }

        /// <summary>ساخت شبکه‌ی روزهای ماه جاری تقویم.</summary>
        private void BuildCalendar()
        {
            CalendarDaysGrid.Children.Clear();
            UpdateHeader();

            var now = DateTime.Now;
            int ty = _pc.GetYear(now);
            int tm = _pc.GetMonth(now);
            int td = _pc.GetDayOfMonth(now);

            // شنبه = 0
            int firstDayIndex = ((int)_pc.ToDateTime(_calYear, _calMonth, 1, 0, 0, 0, 0).DayOfWeek + 1) % 7;
            int daysInMonth = _pc.GetDaysInMonth(_calYear, _calMonth);

            for (int i = 0; i < firstDayIndex; i++)
                CalendarDaysGrid.Children.Add(new Border());

            for (int day = 1; day <= daysInMonth; day++)
            {
                var highlight = CalItemHighlight.Normal;
                if (day == _calSelDay && _calMonth == _calSelMonth && _calYear == _calSelYear)
                    highlight = CalItemHighlight.Selected;
                else if (day == td && _calMonth == tm && _calYear == ty)
                    highlight = CalItemHighlight.Today;

                var btn = CreateCalendarItem(ToPersianDigits(day.ToString()), 13, highlight);
                int capturedDay = day;
                btn.Click += (s, e2) => SelectCalendarDay(capturedDay);
                CalendarDaysGrid.Children.Add(btn);
            }
        }

        /// <summary>ساخت شبکه‌ی ۱۲ ماه.</summary>
        private void BuildMonths()
        {
            CalendarMonthsGrid.Children.Clear();
            UpdateHeader();

            var now = DateTime.Now;
            int ty = _pc.GetYear(now);
            int tm = _pc.GetMonth(now);

            for (int m = 1; m <= 12; m++)
            {
                var highlight = CalItemHighlight.Normal;
                if (m == _calSelMonth && _calYear == _calSelYear)
                    highlight = CalItemHighlight.Selected;
                else if (m == tm && _calYear == ty)
                    highlight = CalItemHighlight.Today;

                var btn = CreateCalendarItem(PersianMonthNames[m - 1], 12, highlight);
                int capturedMonth = m;
                btn.Click += (s, e2) =>
                {
                    _calMonth = capturedMonth;
                    ShowView(CalendarMode.Days);
                };
                CalendarMonthsGrid.Children.Add(btn);
            }
        }

        /// <summary>ساخت شبکه‌ی ۱۲ سال (از _calDecadeStart تا +۱۱).</summary>
        private void BuildYears()
        {
            CalendarYearsGrid.Children.Clear();
            UpdateHeader();

            var now = DateTime.Now;
            int ty = _pc.GetYear(now);

            for (int y = _calDecadeStart; y <= _calDecadeStart + 11; y++)
            {
                var highlight = CalItemHighlight.Normal;
                if (y == _calSelYear)
                    highlight = CalItemHighlight.Selected;
                else if (y == ty)
                    highlight = CalItemHighlight.Today;

                var btn = CreateCalendarItem(ToPersianDigits(y.ToString()), 13, highlight);
                int capturedYear = y;
                btn.Click += (s, e2) =>
                {
                    _calYear = capturedYear;
                    ShowView(CalendarMode.Months);
                };
                CalendarYearsGrid.Children.Add(btn);
            }
        }

        /// <summary>انتخاب یک روز: فیلدها پر می‌شوند و تقویم بسته می‌شود.</summary>
        private void SelectCalendarDay(int day)
        {
            _isUpdating = true;
            try
            {
                YearTextBox.Text = ToPersianDigits(_calYear.ToString());
                MonthTextBox.Text = ToPersianDigits(_calMonth.ToString("D2"));
                DayTextBox.Text = ToPersianDigits(day.ToString("D2"));
            }
            finally
            {
                _isUpdating = false;
            }
            TryUpdateDate();
            CalendarPopup.IsOpen = false;
        }

        /// <summary>
        /// کلیک روی هدر: روزها ← ماه‌ها ← سال‌ها، و از سال‌ها دوباره به روزها (صفحه‌ی اول) برمی‌گردد.
        /// </summary>
        private void CalendarLabel_Click(object sender, MouseButtonEventArgs e)
        {
            if (_calMode == CalendarMode.Days)
            {
                ShowView(CalendarMode.Months);
            }
            else if (_calMode == CalendarMode.Months)
            {
                _calDecadeStart = _calYear - 6;
                if (_calDecadeStart < 1) _calDecadeStart = 1;
                ShowView(CalendarMode.Years);
            }
            else if (_calMode == CalendarMode.Years)
            {
                ShowView(CalendarMode.Days);
            }
        }

        private void CalendarPrevButton_Click(object sender, RoutedEventArgs e)
        {
            switch (_calMode)
            {
                case CalendarMode.Days:
                    _calMonth--;
                    if (_calMonth < 1)
                    {
                        _calMonth = 12;
                        _calYear--;
                    }
                    if (_calYear < 1) _calYear = 1;
                    BuildCalendar();
                    break;
                case CalendarMode.Months:
                    _calYear--;
                    if (_calYear < 1) _calYear = 1;
                    BuildMonths();
                    break;
                case CalendarMode.Years:
                    _calDecadeStart -= 12;
                    if (_calDecadeStart < 1) _calDecadeStart = 1;
                    BuildYears();
                    break;
            }
        }

        private void CalendarNextButton_Click(object sender, RoutedEventArgs e)
        {
            switch (_calMode)
            {
                case CalendarMode.Days:
                    _calMonth++;
                    if (_calMonth > 12)
                    {
                        _calMonth = 1;
                        _calYear++;
                    }
                    BuildCalendar();
                    break;
                case CalendarMode.Months:
                    _calYear++;
                    BuildMonths();
                    break;
                case CalendarMode.Years:
                    _calDecadeStart += 12;
                    BuildYears();
                    break;
            }
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            _isUpdating = true;
            try
            {
                var now = DateTime.Now;
                YearTextBox.Text = ToPersianDigits(_pc.GetYear(now).ToString());
                MonthTextBox.Text = ToPersianDigits(_pc.GetMonth(now).ToString("D2"));
                DayTextBox.Text = ToPersianDigits(_pc.GetDayOfMonth(now).ToString("D2"));
            }
            finally
            {
                _isUpdating = false;
            }
            TryUpdateDate();
            CalendarPopup.IsOpen = false;
        }

        private void YearTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded || _isUpdating) return;
            FilterNumeric(YearTextBox, 4);
            TryUpdateDate();
        }

        private void MonthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded || _isUpdating) return;
            FilterNumeric(MonthTextBox, 2);
            TryUpdateDate();
        }

        private void DayTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isLoaded || _isUpdating) return;
            FilterNumeric(DayTextBox, 2);
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

            bool yearOk = int.TryParse(NormalizeDigits(YearTextBox.Text), out year);
            bool monthOk = int.TryParse(NormalizeDigits(MonthTextBox.Text), out month);
            bool dayOk = int.TryParse(NormalizeDigits(DayTextBox.Text), out day);
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

        /// <summary>
        /// فقط رقم (فارسی/انگلیسی/عربی) باقی می‌ماند و نمایش همیشه با ارقام فارسی است.
        /// </summary>
        private void FilterNumeric(TextBox txt, int maxLen)
        {
            if (txt == null) return;

            string result = "";
            foreach (char c in txt.Text)
                if (char.IsDigit(c)) result += c;

            if (result.Length > maxLen)
                result = result.Substring(0, maxLen);

            result = ToPersianDigits(result);

            if (result != txt.Text)
            {
                int caret = txt.CaretIndex;
                txt.Text = result;
                txt.CaretIndex = Math.Min(caret, result.Length);
            }
        }

        /// <summary>رقم‌های انگلیسی و عربی را به رقم فارسی (۰-۹) تبدیل می‌کند؛ بقیه دست‌نخورده می‌ماند.</summary>
        private static string ToPersianDigits(string input)
        {
            if (string.IsNullOrEmpty(input)) return input ?? "";

            var sb = new StringBuilder(input.Length);
            foreach (char c in input)
            {
                if (c >= '0' && c <= '9')
                    sb.Append((char)('۰' + (c - '0')));
                else if (c >= '٠' && c <= '٩')
                    sb.Append((char)('۰' + (c - '٠')));
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
