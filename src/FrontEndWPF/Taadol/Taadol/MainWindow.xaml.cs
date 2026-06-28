using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using Taadol.Services;
using Taadol.Views;
namespace Taadol
{
    public partial class MainWindow : Window
    {

        private NavigationService _nav;
        private ViewFactory _factory;

        public MainWindow()
        {
            InitializeComponent();

            // تغییر اینجا: App.ServiceProvider رو پاس دادیم
            _factory = new ViewFactory(App.ServiceProvider);

            // ثبت همه صفحات (دقیقاً مثل قبل خودتان)
            _factory.Register("person_list", () => new PersonListView());
            _factory.Register("person_new", () => new NewPersonView());

            _factory.Register("product_list", () => new ProductListView());
            _factory.Register("product_new", () => new NewProductView());

            _factory.Register("company_info", () => new NewCompanyView());
            _factory.Register("company_list", () => new CompanyListView());

            _factory.Register("branch_new", () => new NewBranchView());
            _factory.Register("branch_list", () => new BranchListView());

            _factory.Register("financial_period", () => new NewFinancialPeriodView());

            // تغییر اینجا: _factory رو به عنوان پارامتر دوم دادیم
            _nav = new NavigationService(MainContent, _factory);

            Sidebar.SidebarWidthChanged += (width) =>
            {
                SidebarColumn.Width = new GridLength(width);
                SidebarColumn.MinWidth = 0;
                SidebarColumn.MaxWidth = double.PositiveInfinity;
            };

            this.Loaded += (s, e) =>
            {
                Sidebar.SubMenuClicked -= OnSubMenuClicked;
                Sidebar.SubMenuClicked += OnSubMenuClicked;
            };

            MainContentBorder.Visibility = Visibility.Collapsed;
        }

        private void OnSubMenuClicked(string tag)
        {
            MainContentBorder.Visibility = Visibility.Visible;

            _nav.Navigate(tag);

            Dispatcher.InvokeAsync(() =>
            {
                MainContentBorder.InvalidateVisual();
                MainContentBorder.UpdateLayout();
            });
        }

        private void Sidebar_Loaded(object sender, RoutedEventArgs e)
        {
            Sidebar.SidebarWidthChanged += width =>
            {
                SidebarColumn.Width = new GridLength(width);
            };
        }

       

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("آیا از خروج از برنامه مطمئن هستید؟",
                "تأیید خروج",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}