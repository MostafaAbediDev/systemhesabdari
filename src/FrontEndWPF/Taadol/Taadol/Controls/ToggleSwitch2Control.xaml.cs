using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Taadol.Controls
{
    public partial class ToggleSwitch2Control : UserControl
    {
        private static readonly SolidColorBrush WhiteBrush = new(Colors.White);
        private static readonly SolidColorBrush GrayBrush = new(Color.FromRgb(0x37, 0x41, 0x51));

        public static readonly DependencyProperty FirstLabelProperty =
            DependencyProperty.Register("FirstLabel", typeof(string), typeof(ToggleSwitch2Control),
                new PropertyMetadata("خودکار", OnLabelChanged));

        public static readonly DependencyProperty SecondLabelProperty =
            DependencyProperty.Register("SecondLabel", typeof(string), typeof(ToggleSwitch2Control),
                new PropertyMetadata("دستی", OnLabelChanged));

        public static readonly DependencyProperty IsFirstSelectedProperty =
            DependencyProperty.Register("IsFirstSelected", typeof(bool), typeof(ToggleSwitch2Control),
                new PropertyMetadata(true, OnSelectionChanged));

        public event EventHandler<bool> SelectionChanged;

        public string FirstLabel
        {
            get => (string)GetValue(FirstLabelProperty);
            set => SetValue(FirstLabelProperty, value);
        }

        public string SecondLabel
        {
            get => (string)GetValue(SecondLabelProperty);
            set => SetValue(SecondLabelProperty, value);
        }

        public bool IsFirstSelected
        {
            get => (bool)GetValue(IsFirstSelectedProperty);
            set => SetValue(IsFirstSelectedProperty, value);
        }

        private const double OuterRadius = 6;
        private const double SliderRadius = 10;

        public ToggleSwitch2Control()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                UpdateLabels();
                UpdateClip();
                UpdateVisualState(animate: false);
            };
            this.SizeChanged += (s, e) =>
            {
                UpdateClip();
                UpdateVisualState(animate: false);
            };
        }

        private void UpdateClip()
        {
            if (ToggleRoot.ActualWidth <= 0 || ToggleRoot.ActualHeight <= 0) return;
            ToggleRoot.Clip = new RectangleGeometry(
                new Rect(0, 0, ToggleRoot.ActualWidth, ToggleRoot.ActualHeight),
                OuterRadius, OuterRadius);
        }

        private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ToggleSwitch2Control control) control.UpdateLabels();
        }

        private void UpdateLabels()
        {
            var firstText = FindTextBlock(BtnFirst);
            var secondText = FindTextBlock(BtnSecond);
            if (firstText != null) firstText.Text = FirstLabel;
            if (secondText != null) secondText.Text = SecondLabel;
        }

        private TextBlock FindTextBlock(ToggleButton btn)
        {
            if (btn == null || VisualTreeHelper.GetChildrenCount(btn) == 0) return null;
            var border = VisualTreeHelper.GetChild(btn, 0) as Border;
            return border?.Child as TextBlock;
        }

        private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ToggleSwitch2Control control) control.UpdateVisualState(animate: true);
        }

        private void UpdateVisualState(bool animate)
        {
            bool isFirst = IsFirstSelected;
            double w = ToggleRoot.ActualWidth;
            double sliderMargin = 4;

            if (isFirst)
            {
                SliderBg.CornerRadius = new CornerRadius(0, SliderRadius, SliderRadius, 0);
                BtnFirst.IsChecked = true;
                BtnSecond.IsChecked = false;
                AnimateSlider(w / 2 + sliderMargin, animate);
            }
            else
            {
                SliderBg.CornerRadius = new CornerRadius(SliderRadius, 0, 0, SliderRadius);
                BtnSecond.IsChecked = true;
                BtnFirst.IsChecked = false;
                AnimateSlider(-(w / 2 + sliderMargin), animate);
            }

            var firstText = FindTextBlock(BtnFirst);
            var secondText = FindTextBlock(BtnSecond);
            if (isFirst)
            {
                if (firstText != null) firstText.Foreground = WhiteBrush;
                if (secondText != null) secondText.Foreground = GrayBrush;
            }
            else
            {
                if (firstText != null) firstText.Foreground = GrayBrush;
                if (secondText != null) secondText.Foreground = WhiteBrush;
            }
        }

        private void BtnFirst_Checked(object sender, RoutedEventArgs e)
        {
            if (BtnSecond == null) return;
            BtnSecond.IsChecked = false;
            IsFirstSelected = true;
            SelectionChanged?.Invoke(this, true);
        }

        private void BtnFirst_Unchecked(object sender, RoutedEventArgs e)
        {
            if (BtnSecond.IsChecked != true) BtnFirst.IsChecked = true;
        }

        private void BtnSecond_Checked(object sender, RoutedEventArgs e)
        {
            if (BtnFirst == null) return;
            BtnFirst.IsChecked = false;
            IsFirstSelected = false;
            SelectionChanged?.Invoke(this, false);
        }

        private void BtnSecond_Unchecked(object sender, RoutedEventArgs e)
        {
            if (BtnFirst.IsChecked != true) BtnSecond.IsChecked = true;
        }

        private void AnimateSlider(double toX, bool animate)
        {
            if (animate)
            {
                var ease = new QuarticEase { EasingMode = EasingMode.EaseInOut };
                var anim = new DoubleAnimation
                {
                    To = toX,
                    Duration = TimeSpan.FromMilliseconds(250),
                    EasingFunction = ease
                };
                SliderTransform.BeginAnimation(TranslateTransform.XProperty, anim);
            }
            else
            {
                SliderTransform.BeginAnimation(TranslateTransform.XProperty, null);
                SliderTransform.X = toX;
            }
        }
    }
}
