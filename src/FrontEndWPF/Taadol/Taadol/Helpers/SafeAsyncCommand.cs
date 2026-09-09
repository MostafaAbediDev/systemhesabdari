using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Taadol.Controls;

namespace Taadol.Helpers
{
    internal sealed class SafeAsyncCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;

        public SafeAsyncCommand(Func<Task> execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute();

        public void Execute(object? parameter) => _ = ExecuteSafeAsync();

        private async Task ExecuteSafeAsync()
        {
            try
            {
                await _execute();
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SafeAsyncCommand] خطای کنترل‌نشده: {ex}");
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher == null)
                    ToastManager.Error("خطای غیرمنتظره در اجرای عملیات");
                else
                    dispatcher.BeginInvoke(new Action(() => ToastManager.Error("خطای غیرمنتظره در اجرای عملیات")));
            }
        }

        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
