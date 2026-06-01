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
    public partial class IconButton : UserControl
    {
        public IconButton()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ButtonTextProperty =
    DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(IconButton), new PropertyMetadata("دکمه"));

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty IconSourceProperty =
    DependencyProperty.Register(nameof(IconSource), typeof(Uri), typeof(IconButton), new PropertyMetadata(null));

        public Uri IconSource
        {
            get => (Uri)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        public static readonly DependencyProperty ButtonCommandProperty =
    DependencyProperty.Register(nameof(ButtonCommand), typeof(ICommand), typeof(IconButton), new PropertyMetadata(null));

        public ICommand ButtonCommand
        {
            get => (ICommand)GetValue(ButtonCommandProperty);
            set => SetValue(ButtonCommandProperty, value);
        }

        public static readonly DependencyProperty ButtonBackgroundProperty =
    DependencyProperty.Register(nameof(ButtonBackground), typeof(Brush), typeof(IconButton),
        new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0x26, 0x67, 0xFF))));

        public Brush ButtonBackground
        {
            get => (Brush)GetValue(ButtonBackgroundProperty);
            set => SetValue(ButtonBackgroundProperty, value);
        }

        public static readonly DependencyProperty ButtonForegroundProperty =
    DependencyProperty.Register(nameof(ButtonForeground), typeof(Brush), typeof(IconButton),
        new PropertyMetadata(Brushes.White));

        public Brush ButtonForeground
        {
            get => (Brush)GetValue(ButtonForegroundProperty);
            set => SetValue(ButtonForegroundProperty, value);
        }

        public static readonly DependencyProperty ButtonHeightProperty =
    DependencyProperty.Register(nameof(ButtonHeight), typeof(double), typeof(IconButton), new PropertyMetadata(45.0));

        public double ButtonHeight
        {
            get => (double)GetValue(ButtonHeightProperty);
            set => SetValue(ButtonHeightProperty, value);
        }

        public static readonly DependencyProperty ButtonWidthProperty =
            DependencyProperty.Register(nameof(ButtonWidth), typeof(double), typeof(IconButton), new PropertyMetadata(86.0));

        public double ButtonWidth
        {
            get => (double)GetValue(ButtonWidthProperty);
            set => SetValue(ButtonWidthProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
    DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(IconButton),
        new PropertyMetadata(new CornerRadius(8)));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty IconSizeProperty =
    DependencyProperty.Register(nameof(IconSize), typeof(double), typeof(IconButton), new PropertyMetadata(20.0));

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public static readonly DependencyProperty TextSizeProperty =
    DependencyProperty.Register(nameof(TextSize), typeof(double), typeof(IconButton), new PropertyMetadata(16.0));

        public double TextSize
        {
            get => (double)GetValue(TextSizeProperty);
            set => SetValue(TextSizeProperty, value);
        }
    }
}
