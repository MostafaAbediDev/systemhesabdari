using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Taadol.Controls
{
    public partial class ActionButton : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(ActionButton),
                new PropertyMetadata(string.Empty, OnTextChanged));

        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register(nameof(TextColor), typeof(Brush), typeof(ActionButton),
                new PropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register(nameof(BorderColor), typeof(Brush), typeof(ActionButton),
                new PropertyMetadata(Brushes.LightGray));

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register(nameof(IconPath), typeof(string), typeof(ActionButton),
                new PropertyMetadata(string.Empty, OnIconChanged));

        public static readonly DependencyProperty IconColorProperty =
            DependencyProperty.Register(nameof(IconColor), typeof(Brush), typeof(ActionButton),
                new PropertyMetadata(Brushes.Black));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ActionButton),
                new PropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(ActionButton),
                new PropertyMetadata(null));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Brush TextColor
        {
            get => (Brush)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public Brush BorderColor
        {
            get => (Brush)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }

        public string IconPath
        {
            get => (string)GetValue(IconPathProperty);
            set => SetValue(IconPathProperty, value);
        }

        public Brush IconColor
        {
            get => (Brush)GetValue(IconColorProperty);
            set => SetValue(IconColorProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public event RoutedEventHandler Click;

        public ActionButton()
        {
            InitializeComponent();
            Loaded += (_, _) => ApplyValues();
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ActionButton btn)
                btn.BtnText.Text = e.NewValue as string ?? string.Empty;
        }

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ActionButton btn)
                btn.ApplyIcon();
        }

        private void ApplyValues()
        {
            BtnText.Text = Text;
            BtnText.Foreground = TextColor;
            BtnBorder.BorderBrush = BorderColor;
            ApplyIcon();
        }

        private void ApplyIcon()
        {
            if (!string.IsNullOrEmpty(IconPath))
            {
                BtnIcon.Source = new Uri(IconPath, UriKind.Relative);
                BtnIcon.Visibility = Visibility.Visible;
                BtnText.Margin = new Thickness(0, 0, 6, 0);
            }
            else
            {
                BtnIcon.Visibility = Visibility.Collapsed;
                BtnText.Margin = new Thickness(0);
            }
        }

        private void BtnBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Command != null)
            {
                if (Command.CanExecute(CommandParameter))
                    Command.Execute(CommandParameter);
                return;
            }

            Click?.Invoke(this, e);
        }

        private void BtnBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            BtnBorder.Background = new SolidColorBrush(Color.FromArgb(20, 0, 0, 0));
        }

        private void BtnBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            BtnBorder.Background = Brushes.White;
        }
    }
}
