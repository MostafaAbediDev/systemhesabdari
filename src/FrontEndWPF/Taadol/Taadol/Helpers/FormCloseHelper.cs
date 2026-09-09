using System;
using System.Threading.Tasks;
using System.Windows;
using Taadol.Controls;

namespace Taadol.Helpers
{
    /// <summary>
    /// راهنمای بستن فرم‌های دارای تغییرات ذخیره‌نشده (پرسش Yes/No/Cancel).
    /// رفتار با HandleCancelAsync قبلی دو فرم بانک یکسان است:
    /// - بدون تغییر ذخیره‌نشده → بستن مستقیم
    /// - Cancel → فرم باز می‌ماند
    /// - Yes → ذخیره انجام می‌شود ولی فرم باز می‌ماند؛ بستن پس از ذخیرهٔ موفق توسط
    ///   رویدادهای موفقیت (BankSaved/BankUpdated در MainWindow) انجام می‌شود.
    /// - No → بدون ذخیره بسته می‌شود
    /// </summary>
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

            // No → بدون ذخیره ببند.
            closeAction();
        }
    }
}
