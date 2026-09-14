using System.Windows;

namespace Taadol.Controls
{

    public static class FilterTabBehavior
    {
        public static readonly DependencyProperty ShowCloseProperty =
            DependencyProperty.RegisterAttached(
                "ShowClose",
                typeof(bool),
                typeof(FilterTabBehavior),
                new PropertyMetadata(true));

        public static void SetShowClose(DependencyObject obj, bool value) => obj.SetValue(ShowCloseProperty, value);
        public static bool GetShowClose(DependencyObject obj) => (bool)obj.GetValue(ShowCloseProperty);
    }
}
