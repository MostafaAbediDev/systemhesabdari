using System.Windows.Controls;

namespace Taadol.Services
{
    /// <summary>
    /// Manages the modal overlay layer in MainWindow.
    /// Extracted from MainWindow to reduce duplication (open block was repeated 7 times).
    /// </summary>
    public interface IModalService
    {
        /// <summary>Opens a modal view with fade-in.</summary>
        void Open(UserControl view);

        /// <summary>Closes the current modal with fade-out (identical to previous CloseModal behavior).</summary>
        void Close();

        /// <summary>True when a modal view is currently displayed.</summary>
        bool IsOpen { get; }

        /// <summary>The currently displayed modal view (or null).</summary>
        UserControl? Current { get; }
    }
}