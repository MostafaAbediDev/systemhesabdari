using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Taadol.Controls
{
    public partial class ModernPersianTextBox : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(ModernPersianTextBox),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnTextChanged));

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(ModernPersianTextBox),
                new PropertyMetadata(string.Empty, OnPlaceholderChanged));

        public static readonly DependencyProperty SuffixProperty =
            DependencyProperty.Register(nameof(Suffix), typeof(string), typeof(ModernPersianTextBox),
                new PropertyMetadata(string.Empty, OnSuffixChanged));

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

        public ModernPersianTextBox()
        {
            InitializeComponent();
            UpdatePlaceholderVisibility();
            UpdateSuffixVisibility();
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Text = PART_TextBox.Text;
            UpdatePlaceholderVisibility();
        }

        private void PART_TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2667FF"));
            border.BorderThickness = new Thickness(1.5);
        }

        private void PART_TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            border.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E8E8E8"));
            border.BorderThickness = new Thickness(1);
        }

        private void UpdatePlaceholderVisibility()
        {
            PlaceholderText.Visibility = string.IsNullOrEmpty(PART_TextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UpdateSuffixVisibility()
        {
            SuffixText.Visibility = string.IsNullOrEmpty(Suffix) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
