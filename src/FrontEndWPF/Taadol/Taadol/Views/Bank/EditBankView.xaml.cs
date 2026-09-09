using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Taadol.Controls;
using Taadol.Helpers;
using Taadol.ViewModels;

namespace Taadol.Views.Bank
{
    public partial class EditBankView : UserControl, IUnsavedChangesAware
    {
        public EditBankViewModel ViewModel { get; }

        public bool HasUnsavedChanges => ViewModel?.HasUnsavedChanges ?? false;

        public EditBankView(long bankId)
        {
            InitializeComponent();
            ViewModel = new EditBankViewModel(App.ServiceProvider, bankId);
            DataContext = ViewModel;

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            ViewModel.LoadFailed += OnLoadFailed;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            try
            {
                await ViewModel.LoadAsync(CancellationToken.None);
            }
            catch (OperationCanceledException)
            {
                // لغو هنگام خروج از فرم رفتار عادی است.
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"[EditBankView] خطا در بارگذاری بانک: {exception}");
                // خطای بارگذاری در ViewModel از طریق LoadFailed مدیریت می‌شود.
            }
        }

        private void OnLoadFailed()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var message = string.IsNullOrWhiteSpace(ViewModel.LoadErrorText)
                    ? "خطا در بارگذاری اطلاعات بانک."
                    : ViewModel.LoadErrorText;

                ToastManager.Error(message);
                RequestClose();
            }));
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadFailed -= OnLoadFailed;
            Unloaded -= OnUnloaded;
            ViewModel.Dispose();
        }

        // این handler در XAML با نام View_Unloaded ثبت شده است؛ برای حفظ قالب Phase 2
        // به همان مسیر Cleanup متصل می‌شود.
        private void View_Unloaded(object sender, RoutedEventArgs e)
        {
            OnUnloaded(sender, e);
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
                RequestClose);
        }

        private void RequestClose()
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                if (mainWindow.ModalContent.Content == this)
                    mainWindow.CloseModal();
                else
                    mainWindow.CloseCurrentForm();
            }
        }
    }
}
