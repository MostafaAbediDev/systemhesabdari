using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Taadol.Controls;
using Taadol.Helpers;
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
            ViewModel.CancelRequested += HandleCancelRequested;
            ViewModel.BankSaved += OnBankSaved;
            DataContext = ViewModel;
        }

        private async void View_Loaded(object sender, RoutedEventArgs e) => await ViewModel.LoadAsync();

        private void View_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.CancelRequested -= HandleCancelRequested;
            ViewModel.BankSaved -= OnBankSaved;
            ViewModel.Dispose();
        }

        private void OnBankSaved()
        {
            if (Window.GetWindow(this) is not MainWindow mainWindow)
                return;

            if (mainWindow.MainContent.Content is BankListView listView)
            {
                _ = RefreshBankListSafeAsync(listView);
                return;
            }

            mainWindow.NavigateTo(NavKeys.BankList);
        }

        private async Task RefreshBankListSafeAsync(BankListView listView)
        {
            try
            {
                await listView.RefreshGridAsync();
            }
            catch (OperationCanceledException)
            {
                // لغو رفرش هنگام بسته‌شدن یا جابه‌جایی صفحه، رفتار عادی است.
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewBankView] خطا در رفرش لیست بانک: {ex}");
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                    ToastManager.Error("خطا در بروزرسانی لیست بانک‌ها")));
            }
        }

        private void HeaderClose_Click(object sender, MouseButtonEventArgs e) => HandleCancelRequested();
        private void Cancel_Click(object sender, RoutedEventArgs e) => HandleCancelRequested();

        private void HandleCancelRequested()
        {
            if (ViewModel.HasUnsavedChanges)
            {
                var result = MessageBox.Show(
                    "تغییرات ذخیره‌نشده وجود دارد. آیا می‌خواهید از فرم خارج شوید؟",
                    "تأیید خروج",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            CloseForm();
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
