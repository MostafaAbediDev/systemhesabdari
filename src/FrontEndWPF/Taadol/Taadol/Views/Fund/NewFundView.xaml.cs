using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Taadol.Helpers;
using Taadol.ViewModels;

namespace Taadol.Views.Fund
{
    public partial class NewFundView : UserControl
    {
        public NewFundViewModel ViewModel { get; }

        public NewFundView()
        {
            InitializeComponent();
            ViewModel = new NewFundViewModel(App.ServiceProvider);
            ViewModel.CancelRequested += CloseForm;
            ViewModel.Saved += OnSaved;
            DataContext = ViewModel;
        }

        private async void View_Loaded(object sender, RoutedEventArgs e) => await ViewModel.LoadAsync();

        private void View_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.CancelRequested -= CloseForm;
            ViewModel.Saved -= OnSaved;
            ViewModel.Dispose();
        }

        private void OnSaved()
        {

            (Window.GetWindow(this) as MainWindow)?.NavigateTo(NavKeys.FundList);
        }

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
