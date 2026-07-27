using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Taadol.Controls
{
    public partial class ActiveToggleControl : UserControl
    {
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ActiveToggleControl),
                new PropertyMetadata(false, OnIsCheckedChanged));

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(ActiveToggleControl),
                new PropertyMetadata("شخص فعال است", OnLabelChanged));

        public event RoutedEventHandler Toggled;

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public ActiveToggleControl()
        {
            InitializeComponent();
            this.Loaded += (s, e) => UpdateVisual(false);
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ActiveToggleControl ctrl)
                ctrl.UpdateVisual(true);
        }

        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ActiveToggleControl ctrl && e.NewValue is string text)
                ctrl.LabelText.Text = text;
        }

        private void Toggle_Click(object sender, RoutedEventArgs e)
        {
            IsChecked = !IsChecked;
            Toggled?.Invoke(this, e);
        }

        private void UpdateVisual(bool animate)
        {
            double toX = IsChecked ? 26 : 0;

            var targetBg = IsChecked
                ? new SolidColorBrush(Color.FromRgb(0x22, 0xC5, 0x5E))
                : new SolidColorBrush(Color.FromRgb(0xE5, 0xE7, 0xEB));

            if (animate)
            {
                var anim = new DoubleAnimation(toX, TimeSpan.FromMilliseconds(200))
                {
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };
                KnobTransform.BeginAnimation(TranslateTransform.XProperty, anim);

                var bgAnim = new ColorAnimation
                {
                    To = targetBg.Color,
                    Duration = TimeSpan.FromMilliseconds(200)
                };
                ToggleBorder.Background.BeginAnimation(SolidColorBrush.ColorProperty, bgAnim);
            }
            else
            {
                KnobTransform.X = toX;
                ToggleBorder.Background = targetBg;
            }
        }
    }
}
