using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Taadol.ViewModels.Product;

namespace Taadol.Views
{
    public partial class ProductListView : UserControl
    {
        private ProductListViewModel ViewModel { get; }

        public ProductListView()
        {
            InitializeComponent();
            ViewModel = new ProductListViewModel();
            DataContext = ViewModel;

            ProductsGrid.NextPageRequested += (s, e) => ViewModel.GoToNextPageCommand.Execute(null);
            ProductsGrid.PreviousPageRequested += (s, e) => ViewModel.GoToPreviousPageCommand.Execute(null);
            ProductsGrid.PageRequested += (s, page) => ViewModel.GoToPageCommand.Execute(page);
            ProductsGrid.PageSizeRequested += (s, size) => ViewModel.ChangePageSize(size);
        }

        private void HeaderClose_Click(object sender, MouseButtonEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.CloseCurrentForm();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.RefreshCommand.Execute(null);
        }
    }
}
