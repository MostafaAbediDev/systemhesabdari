using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Taadol.Controls
{
    public partial class ToggleRadioControl : UserControl
    {
        private bool _isHovered = false;

        // ─── Dependency Properties ────────────────────────────────────

        public static readonly DependencyProperty CheckedLabelProperty =
            DependencyProperty.Register("CheckedLabel", typeof(string), typeof(ToggleRadioControl),
                new PropertyMetadata("انتخاب شده", OnLabelChanged));

        public static readonly DependencyProperty UncheckedLabelProperty =
            DependencyProperty.Register("UncheckedLabel", typeof(string), typeof(ToggleRadioControl),
                new PropertyMetadata("انتخاب نشده", OnLabelChanged));

        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register("IsChecked", typeof(bool), typeof(ToggleRadioControl),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnIsCheckedChanged));

        // ─── Properties ──────────────────────────────────────────────

        public string CheckedLabel
        {
            get { return (string)GetValue(CheckedLabelProperty); }
            set { SetValue(CheckedLabelProperty, value); }
        }

        public string UncheckedLabel
        {
            get { return (string)GetValue(UncheckedLabelProperty); }
            set { SetValue(UncheckedLabelProperty, value); }
        }

        public bool IsChecked
        {
            get { return (bool)GetValue(IsCheckedProperty); }
            set { SetValue(IsCheckedProperty, value); }
        }

        // ─── Routed Events ────────────────────────────────────────────

        public static readonly RoutedEvent CheckedEvent =
            EventManager.RegisterRoutedEvent("Checked", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(ToggleRadioControl));

        public static readonly RoutedEvent UncheckedEvent =
            EventManager.RegisterRoutedEvent("Unchecked", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(ToggleRadioControl));

        public event RoutedEventHandler Checked
        {
            add { AddHandler(CheckedEvent, value); }
            remove { RemoveHandler(CheckedEvent, value); }
        }

        public event RoutedEventHandler Unchecked
        {
            add { AddHandler(UncheckedEvent, value); }
            remove { RemoveHandler(UncheckedEvent, value); }
        }

        // ─── Constructor ──────────────────────────────────────────────

        public ToggleRadioControl()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateVisual(animate: false);
        }

        // ─── Hover Handlers ───────────────────────────────────────────

        private void Container_MouseEnter(object sender, MouseEventArgs e)
        {
            _isHovered = true;

            // Scale دایره بزرگتر
            AnimateScale(1);

            // Glow ظاهر شود
            AnimateOpacity(HoverGlow, IsChecked ? 0.5 : 0.35);

            // رنگ دایره سبز شود (فقط اگر Checked نیست)
            if (!IsChecked)
            {
                AnimateColor(OuterStrokeBrush, (Color)ColorConverter.ConvertFromString("#27AE60"));
                AnimateColor(OuterFillBrush, (Color)ColorConverter.ConvertFromString("#E8F8F0"));
            }

            // لیبل کمی حرکت کند
            // AnimateTranslate(LabelTranslate, 3);
        }

        private void Container_MouseLeave(object sender, MouseEventArgs e)
        {
            _isHovered = false;

            // Scale برگردد
            AnimateScale(1);

            // Glow محو شود
            AnimateOpacity(HoverGlow, IsChecked ? 0.3 : 0.0);

            // رنگ برگردد (فقط اگر Checked نیست)
            if (!IsChecked)
            {
                AnimateColor(OuterStrokeBrush, (Color)ColorConverter.ConvertFromString("#CBD5E1"));
                AnimateColor(OuterFillBrush, Colors.White);
            }

            // لیبل برگردد

        }

        // ─── Click Handler ────────────────────────────────────────────

        private void Container_Click(object sender, MouseButtonEventArgs e)
        {
            IsChecked = !IsChecked;
        }

        // ─── Visual Update ────────────────────────────────────────────

        private void UpdateVisual(bool animate = true)
        {
            var duration = animate
                ? TimeSpan.FromSeconds(0.2)
                : TimeSpan.Zero;

            if (IsChecked)
            {
                InnerDot.Visibility = Visibility.Visible;
                LabelText.Text = CheckedLabel;

                AnimateColor(OuterStrokeBrush,
                    (Color)ColorConverter.ConvertFromString("#27AE60"), duration);
                AnimateColor(OuterFillBrush,
                    (Color)ColorConverter.ConvertFromString("#F0FBF4"), duration);
                AnimateOpacity(HoverGlow, 0.3, duration);
            }
            else
            {
                InnerDot.Visibility = Visibility.Collapsed;
                LabelText.Text = UncheckedLabel;

                AnimateColor(OuterStrokeBrush,
                    (Color)ColorConverter.ConvertFromString("#CBD5E1"), duration);
                AnimateColor(OuterFillBrush, Colors.White, duration);
                AnimateOpacity(HoverGlow, _isHovered ? 0.35 : 0.0, duration);
            }
        }

        // ─── انیمیشن‌های کمکی ─────────────────────────────────────────

        private void AnimateScale(double to)
        {
            var dur = new Duration(TimeSpan.FromSeconds(0.15));

            var sx = new DoubleAnimation(to, dur);
            sx.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
            CircleScale.BeginAnimation(ScaleTransform.ScaleXProperty, sx);

            var sy = new DoubleAnimation(to, dur);
            sy.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
            CircleScale.BeginAnimation(ScaleTransform.ScaleYProperty, sy);
        }

        private void AnimateOpacity(UIElement target, double to,
            TimeSpan? duration = null)
        {
            var dur = new Duration(duration ?? TimeSpan.FromSeconds(0.2));
            var anim = new DoubleAnimation(to, dur);
            target.BeginAnimation(UIElement.OpacityProperty, anim);
        }

        private void AnimateColor(SolidColorBrush brush, Color to,
            TimeSpan? duration = null)
        {
            var dur = new Duration(duration ?? TimeSpan.FromSeconds(0.2));
            var anim = new ColorAnimation(to, dur);
            brush.BeginAnimation(SolidColorBrush.ColorProperty, anim);
        }

        private void AnimateTranslate(TranslateTransform translate, double to)
        {
            var dur = new Duration(TimeSpan.FromSeconds(0.15));
            var anim = new DoubleAnimation(to, dur);
            anim.EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut };
            translate.BeginAnimation(TranslateTransform.XProperty, anim);
        }

        // ─── Callbacks ────────────────────────────────────────────────

        private static void OnLabelChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ToggleRadioControl;
            if (ctrl != null) ctrl.UpdateVisual(animate: false);
        }

        private static void OnIsCheckedChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var ctrl = d as ToggleRadioControl;
            if (ctrl == null) return;

            ctrl.UpdateVisual(animate: true);

            if ((bool)e.NewValue)
                ctrl.RaiseEvent(new RoutedEventArgs(CheckedEvent, ctrl));
            else
                ctrl.RaiseEvent(new RoutedEventArgs(UncheckedEvent, ctrl));
        }
    }
}
