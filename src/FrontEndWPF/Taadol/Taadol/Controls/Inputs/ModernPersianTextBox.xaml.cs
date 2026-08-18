using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Taadol.Controls
{
    public enum ModernTextBoxInputType
    {
        Text,
        Number,
        Letters,
        Alphanumeric,
        Email
    }

    public enum ValidationState
    {
        None,
        Valid,
        Invalid
    }

    public partial class ModernPersianTextBox : UserControl
    {
        public event RoutedEventHandler TextChanged;

        public static readonly DependencyProperty InputTypeProperty =
            DependencyProperty.Register(nameof(InputType), typeof(ModernTextBoxInputType), typeof(ModernPersianTextBox),
                new PropertyMetadata(ModernTextBoxInputType.Text));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(ModernPersianTextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(ModernPersianTextBox),
                new PropertyMetadata(string.Empty, OnPlaceholderChanged));

        public static readonly DependencyProperty SuffixProperty =
            DependencyProperty.Register(nameof(Suffix), typeof(string), typeof(ModernPersianTextBox),
                new PropertyMetadata(string.Empty, OnSuffixChanged));

        public static readonly DependencyProperty ValidationStateProperty =
            DependencyProperty.Register(nameof(ValidationState), typeof(ValidationState), typeof(ModernPersianTextBox),
                new PropertyMetadata(ValidationState.None, OnValidationStateChanged));

        public static readonly DependencyProperty ValidationMessageProperty =
            DependencyProperty.Register(nameof(ValidationMessage), typeof(string), typeof(ModernPersianTextBox),
                new PropertyMetadata(string.Empty, OnValidationMessageChanged));

        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register(nameof(MaxLength), typeof(int), typeof(ModernPersianTextBox),
                new PropertyMetadata(0, OnMaxLengthChanged));

        public ModernTextBoxInputType InputType
        {
            get => (ModernTextBoxInputType)GetValue(InputTypeProperty);
            set => SetValue(InputTypeProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public string Suffix
        {
            get => (string)GetValue(SuffixProperty);
            set => SetValue(SuffixProperty, value);
        }

        public ValidationState ValidationState
        {
            get => (ValidationState)GetValue(ValidationStateProperty);
            set => SetValue(ValidationStateProperty, value);
        }

        public string ValidationMessage
        {
            get => (string)GetValue(ValidationMessageProperty);
            set => SetValue(ValidationMessageProperty, value);
        }

        public int MaxLength
        {
            get => (int)GetValue(MaxLengthProperty);
            set => SetValue(MaxLengthProperty, value);
        }

        public ModernPersianTextBox()
        {
            InitializeComponent();
            UpdatePlaceholderVisibility();
            UpdateSuffixVisibility();
            DataObject.AddPastingHandler(PART_TextBox, OnPaste);
            Loaded += (s, e) =>
            {
                UpdateIconAndTextDirection();
                // Loaded پدینگ را ریست می‌کند؛ اگر آیکون فعال بود دوباره جایش را باز کن
                UpdatePaddingForIcon(ValidationState != ValidationState.None);
            };
            UpdateIconAndTextDirection();
        }

        /// <summary>
        /// فیلد نباید با محتوای زیاد کش بیاید و چیدمان بقیه‌ی فرم را تحت تاثیر قرار دهد.
        /// عرض دلخواه همیشه به فضای موجود محدود می‌شود (و در بافتِ بدون محدودیت، سقف معقول ۳۶۰).
        /// محتوای اضافه با اسکرول افقی داخل خود فیلد قابل مشاهده است.
        /// </summary>
        protected override Size MeasureOverride(Size availableSize)
        {
            var desired = base.MeasureOverride(availableSize);

            if (!double.IsInfinity(availableSize.Width) && availableSize.Width > 0)
                desired.Width = System.Math.Min(desired.Width, availableSize.Width);
            else if (double.IsInfinity(availableSize.Width) && desired.Width > 360)
                desired.Width = 360;

            return desired;
        }

        private void PART_TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsInputAllowed(e.Text);
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            string pastedText = e.DataObject.GetData(typeof(string)) as string;

            if (!IsInputAllowed(pastedText))
                e.CancelCommand();
        }

        private bool IsInputAllowed(string input)
        {
            if (string.IsNullOrEmpty(input))
                return true;

            switch (InputType)
            {
                case ModernTextBoxInputType.Number:
                    return Regex.IsMatch(input, @"^[0-9۰-۹]+$");

                case ModernTextBoxInputType.Letters:
                    return Regex.IsMatch(input, @"^[\p{L}\s]+$");

                case ModernTextBoxInputType.Alphanumeric:
                    return Regex.IsMatch(input, @"^[a-zA-Z0-9]+$");

                case ModernTextBoxInputType.Email:
                    return Regex.IsMatch(input, @"^[a-zA-Z0-9@._-]+$");

                case ModernTextBoxInputType.Text:
                default:
                    return true;
            }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ModernPersianTextBox)d;
            control.PART_TextBox.Text = e.NewValue?.ToString() ?? string.Empty;
            control.UpdatePlaceholderVisibility();
        }

        private static void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ModernPersianTextBox)d;
            control.PlaceholderText.Text = e.NewValue?.ToString() ?? string.Empty;
        }

        private static void OnSuffixChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ModernPersianTextBox)d;
            control.SuffixText.Text = e.NewValue?.ToString() ?? string.Empty;
            control.UpdateSuffixVisibility();
        }

        private static void OnValidationStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ModernPersianTextBox ctrl)
                ctrl.UpdateValidationVisual();
        }

        private static void OnValidationMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ModernPersianTextBox ctrl)
                ctrl.UpdateValidationVisual();
        }

        private static void OnMaxLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ModernPersianTextBox ctrl && e.NewValue is int maxLen)
                ctrl.PART_TextBox.MaxLength = maxLen;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int caret = PART_TextBox.CaretIndex;

            if (InputType == ModernTextBoxInputType.Number || InputType == ModernTextBoxInputType.Text)
            {
                PART_TextBox.Text = ConvertToPersianDigits(PART_TextBox.Text);
                PART_TextBox.CaretIndex = Math.Min(caret, PART_TextBox.Text.Length);
            }

            Text = PART_TextBox.Text;
            UpdatePlaceholderVisibility();
            TextChanged?.Invoke(this, new RoutedEventArgs());
        }

        private void PART_TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ValidationState == ValidationState.None)
            {
                border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2667FF"));
                border.BorderThickness = new Thickness(1.5);
            }

            var tb = sender as TextBox;
            if (tb != null && string.IsNullOrEmpty(tb.Text))
            {
                tb.CaretIndex = 0;
            }
        }

        private void PART_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ValidationState == ValidationState.None)
            {
                border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8E8E8"));
                border.BorderThickness = new Thickness(1);
            }
        }

        private void UpdatePlaceholderVisibility()
        {
            PlaceholderText.Visibility = string.IsNullOrEmpty(PART_TextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateSuffixVisibility()
        {
            SuffixText.Visibility = string.IsNullOrEmpty(Suffix) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void UpdateIconAndTextDirection()
        {
            PART_TextBox.FlowDirection = this.FlowDirection;
            PlaceholderText.FlowDirection = this.FlowDirection;
            ValidationText.FlowDirection = FlowDirection.RightToLeft;

            bool isRtl = this.FlowDirection == FlowDirection.RightToLeft;

            // در هر دو جهت، متن و کرسر از یک لبه شروع می‌شوند و پلیس‌هولدر دقیقاً
            // هم‌تراز با همان لبه قرار می‌گیرد تا کرسر تکست‌باکس خالی دقیقاً از
            // محل شروع هینت شروع شود. آیکون ولیدیشن همیشه سمت مخالف شروع متن است.
            if (isRtl)
            {
                ValidationText.HorizontalAlignment = HorizontalAlignment.Left;

                // آیکون سمت مخالف شروع متن (سمت چپ) — در RTL با HA=Right چپ می‌نشیند
                ValidationIconBorder.HorizontalAlignment = HorizontalAlignment.Right;
                ValidationIconBorder.Margin = new Thickness(0, 0, 10, 0);
                // ⚠️ در RTL، HorizontalAlignment/ContentAlignment سمنتیک برعکس دارد:
                // HA=Right عنصر را چپ می‌گذارد و HA=Left راست.
                PART_TextBox.HorizontalContentAlignment = HorizontalAlignment.Left;
                PART_TextBox.Padding = new Thickness(16, 0, 12, 0);
                PlaceholderText.HorizontalAlignment = HorizontalAlignment.Left;
                PlaceholderText.Margin = new Thickness(16, 0, 0, 0);
            }
            else
            {
                // پیام خطا همیشه سمت راست (مثل فیلدهای RTL/نام)
                ValidationText.HorizontalAlignment = HorizontalAlignment.Right;

                // برای یکسان بودن جایگاه آیکون در همه‌ی فیلدها (مثل فیلدهای RTL)،
                // آیکون در فیلدهای LTR هم سمت چپ می‌نشیند. چون متن این فیلدها سمت
                // چپ است، هنگام نمایش آیکون پدینگ چپ اضافه می‌شود تا تداخل نشود
                // (UpdatePaddingForIcon).
                ValidationIconBorder.HorizontalAlignment = HorizontalAlignment.Left;
                ValidationIconBorder.Margin = new Thickness(10, 0, 0, 0);
                PART_TextBox.HorizontalContentAlignment = HorizontalAlignment.Left;
                PART_TextBox.Padding = new Thickness(12, 0, 12, 0);
                PlaceholderText.HorizontalAlignment = HorizontalAlignment.Left;
                PlaceholderText.Margin = new Thickness(12, 0, 0, 0);
            }

            SuffixText.HorizontalAlignment = HorizontalAlignment.Left;
            SuffixText.Margin = new Thickness(10, 0, 0, 0);
        }

        private void UpdateValidationVisual()
        {
            UpdatePaddingForIcon(ValidationState != ValidationState.None);

            switch (ValidationState)
            {
                case ValidationState.Valid:
                    ValidationIconBorder.Visibility = Visibility.Visible;
                    ValidationIconBorder.Background = Brushes.Transparent;
                    WarningIcon.Visibility = Visibility.Collapsed;
                    SuccessIcon.Visibility = Visibility.Visible;
                    ValidationText.Visibility = Visibility.Collapsed;
                    break;

                case ValidationState.Invalid:
                    border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626"));
                    border.BorderThickness = new Thickness(1.5);
                    ValidationIconBorder.Visibility = Visibility.Visible;
                    ValidationIconBorder.Background = Brushes.Transparent;
                    WarningIcon.Visibility = Visibility.Visible;
                    SuccessIcon.Visibility = Visibility.Collapsed;
                    if (!string.IsNullOrEmpty(ValidationMessage))
                    {
                        ValidationText.Visibility = Visibility.Visible;
                        ValidationText.Text = ValidationMessage;
                        ValidationText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626"));
                    }
                    break;

                case ValidationState.None:
                default:
                    border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8E8E8"));
                    border.BorderThickness = new Thickness(1);
                    ValidationIconBorder.Visibility = Visibility.Collapsed;
                    ValidationText.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        /// <summary>
        /// فقط در فیلدهای LTR: آیکون سمت چپ است و متن هم سمت چپ شروع می‌شود، پس وقتی
        /// آیکون ظاهر می‌شود باید پدینگ چپ اضافه شود تا متن زیر آیکون نرود.
        /// (در فیلدهای RTL متن سمت راست است و آیکون سمت چپ — تداخلی نیست.)
        /// </summary>
        private void UpdatePaddingForIcon(bool iconVisible)
        {
            if (this.FlowDirection != FlowDirection.LeftToRight) return;

            if (iconVisible)
            {
                PART_TextBox.Padding = new Thickness(34, 0, 12, 0);
                PlaceholderText.Margin = new Thickness(34, 0, 0, 0);
            }
            else
            {
                PART_TextBox.Padding = new Thickness(12, 0, 12, 0);
                PlaceholderText.Margin = new Thickness(12, 0, 0, 0);
            }
        }

        private string ConvertToPersianDigits(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            string[] englishDigits = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

            for (int i = 0; i < 10; i++)
                input = input.Replace(englishDigits[i], persianDigits[i]);

            return input;
        }
    }
}
