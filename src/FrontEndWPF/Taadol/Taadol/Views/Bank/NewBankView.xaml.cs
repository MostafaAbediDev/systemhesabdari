using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Taadol.ViewModels;

namespace Taadol.Views.Bank
{
    public partial class NewBankView : UserControl
    {
        public NewBankViewModel ViewModel { get; }

        public NewBankView()
        {
            InitializeComponent();
            ViewModel = new NewBankViewModel(App.ServiceProvider);
            DataContext = ViewModel;
        }

        private async void View_Loaded(object sender, RoutedEventArgs e) => await ViewModel.LoadAsync();
        private void View_Unloaded(object sender, RoutedEventArgs e) => ViewModel.Dispose();
        private void HeaderClose_Click(object sender, MouseButtonEventArgs e) => CloseForm();
        private void Cancel_Click(object sender, RoutedEventArgs e) => CloseForm();

        private void CloseForm()
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                if (mainWindow.ModalContent.Content == this) mainWindow.CloseModal();
                else mainWindow.CloseCurrentForm();
            }
        }
    }
}
