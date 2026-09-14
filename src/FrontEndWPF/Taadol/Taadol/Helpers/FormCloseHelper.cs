using System;
using System.Threading.Tasks;
using System.Windows;
using Taadol.Controls;

namespace Taadol.Helpers
{

    public static class FormCloseHelper
    {
        public static async Task ConfirmAndCloseAsync(
            IUnsavedChangesAware? vm,
            Func<Task<bool>> saveAsync,
            Action closeAction)
        {
            if (vm == null || !vm.HasUnsavedChanges)
            {
                closeAction();
                return;
            }

            var result = MessageBox.Show(
                "تغییراتی که ایجاد کرده‌اید ذخیره نشده است.\nآیا می‌خواهید آن‌ها را ذخیره کنید؟",
                "ذخیره تغییرات",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Cancel)
                return;

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await saveAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[FormCloseHelper] خطا در عملیات انصراف: {ex}");
                    Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                        ToastManager.Error("خطا در عملیات")));
                }

                return;
            }

            closeAction();
        }
    }
}
