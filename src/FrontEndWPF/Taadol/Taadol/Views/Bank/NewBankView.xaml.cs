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

        private async void HeaderClose_Click(object sender, MouseButtonEventArgs e)
            => await HandleCancelAsync();

        private async void Cancel_Click(object sender, RoutedEventArgs e)
            => await HandleCancelAsync();

        private async Task HandleCancelAsync()
        {
            try
            {
                if (ViewModel.HasUnsavedChanges)
                {
                    var result = MessageBox.Show(
                        "تغییراتی که ایجاد کرده‌اید ذخیره نشده است.\nآیا می‌خواهید آن‌ها را ذخیره کنید؟",
                        "ذخیره تغییرات",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        await ViewModel.SaveAsync();
                        return;
                    }

                    if (result == MessageBoxResult.Cancel)
                        return;
                }

                CloseForm();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewBankView] خطا در عملیات انصراف: {ex}");
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                    ToastManager.Error("خطا در عملیات")));
            }
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
