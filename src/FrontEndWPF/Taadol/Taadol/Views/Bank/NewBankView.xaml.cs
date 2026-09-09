using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading.Tasks;
using Taadol.Controls;
using Taadol.Helpers;
using Taadol.ViewModels;

namespace Taadol.Views.Bank
{
    public partial class NewBankView : UserControl, IUnsavedChangesAware
    {
        public NewBankViewModel ViewModel { get; }

        public bool HasUnsavedChanges => ViewModel?.HasUnsavedChanges ?? false;

        public NewBankView()
        {
            InitializeComponent();
            ViewModel = new NewBankViewModel(App.ServiceProvider);
            DataContext = ViewModel;
        }

        private void View_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Dispose();
        }

        private async void HeaderClose_Click(object sender, RoutedEventArgs e)
            => await HandleCancelAsync();
        private async void Cancel_Click(object sender, RoutedEventArgs e)
            => await HandleCancelAsync();

        // کلید Escape مسیر لغو/بستن و کلید Enter مسیر ذخیره را اجرا میکند؛
        // Enter در فیلدهای چندخطی به خط بعدی میگذارد.
        private void View_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                _ = HandleCancelAsync();
                return;
            }

            if (e.Key != Key.Enter)
                return;

            if (Keyboard.FocusedElement is TextBox { AcceptsReturn: true })
                return; // فیلد چندخطی: Enter باید خط جدید ایجاد کند

            if (ViewModel?.IsFormInteractive == true && ViewModel.SaveCommand.CanExecute(null))
            {
                e.Handled = true;
                ViewModel.SaveCommand.Execute(null);
            }
        }

        private async Task HandleCancelAsync()
        {
            await FormCloseHelper.ConfirmAndCloseAsync(
                ViewModel,
                () => ViewModel.SaveAsync(),
                CloseForm);
        }

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
