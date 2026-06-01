using System.Windows;
using System.Windows.Controls;

namespace Taadol.Controls
{
    public partial class ModernTabControl : UserControl
    {
        public ModernTabControl()
        {
            InitializeComponent();
        }

        private void TabPricing_Checked(object sender, RoutedEventArgs e)
        {
            if (TabInventory == null || TabTax == null) return;

            TabInventory.IsChecked = false;
            TabTax.IsChecked = false;

            PricingContent.Visibility = Visibility.Visible;
            InventoryContent.Visibility = Visibility.Collapsed;
            TaxContent.Visibility = Visibility.Collapsed;
        }

        private void TabInventory_Checked(object sender, RoutedEventArgs e)
        {
            if (TabPricing == null || TabTax == null) return;

            TabPricing.IsChecked = false;
            TabTax.IsChecked = false;

            PricingContent.Visibility = Visibility.Collapsed;
            InventoryContent.Visibility = Visibility.Visible;
            TaxContent.Visibility = Visibility.Collapsed;
        }

        private void TabTax_Checked(object sender, RoutedEventArgs e)
        {
            if (TabPricing == null || TabInventory == null) return;

            TabPricing.IsChecked = false;
            TabInventory.IsChecked = false;

            PricingContent.Visibility = Visibility.Collapsed;
            InventoryContent.Visibility = Visibility.Collapsed;
            TaxContent.Visibility = Visibility.Visible;
        }
    }
}
