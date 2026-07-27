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
        Letters
    }

    public enum ValidationState
    {
        None,
        Valid,
        Invalid
    }

    public partial class ModernPersianTextBox : UserControl
    {
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

        public ModernPersianTextBox()
        {
            InitializeComponent();
            UpdatePlaceholderVisibility();
            UpdateSuffixVisibility();
            DataObject.AddPastingHandler(PART_TextBox, OnPaste);
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
        }

        private void PART_TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (ValidationState == ValidationState.None)
            {
                border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2667FF"));
                border.BorderThickness = new Thickness(1.5);
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

        private void UpdateValidationVisual()
        {
            switch (ValidationState)
            {
                case ValidationState.Valid:
                    border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#10B981"));
                    border.BorderThickness = new Thickness(1.5);
                    ValidationIconBorder.Visibility = Visibility.Visible;
                    ValidationIconBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D1FAE5"));
                    WarningIcon.Visibility = Visibility.Collapsed;
                    SuccessIcon.Visibility = Visibility.Visible;
                    ValidationText.Visibility = Visibility.Collapsed;
                    break;

                case ValidationState.Invalid:
                    border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DC2626"));
                    border.BorderThickness = new Thickness(1.5);
                    ValidationIconBorder.Visibility = Visibility.Visible;
                    ValidationIconBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FEE2E2"));
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
