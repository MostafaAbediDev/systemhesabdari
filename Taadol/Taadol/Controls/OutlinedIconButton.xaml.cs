using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Taadol.Controls
{
    /// <summary>
    /// Interaction logic for OutlinedIconButton.xaml
    /// </summary>
    public partial class OutlinedIconButton : UserControl
    {
        public OutlinedIconButton()
        {
            InitializeComponent();
        }

        // ─── Event ───────────────────────────────────────────────
        public static readonly RoutedEvent ClickEvent =
            EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(OutlinedIconButton));

        public event RoutedEventHandler Click
        {
            add => AddHandler(ClickEvent, value);
            remove => RemoveHandler(ClickEvent, value);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ClickEvent, this));
        }

        // ─── Text ─────────────────────────────────────────────────
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(OutlinedIconButton),
                new PropertyMetadata("دکمه"));

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty TextSizeProperty =
            DependencyProperty.Register(nameof(TextSize), typeof(double), typeof(OutlinedIconButton),
                new PropertyMetadata(16.0));

        public double TextSize
        {
            get => (double)GetValue(TextSizeProperty);
            set => SetValue(TextSizeProperty, value);
        }

        public static readonly DependencyProperty TextWeightProperty =
            DependencyProperty.Register(nameof(TextWeight), typeof(FontWeight), typeof(OutlinedIconButton),
                new PropertyMetadata(FontWeights.Medium));

        public FontWeight TextWeight
        {
            get => (FontWeight)GetValue(TextWeightProperty);
            set => SetValue(TextWeightProperty, value);
        }

        public static readonly DependencyProperty TextForegroundProperty =
            DependencyProperty.Register(nameof(TextForeground), typeof(Brush), typeof(OutlinedIconButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0x26, 0x67, 0xFF))));

        public Brush TextForeground
        {
            get => (Brush)GetValue(TextForegroundProperty);
            set => SetValue(TextForegroundProperty, value);
        }

        // ─── Icon ─────────────────────────────────────────────────
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(nameof(IconSource), typeof(Uri), typeof(OutlinedIconButton),
                new PropertyMetadata(null, OnIconSourceChanged));

        public Uri IconSource
        {
            get => (Uri)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        private static void OnIconSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (OutlinedIconButton)d;
            ctrl.IconVisibility = e.NewValue is null ? Visibility.Collapsed : Visibility.Visible;
        }

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(OutlinedIconButton),
                new PropertyMetadata(20.0));

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public static readonly DependencyProperty IconVisibilityProperty =
            DependencyProperty.Register(nameof(IconVisibility), typeof(Visibility), typeof(OutlinedIconButton),
                new PropertyMetadata(Visibility.Collapsed));

        public Visibility IconVisibility
        {
            get => (Visibility)GetValue(IconVisibilityProperty);
            set => SetValue(IconVisibilityProperty, value);
        }

        // ─── Colors ───────────────────────────────────────────────
        public static readonly DependencyProperty ButtonBackgroundProperty =
            DependencyProperty.Register(nameof(ButtonBackground), typeof(Brush), typeof(OutlinedIconButton),
                new PropertyMetadata(Brushes.White));

        public Brush ButtonBackground
        {
            get => (Brush)GetValue(ButtonBackgroundProperty);
            set => SetValue(ButtonBackgroundProperty, value);
        }

        public static readonly DependencyProperty ButtonBorderBrushProperty =
            DependencyProperty.Register(nameof(ButtonBorderBrush), typeof(Brush), typeof(OutlinedIconButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0x26, 0x67, 0xFF))));

        public Brush ButtonBorderBrush
        {
            get => (Brush)GetValue(ButtonBorderBrushProperty);
            set => SetValue(ButtonBorderBrushProperty, value);
        }

        public static readonly DependencyProperty ButtonBorderThicknessProperty =
            DependencyProperty.Register(nameof(ButtonBorderThickness), typeof(Thickness), typeof(OutlinedIconButton),
                new PropertyMetadata(new Thickness(1)));

        public Thickness ButtonBorderThickness
        {
            get => (Thickness)GetValue(ButtonBorderThicknessProperty);
            set => SetValue(ButtonBorderThicknessProperty, value);
        }

        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.Register(nameof(HoverBackground), typeof(Brush), typeof(OutlinedIconButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0xF0, 0xF5, 0xFF))));

        public Brush HoverBackground
        {
            get => (Brush)GetValue(HoverBackgroundProperty);
            set => SetValue(HoverBackgroundProperty, value);
        }

        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.Register(nameof(PressedBackground), typeof(Brush), typeof(OutlinedIconButton),
                new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0xE0, 0xEB, 0xFF))));

        public Brush PressedBackground
        {
            get => (Brush)GetValue(PressedBackgroundProperty);
            set => SetValue(PressedBackgroundProperty, value);
        }

        // ─── Size & Layout ────────────────────────────────────────
        public static readonly DependencyProperty ButtonHeightProperty =
            DependencyProperty.Register(nameof(ButtonHeight), typeof(double), typeof(OutlinedIconButton),
                new PropertyMetadata(45.0));

        public double ButtonHeight
        {
            get => (double)GetValue(ButtonHeightProperty);
            set => SetValue(ButtonHeightProperty, value);
        }

        public static readonly DependencyProperty ButtonWidthProperty =
            DependencyProperty.Register(nameof(ButtonWidth), typeof(double), typeof(OutlinedIconButton),
                new PropertyMetadata(125.0));

        public double ButtonWidth
        {
            get => (double)GetValue(ButtonWidthProperty);
            set => SetValue(ButtonWidthProperty, value);
        }

        public static readonly DependencyProperty ButtonMarginProperty =
            DependencyProperty.Register(nameof(ButtonMargin), typeof(Thickness), typeof(OutlinedIconButton),
                new PropertyMetadata(new Thickness(0)));

        public Thickness ButtonMargin
        {
            get => (Thickness)GetValue(ButtonMarginProperty);
            set => SetValue(ButtonMarginProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(OutlinedIconButton),
                new PropertyMetadata(new CornerRadius(8)));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        // ─── Command ──────────────────────────────────────────────
        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register(nameof(ButtonCommand), typeof(ICommand), typeof(OutlinedIconButton),
                new PropertyMetadata(null));

        public ICommand ButtonCommand
        {
            get => (ICommand)GetValue(ButtonCommandProperty);
            set => SetValue(ButtonCommandProperty, value);
        }
    }
}
