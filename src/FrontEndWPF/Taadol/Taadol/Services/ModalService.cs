using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Taadol.Services
{
    /// <summary>
    /// Manages the modal overlay layer in MainWindow.
    /// Extracted from MainWindow to reduce duplication (open block was repeated 7 times).
    /// </summary>
    public class ModalService : IModalService
    {
        private readonly ContentControl _modalContent;
        private readonly Border _modalOverlay;

        public ModalService(ContentControl modalContent, Border modalOverlay)
        {
            _modalContent = modalContent ?? throw new ArgumentNullException(nameof(modalContent));
            _modalOverlay = modalOverlay ?? throw new ArgumentNullException(nameof(modalOverlay));
        }

        public bool IsOpen => _modalContent.Content != null;

        public UserControl? Current => _modalContent.Content as UserControl;

        public void Open(UserControl view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));

            _modalContent.Content = view;
            _modalOverlay.BeginAnimation(UIElement.OpacityProperty, null);
            _modalOverlay.Opacity = 1;
            _modalOverlay.Visibility = Visibility.Visible;
        }

        public void Close()
        {
            if (_modalOverlay == null) return;

            // Detach the content immediately so its Unloaded handlers cancel work
            // before the fade animation completes.
            var contentBeingClosed = _modalContent.Content;
            _modalContent.Content = null;

            // بستن نرم با فید-اوت تا فرم «یهویی» ناپدید نشود
            var fade = new DoubleAnimation(
                0, TimeSpan.FromMilliseconds(220));
            fade.EasingFunction = new QuadraticEase
            {
                EasingMode = EasingMode.EaseIn
            };
            fade.Completed += (s, e) =>
            {
                _modalOverlay.Visibility = Visibility.Collapsed;
                _modalOverlay.Opacity = 1;
            };
            _modalOverlay.BeginAnimation(UIElement.OpacityProperty, fade);
        }
    }
}