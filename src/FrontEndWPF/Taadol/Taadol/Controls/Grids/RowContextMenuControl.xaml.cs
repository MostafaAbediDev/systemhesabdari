using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Taadol.Controls
{

    public partial class RowContextMenuControl : UserControl
    {
        public event EventHandler EditRequested;
        public event EventHandler DeleteRequested;

        private Window _hostWindow;

        public RowContextMenuControl()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                MenuCard.Visibility = Visibility.Collapsed;
                RootGrid.IsHitTestVisible = false;
            }
        }

        public void ShowMenu(bool showEdit, bool showDelete, Point position)
        {
            if (!showEdit && !showDelete)
                return;

            EditButton.Visibility = showEdit ? Visibility.Visible : Visibility.Collapsed;
            DeleteButton.Visibility = showDelete ? Visibility.Visible : Visibility.Collapsed;
            DividerBorder.Visibility = showEdit && showDelete ? Visibility.Visible : Visibility.Collapsed;

            double menuWidth = 105;
            double menuHeight = 100;
            double maxX = ActualWidth - menuWidth - 4;
            double maxY = ActualHeight - menuHeight - 4;

            double x = position.X + 6;
            double y = position.Y + 6;

            PositionTransform.X = Math.Min(x, Math.Max(0, maxX));
            PositionTransform.Y = Math.Min(y, Math.Max(0, maxY));

            RootGrid.IsHitTestVisible = true;
            RootGrid.Background = Brushes.Transparent;
            MenuCard.Visibility = Visibility.Visible;

            AttachCloseHandlers();
        }

        private void HideMenu()
        {
            DetachCloseHandlers();

            MenuCard.Visibility = Visibility.Collapsed;
            RootGrid.Background = null;
            RootGrid.IsHitTestVisible = false;
        }

        private void AttachCloseHandlers()
        {
            _hostWindow = Window.GetWindow(this);
            if (_hostWindow == null)
                return;

            _hostWindow.PreviewMouseLeftButtonDown += Window_PreviewMouseDown;
            _hostWindow.PreviewMouseRightButtonDown += Window_PreviewMouseDown;
            _hostWindow.PreviewKeyDown += Window_PreviewKeyDown;
            _hostWindow.Deactivated += Window_Deactivated;
        }

        private void DetachCloseHandlers()
        {
            if (_hostWindow == null)
                return;

            _hostWindow.PreviewMouseLeftButtonDown -= Window_PreviewMouseDown;
            _hostWindow.PreviewMouseRightButtonDown -= Window_PreviewMouseDown;
            _hostWindow.PreviewKeyDown -= Window_PreviewKeyDown;
            _hostWindow.Deactivated -= Window_Deactivated;
            _hostWindow = null;
        }

        private void Window_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            if (IsClickInsideMenu(e.OriginalSource as DependencyObject))
                return;

            HideMenu();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                HideMenu();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            HideMenu();
        }

        private bool IsClickInsideMenu(DependencyObject source)
        {
            while (source != null)
            {
                if (source == MenuCard)
                    return true;

                if (source is Visual || source is System.Windows.Media.Media3D.Visual3D)
                    source = VisualTreeHelper.GetParent(source);
                else
                    source = LogicalTreeHelper.GetParent(source);
            }
            return false;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            HideMenu();
            EditRequested?.Invoke(this, EventArgs.Empty);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            HideMenu();
            DeleteRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
