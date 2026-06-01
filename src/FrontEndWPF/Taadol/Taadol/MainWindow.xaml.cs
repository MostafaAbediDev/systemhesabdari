using System.Windows;
using Taadol.Views;

namespace Taadol
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Sidebar.SidebarWidthChanged += (width) =>
            {
                SidebarColumn.Width = new GridLength(width);
                SidebarColumn.MinWidth = 0;
                SidebarColumn.MaxWidth = double.PositiveInfinity;
            };

            Sidebar.SubMenuClicked += (tag) =>
{
};

            MainContentBorder.Visibility = Visibility.Collapsed;

            Sidebar.SubMenuClicked += OnSubMenuClicked;
        }

        private void OnSubMenuClicked(string tag)
        {
            MainContent.Content = null;



            switch (tag)
            {
                case "dashboard_overview":
                    MessageBox.Show("داشبورد مدیریت - کسب و کارها / شعبه‌ها");
                    MainContentBorder.Visibility = Visibility.Visible;
                    break;

                case "dashboard_sales":
                    MessageBox.Show("داشبورد مدیریت - کسب و کار جدید");
                    MainContentBorder.Visibility = Visibility.Visible;
                    break;

                case "product_new":
                    MainContent.Content = new NewProductView();
                    break;

                case "product_list":
                    MainContent.Content = new ProductListView();
                    break;

                case "service_new":
                    MainContent.Content = new NewServiceView();
                    break;

                case "person_new":
                    MainContent.Content = new NewPersonView();
                    break;

                case "person_list":
                    MainContent.Content = new PersonListView();
                    break;
                case "financial_period":
                    MainContent.Content = new NewFinancialPeriodView();
                    break;
                case "company_info":
                    MainContent.Content = new NewCompanyView();
                    break;


                case "branch_new":
                    MainContent.Content = new NewBranchView();
                    break;

                case "branch_list":
                    MainContent.Content = new BranchListView();
                    break;















                default:
                    MessageBox.Show($"منوی {tag} هنوز پیاده‌سازی نشده");
                    break;
            }

            if (tag != null)
            {
                MainContentBorder.Visibility = Visibility.Visible;
            }
        }

        private void Sidebar_Loaded(object sender, RoutedEventArgs e)
        {
            Sidebar.SidebarWidthChanged += width =>
            {
                SidebarColumn.Width = new GridLength(width);
            };
        }

        private void NewProductView_Loaded(object sender, RoutedEventArgs e)
        {
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