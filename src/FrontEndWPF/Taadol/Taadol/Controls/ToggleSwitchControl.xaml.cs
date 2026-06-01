using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace Taadol.Controls
{
    public partial class ToggleSwitchControl : UserControl
    {
                public static readonly DependencyProperty FirstLabelProperty =
            DependencyProperty.Register("FirstLabel", typeof(string), typeof(ToggleSwitchControl),
                new PropertyMetadata("اتوماتیک", OnLabelChanged));

        public static readonly DependencyProperty SecondLabelProperty =
            DependencyProperty.Register("SecondLabel", typeof(string), typeof(ToggleSwitchControl),
                new PropertyMetadata("دستی", OnLabelChanged));

        public static readonly DependencyProperty IsFirstSelectedProperty =
            DependencyProperty.Register("IsFirstSelected", typeof(bool), typeof(ToggleSwitchControl),
                new PropertyMetadata(true, OnSelectionChanged));

                public event EventHandler<bool> SelectionChanged;

                public string FirstLabel
        {
            get { return (string)GetValue(FirstLabelProperty); }
            set { SetValue(FirstLabelProperty, value); }
        }

        public string SecondLabel
        {
            get { return (string)GetValue(SecondLabelProperty); }
            set { SetValue(SecondLabelProperty, value); }
        }

        public bool IsFirstSelected
        {
            get { return (bool)GetValue(IsFirstSelectedProperty); }
            set { SetValue(IsFirstSelectedProperty, value); }
        }

                public ToggleSwitchControl()
        {
            InitializeComponent();
            this.Loaded += (s, e) => UpdateLabels();
        }

                private static void OnLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ToggleSwitchControl;
            if (control != null)
                control.UpdateLabels();
        }

        private void UpdateLabels()
        {
            var autoText = FindTextBlock(BtnAuto);
            var manualText = FindTextBlock(BtnManual);

            if (autoText != null) autoText.Text = FirstLabel;
            if (manualText != null) manualText.Text = SecondLabel;
        }

        private TextBlock FindTextBlock(ToggleButton btn)
        {
            if (btn == null) return null;
            if (VisualTreeHelper.GetChildrenCount(btn) == 0) return null;

            var border = VisualTreeHelper.GetChild(btn, 0) as Border;
            if (border == null) return null;

            return border.Child as TextBlock;
        }

                private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ToggleSwitchControl;
            if (control == null) return;

            if ((bool)e.NewValue)
            {
                control.BtnAuto.IsChecked = true;
                control.BtnManual.IsChecked = false;
                control.AnimateSlider(0);
            }
            else
            {
                control.BtnManual.IsChecked = true;
                control.BtnAuto.IsChecked = false;
                control.AnimateSlider(control.SliderBg.ActualWidth);
            }
        }

                private void BtnAuto_Checked(object sender, RoutedEventArgs e)
        {
            if (BtnManual == null) return;
            BtnManual.IsChecked = false;
            AnimateSlider(0);
            SelectionChanged?.Invoke(this, true);
        }

        private void BtnManual_Checked(object sender, RoutedEventArgs e)
        {
            if (BtnAuto == null) return;
            BtnAuto.IsChecked = false;
            AnimateSlider(SliderBg.ActualWidth);
            SelectionChanged?.Invoke(this, false);
        }

                private void AnimateSlider(double toX)
        {
            var ease = new QuarticEase { EasingMode = EasingMode.EaseInOut };

            var anim = new DoubleAnimation
            {
                To = toX,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = ease
            };

            SliderTransform.BeginAnimation(TranslateTransform.XProperty, anim);
            AnimateShadow();
        }

        private void AnimateShadow()
        {
            var shadow = SliderBg.Effect as DropShadowEffect;
            if (shadow == null) return;

            var anim = new DoubleAnimation
            {
                To = 0.3,
                Duration = TimeSpan.FromMilliseconds(150),
                AutoReverse = true,
                EasingFunction = new QuarticEase { EasingMode = EasingMode.EaseInOut }
            };

            shadow.BeginAnimation(DropShadowEffect.OpacityProperty, anim);
        }
    }
}
