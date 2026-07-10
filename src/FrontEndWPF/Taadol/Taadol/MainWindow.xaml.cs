using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Taadol.Controls;
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

            ToastManager.Initialize(ToastContainer);

            _factory = new ViewFactory(App.ServiceProvider);

            _factory.Register("person_list", () => new PersonListView());
            _factory.Register("person_new", () => new NewPersonView());

            _factory.Register("product_list", () => new ProductListView());
            _factory.Register("product_new", () => new NewProductView());

            _factory.Register("company_info", () => new NewCompanyView());
            _factory.Register("company_list", () => new CompanyListView());

            _factory.Register("branch_new", () => new NewBranchView());
            _factory.Register("branch_list", () => new BranchListView());

            _factory.Register("financial_period", () => new NewFinancialPeriodView());

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

        public void NavigateTo(string tag)
        {
            MainContentBorder.Visibility = Visibility.Visible;
            _nav.Navigate(tag);
        }

        public void NavigateToEditPerson(long personId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] NavigateToEditPerson called with personId={personId}");
                ToastManager.Info($"Opening edit for person {personId}...");

                var editView = new EditPersonView(personId);
                System.Diagnostics.Debug.WriteLine("[DEBUG] EditPersonView created successfully");
                ModalContent.Content = editView;
                System.Diagnostics.Debug.WriteLine("[DEBUG] ModalContent.Content set");
                ModalOverlay.Visibility = Visibility.Visible;
                System.Diagnostics.Debug.WriteLine("[DEBUG] ModalOverlay set to Visible");
                
                // Debug: check ContentControl size after layout
                Dispatcher.InvokeAsync(() =>
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] ModalContent ActualWidth={ModalContent.ActualWidth}, ActualHeight={ModalContent.ActualHeight}");
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] ModalOverlay ActualWidth={ModalOverlay.ActualWidth}, ActualHeight={ModalOverlay.ActualHeight}");
                }, System.Windows.Threading.DispatcherPriority.Loaded);
                
                ToastManager.Success("فرم ویرایش باز شد");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] {ex}");
                ToastManager.Error($"Error: {ex.Message}");
                
                ModalContent.Content = new Border
                {
                    Background = System.Windows.Media.Brushes.Red,
                    MinWidth = 400,
                    MinHeight = 300,
                    Child = new TextBlock
                    {
                        Text = $"ERROR:\n{ex.Message}",
                        Foreground = System.Windows.Media.Brushes.White,
                        FontSize = 16,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        Margin = new Thickness(20)
                    }
                };
                ModalOverlay.Visibility = Visibility.Visible;
            }
        }

        public void CloseModal()
        {
            ModalOverlay.Visibility = Visibility.Collapsed;
            ModalContent.Content = null;
        }

        private void ModalOverlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CloseModal();
        }

        private void ModalPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
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