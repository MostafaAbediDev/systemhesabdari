using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Taadol.Controls
{
    public partial class PageSizeSelector : UserControl
    {
        private bool _isOpen;
        private bool _isSelecting;
        private int _selectionRequestVersion;
        private static readonly SolidColorBrush BlueBrush = new(Color.FromRgb(0x25, 0x63, 0xEB));
        private static readonly SolidColorBrush GrayBrush = new(Color.FromRgb(0x6B, 0x72, 0x80));
        private static readonly SolidColorBrush BlueBgBrush = new(Color.FromRgb(0xEF, 0xF6, 0xFF));

        public event EventHandler<int> SelectionChanged;
        private int _lastCloseTimestamp = -100000;

        public int SelectedPageSize { get; private set; } = 15;

        private readonly List<int> _options = new() { 10, 15, 20, 25, 50, 75 };

        private Window _subscribedWindow;

        public PageSizeSelector()
        {
            InitializeComponent();
            BuildItems();

            Loaded += (s, e) =>
            {
                var window = Window.GetWindow(this);
                if (window != null && _subscribedWindow == null)
                {
                    _subscribedWindow = window;
                    window.PreviewMouseDown += Window_PreviewMouseDown;
                }
            };

            Unloaded += (s, e) =>
            {
                if (_subscribedWindow != null)
                {
                    _subscribedWindow.PreviewMouseDown -= Window_PreviewMouseDown;
                    _subscribedWindow = null;
                }
            };
        }
        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!DropdownPopup.IsOpen) return;

            var element = e.OriginalSource as DependencyObject;
            if (element == null) return;

            if (IsDescendantOf(element, RootBorder) || IsDescendantOf(element, DropdownBorder))
                return;

            DropdownPopup.IsOpen = false;
        }
        private bool IsDescendantOf(DependencyObject child, DependencyObject parent)
        {
            while (child != null)
            {
                if (child == parent) return true;

                child = child is Visual || child is System.Windows.Media.Media3D.Visual3D
                    ? VisualTreeHelper.GetParent(child)
                    : LogicalTreeHelper.GetParent(child);
            }
            return false;
        }
        private void BuildItems()
        {
            if (DropdownItems == null) return;
            DropdownItems.Children.Clear();

            foreach (var option in _options)
            {
                var btn = new Border
                {
                    Tag = option,
                    Cursor = Cursors.Hand,
                    Padding = new Thickness(6, 3, 6, 3),
                    CornerRadius = new CornerRadius(4),
                    Margin = new Thickness(0, 1, 0, 1),
                    Background = option == SelectedPageSize ? BlueBgBrush : Brushes.Transparent,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };

                var tb = new TextBlock
                {
                    Text = option.ToString(),
                    FontSize = 11,
                    FontFamily = (FontFamily)Application.Current.FindResource("IRANSans"),
                    FontWeight = option == SelectedPageSize ? FontWeights.Bold : FontWeights.Normal,
                    Foreground = option == SelectedPageSize ? BlueBrush : GrayBrush,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                btn.Child = tb;
                btn.MouseLeftButtonDown += Item_Click;
                btn.MouseEnter += Item_MouseEnter;
                btn.MouseLeave += Item_MouseLeave;
                DropdownItems.Children.Add(btn);
            }
        }

        private void RootBorder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (_isSelecting)
                return;

            if (e.Timestamp - _lastCloseTimestamp < 200)
                return;

            DropdownPopup.IsOpen = !DropdownPopup.IsOpen;
        }

        private void RootBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            RootBorder.Background = new SolidColorBrush(Color.FromRgb(0xF9, 0xFA, 0xFB));
            RootBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0xD1, 0xD5, 0xDB));
        }

        private void RootBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!_isOpen)
            {
                RootBorder.Background = Brushes.White;
                RootBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0xE5, 0xE7, 0xEB));
            }
        }

        private void Item_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Border b)
                b.Background = new SolidColorBrush(Color.FromRgb(0xF3, 0xF4, 0xF6));
        }

        private void Item_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Border b && b.Tag is int val && val != SelectedPageSize)
                b.Background = Brushes.Transparent;
        }

        private void Item_Click(object sender, MouseButtonEventArgs e)
        {

            if (_isSelecting || sender is not Border b || b.Tag is not int val)
            {
                e.Handled = _isSelecting;
                return;
            }

            e.Handled = true;

            if (val == SelectedPageSize)
            {
                DropdownPopup.IsOpen = false;
                return;
            }

            _isSelecting = true;
            SelectedPageSize = val;
            SelectedText.Text = val.ToString();
            BuildItems();
            DropdownPopup.IsOpen = false;

            IsEnabled = false;
            var requestVersion = ++_selectionRequestVersion;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    SelectionChanged?.Invoke(this, val);
                }
                finally
                {

                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (requestVersion == _selectionRequestVersion)
                        {
                            IsEnabled = true;
                            _isSelecting = false;
                        }
                    }), DispatcherPriority.ContextIdle);
                }
            }), DispatcherPriority.Background);
        }

        private void DropdownPopup_Opened(object sender, EventArgs e)
        {
            _isOpen = true;
            RootBorder.Background = BlueBgBrush;
            RootBorder.BorderBrush = BlueBrush;

            var rotateAnim = new DoubleAnimation(180, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            ArrowRotate.BeginAnimation(RotateTransform.AngleProperty, rotateAnim);
        }
        private bool _ignoreNextOpen;

        private void DropdownPopup_Closed(object sender, EventArgs e)
        {
            _isOpen = false;
            _lastCloseTimestamp = Environment.TickCount;

            RootBorder.Background = Brushes.White;
            RootBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(0xE5, 0xE7, 0xEB));

            var rotateAnim = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            ArrowRotate.BeginAnimation(RotateTransform.AngleProperty, rotateAnim);
        }
    }
}
