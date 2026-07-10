using System.Windows;
using System.Windows.Controls;

namespace Taadol.Controls
{
    public static class ToastManager
    {
        private static StackPanel _container;

        public static void Initialize(StackPanel toastContainer)
        {
            _container = toastContainer;
        }

        public static void Show(string message, ToastType type = ToastType.Success, int durationMs = 3000)
        {
            if (_container == null) return;

            var toast = new ToastNotification(message, type, durationMs);
            _container.Children.Add(toast);
            toast.Show();
        }

        public static void Success(string message, int durationMs = 4000)
            => Show(message, ToastType.Success, durationMs);

        public static void Error(string message, int durationMs = 5000)
            => Show(message, ToastType.Error, durationMs);

        public static void Warning(string message, int durationMs = 4500)
            => Show(message, ToastType.Warning, durationMs);

        public static void Info(string message, int durationMs = 4000)
            => Show(message, ToastType.Info, durationMs);
    }
}
