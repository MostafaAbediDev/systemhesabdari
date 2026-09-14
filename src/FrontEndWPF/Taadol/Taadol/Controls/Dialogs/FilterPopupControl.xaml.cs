using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Taadol.Controls
{

    public partial class FilterPopupControl : UserControl
    {

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(FilterPopupControl),
                new PropertyMetadata("فیلتر", OnTitleChanged));

        public static readonly DependencyProperty ShowSearchProperty =
            DependencyProperty.Register(nameof(ShowSearch), typeof(bool), typeof(FilterPopupControl),
                new PropertyMetadata(false, OnShowSearchChanged));

        public static readonly DependencyProperty ImmediateApplyProperty =
            DependencyProperty.Register(nameof(ImmediateApply), typeof(bool), typeof(FilterPopupControl),
                new PropertyMetadata(false, OnImmediateApplyChanged));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public bool ShowSearch
        {
            get => (bool)GetValue(ShowSearchProperty);
            set => SetValue(ShowSearchProperty, value);
        }

        public bool ImmediateApply
        {
            get => (bool)GetValue(ImmediateApplyProperty);
            set => SetValue(ImmediateApplyProperty, value);
        }

        public List<string> Options { get; set; } = new List<string>();

        public HashSet<string> SelectedOptions { get; set; } = new HashSet<string>();

        public event Action<List<string>> SelectionChanged;

        private readonly Dictionary<string, ToggleButton> _toggleMap = new();
        private Popup _activePopup;

        public FilterPopupControl()
        {
            InitializeComponent();
        }

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterPopupControl ctrl)
                ctrl.HeaderText.Text = e.NewValue?.ToString() ?? "فیلتر";
        }

        private static void OnShowSearchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterPopupControl ctrl)
                ctrl.SearchBoxBorder.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
        }

        private static void OnImmediateApplyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FilterPopupControl ctrl)
                ctrl.FooterBorder.Visibility = (bool)e.NewValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public void ShowAt(FrameworkElement anchor)
        {
            if (anchor == null) return;

            if (Options == null || Options.Count == 0)
            {
                ToastManager.Info("مقداری برای فیلتر کردن وجود ندارد.");
                return;
            }

            if (_activePopup != null && _activePopup.IsOpen)
                _activePopup.IsOpen = false;

            BuildCheckBoxes();

            SelectAllToggle.IsChecked = SelectedOptions.Count == 0;

            this.Content = null;

            _activePopup = new Popup
            {
                Placement = PlacementMode.Bottom,
                PlacementTarget = anchor,
                StaysOpen = false,
                AllowsTransparency = true,
                PopupAnimation = PopupAnimation.Fade,
                FlowDirection = FlowDirection.RightToLeft,
                Child = RootCard
            };

            _activePopup.IsOpen = true;

            if (ShowSearch)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    SearchBox.Focus();
                    System.Windows.Input.Keyboard.Focus(SearchBox);
                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }

        public void Close()
        {
            if (_activePopup != null)
                _activePopup.IsOpen = false;
        }

        private void BuildCheckBoxes()
        {
            ItemsHost.Children.Clear();
            _toggleMap.Clear();

            var style = (Style)Resources["FilterCheckBoxStyle"];

            foreach (var option in Options)
            {
                var row = CreateCheckBoxRow(option, style);
                ItemsHost.Children.Add(row);
            }
        }

        private ToggleButton CreateCheckBoxRow(string option, Style style)
        {
            var tb = new ToggleButton
            {
                Content = option,
                IsChecked = SelectedOptions.Contains(option),
                Style = style
            };

            tb.Checked += (s, ev) => { if (!_suppressSelectAllSync) SyncSelectAllToggle(); };
            tb.Unchecked += (s, ev) => { if (!_suppressSelectAllSync) SyncSelectAllToggle(); };

            if (ImmediateApply)
            {
                tb.Checked += (s, ev) => RaiseSelectionChanged(option, true);
                tb.Unchecked += (s, ev) => RaiseSelectionChanged(option, false);
            }

            _toggleMap[option] = tb;
            return tb;
        }
        private void SelectAllToggle_Checked(object sender, RoutedEventArgs e)
        {

            _suppressSelectAllSync = true;
            foreach (var kvp in _toggleMap)
            {
                if (kvp.Value.Visibility == Visibility.Visible)
                    kvp.Value.IsChecked = false;
            }
            _suppressSelectAllSync = false;

            if (ImmediateApply)
            {
                SelectedOptions.Clear();
                SelectionChanged?.Invoke(new List<string>());
            }
        }

        private void SelectAllToggle_Unchecked(object sender, RoutedEventArgs e)
        {

            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (_toggleMap.Values.All(t => t.IsChecked != true))
                    SelectAllToggle.IsChecked = true;
            }), System.Windows.Threading.DispatcherPriority.Background);
        }

        private bool _suppressSelectAllSync = false;

        private void SyncSelectAllToggle()
        {
            if (SelectAllToggle == null) return;

            var visibleItems = _toggleMap.Values
                .Where(t => t.Visibility == Visibility.Visible)
                .ToList();

            _suppressSelectAllSync = true;
            SelectAllToggle.IsChecked = visibleItems.Count == 0
                                        || !visibleItems.Any(t => t.IsChecked == true);
            _suppressSelectAllSync = false;
        }
        private void SetChipChecked(Border chip, TextBlock check, bool isChecked)
        {
            if (isChecked)
            {
                chip.BorderBrush = new SolidColorBrush(Color.FromRgb(0x26, 0x67, 0xFF));
                chip.Background = new SolidColorBrush(Color.FromRgb(0x26, 0x67, 0xFF));
                check.Visibility = Visibility.Visible;
            }
            else
            {
                chip.BorderBrush = new SolidColorBrush(Color.FromRgb(0xC0, 0xC7, 0xDD));
                chip.Background = Brushes.White;
                check.Visibility = Visibility.Collapsed;
            }
        }

        private void RaiseSelectionChanged(string option, bool isSelected)
        {
            if (isSelected)
                SelectedOptions.Add(option);
            else
                SelectedOptions.Remove(option);

            SelectionChanged?.Invoke(new List<string>(SelectedOptions));
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            PlaceholderText.Visibility = string.IsNullOrEmpty(SearchBox.Text)
                ? Visibility.Visible : Visibility.Collapsed;

            var q = SearchBox.Text?.Trim() ?? "";
            foreach (var kvp in _toggleMap)
            {
                var visible = string.IsNullOrEmpty(q) ||
                              kvp.Key.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0;
                kvp.Value.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {

            foreach (var kvp in _toggleMap)
            {
                if (kvp.Value.Visibility == Visibility.Visible)
                    kvp.Value.IsChecked = true;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var kvp in _toggleMap)
                kvp.Value.IsChecked = false;
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {

            var result = new List<string>();
            foreach (var kvp in _toggleMap)
            {
                if (kvp.Value.IsChecked == true)
                    result.Add(kvp.Key);
            }

            SelectedOptions.Clear();
            foreach (var r in result) SelectedOptions.Add(r);

            SelectionChanged?.Invoke(result);
            Close();
        }
    }
}
