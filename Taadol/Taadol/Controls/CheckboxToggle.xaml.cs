using System.Windows;
using System.Windows.Controls;

namespace Taadol.Controls
{
    public partial class CheckboxToggle : UserControl
    {
        private bool _isSyncing = false;

        public CheckboxToggle()
        {
            InitializeComponent();
        }

        // ─── IsChecked ────────────────────────────────────────
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(CheckboxToggle),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnIsCheckedChanged));

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        private static void OnIsCheckedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (CheckboxToggle)d;
            if (ctrl._isSyncing) return;
            ctrl._isSyncing = true;
            ctrl.InnerToggle.IsChecked = (bool)e.NewValue;
            ctrl._isSyncing = false;
        }

        private void InnerToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (_isSyncing) return;
            _isSyncing = true;
            IsChecked = true;
            _isSyncing = false;
        }

        private void InnerToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_isSyncing) return;
            _isSyncing = true;
            IsChecked = false;
            _isSyncing = false;
        }

        // ─── LabelContent ─────────────────────────────────────
        public static readonly DependencyProperty LabelContentProperty =
            DependencyProperty.Register(nameof(LabelContent), typeof(string), typeof(CheckboxToggle),
                new PropertyMetadata("وضعیت"));

        public string LabelContent
        {
            get => (string)GetValue(LabelContentProperty);
            set => SetValue(LabelContentProperty, value);
        }
    }
}