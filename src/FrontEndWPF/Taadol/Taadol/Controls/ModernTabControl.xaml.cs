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
            if (PricingContent == null || InventoryContent == null || TaxContent == null) return;

            PricingContent.Visibility = Visibility.Visible;
            InventoryContent.Visibility = Visibility.Collapsed;
            TaxContent.Visibility = Visibility.Collapsed;
        }

        private void TabInventory_Checked(object sender, RoutedEventArgs e)
        {
            if (PricingContent == null || InventoryContent == null || TaxContent == null) return;

            PricingContent.Visibility = Visibility.Collapsed;
            InventoryContent.Visibility = Visibility.Visible;
            TaxContent.Visibility = Visibility.Collapsed;
        }

        private void TabTax_Checked(object sender, RoutedEventArgs e)
        {
            if (PricingContent == null || InventoryContent == null || TaxContent == null) return;

            PricingContent.Visibility = Visibility.Collapsed;
            InventoryContent.Visibility = Visibility.Collapsed;
            TaxContent.Visibility = Visibility.Visible;
        }
    }
}