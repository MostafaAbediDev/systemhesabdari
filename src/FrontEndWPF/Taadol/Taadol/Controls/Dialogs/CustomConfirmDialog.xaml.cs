using System.Windows;

namespace Taadol.Controls
{
    public partial class CustomConfirmDialog : Window
    {
        public bool Result { get; private set; }

        public CustomConfirmDialog()
        {
            InitializeComponent();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            DialogResult = false;
            Close();
        }
    }
}