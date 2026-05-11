using System.Windows;
<<<<<<< HEAD
=======
using Taadol_Cal.Views;
>>>>>>> master

namespace Taadol_Cal
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

            // گوش دادن به کلیک زیرمنو
            Sidebar.SubMenuClicked += (tag) =>
            {
                // TODO: Navigate based on tag
            };
            // پنهان کردن محتوای پیش‌فرض
            MainContentBorder.Visibility = Visibility.Collapsed;

            // Subscribe به event کلیک منو
            Sidebar.SubMenuClicked += OnSubMenuClicked;
        }

        private void OnSubMenuClicked(string tag)
        {
            // پاک کردن محتوای قبلی
            MainContent.Content = null;

            // نمایش view مناسب بر اساس tag
            switch (tag)
            {
                case "dashboard":
                    MessageBox.Show("داشبورد - در حال توسعه");
                    MainContentBorder.Visibility = Visibility.Visible;
                    break;

<<<<<<< HEAD
                //case "product_new":
                //    MainContent.Content = new NewProductView();
                //    break;

                //case "product_list":
                //    MainContent.Content = new ProductListView();
                //    break;
                //case "service_new":
                //    MainContent.Content = new NewServiceView();
                //    break;
                //case "person_new":
                //    MainContent.Content = new NewPersonView();
                //    break;
                //case "person_list":
                //    MainContent.Content = new PersonListView();
                //    break;
=======
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
>>>>>>> master

                // سایر موارد...
                default:
                    MessageBox.Show($"منوی {tag} هنوز پیاده‌سازی نشده");
                    break;
            }

            // نمایش بخش محتوا
            MainContentBorder.Visibility = Visibility.Visible;
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
            // حذف شد - دیگر نیازی نیست
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}