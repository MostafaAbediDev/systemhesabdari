using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Taadol.Controls
{

    public partial class SearchBoxControl : UserControl
    {
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(SearchBoxControl),
                new PropertyMetadata("جستجو..."));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(SearchBoxControl),
                new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly DependencyProperty DebounceMsProperty =
            DependencyProperty.Register(nameof(DebounceMs), typeof(int), typeof(SearchBoxControl),
                new PropertyMetadata(300));

        public event EventHandler<string> SearchTextChanged;

        private readonly DispatcherTimer _debounceTimer;

        public SearchBoxControl()
        {
            InitializeComponent();

            _debounceTimer = new DispatcherTimer();
            _debounceTimer.Tick += (s, e) =>
            {
                _debounceTimer.Stop();
                SearchTextChanged?.Invoke(this, Text);
            };
        }

        private void SearchInput_TextChanged(object sender, TextChangedEventArgs e)
        {

            ClearButton.Visibility = string.IsNullOrEmpty(Text) ? Visibility.Collapsed : Visibility.Visible;

            _debounceTimer.Stop();
            _debounceTimer.Interval = TimeSpan.FromMilliseconds(Math.Max(0, DebounceMs));
            _debounceTimer.Start();
        }

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public int DebounceMs
        {
            get => (int)GetValue(DebounceMsProperty);
            set => SetValue(DebounceMsProperty, value);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            Text = "";
            SearchInput.Focus();
        }
    }
}
