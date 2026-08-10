using GeneralInfoManagement.Application.Contract.Company;
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
        private ICompanyApplication _companyApplication;

        public MainWindow()
        {
            InitializeComponent();

            ToastManager.Initialize(ToastContainer);

            _factory = new ViewFactory(App.ServiceProvider);
            _companyApplication = App.ServiceProvider.GetRequiredService<ICompanyApplication>();

            _factory.Register("person_list", () => new PersonListView());
            _factory.Register("person_new", () => new NewPersonView());

            _factory.Register("product_list", () => new ProductListView());
            _factory.Register("product_new", () => new NewProductView());

            _factory.Register("company_info", () => new NewCompanyView());
            _factory.Register("company_list", () => new CompanyListView());

            _factory.Register("branch_new", () => new NewBranchView());
            _factory.Register("branch_list", () => new BranchListView());
            _factory.Register("branch_archive", () => new BranchArchiveListView());

            _factory.Register("financial_period", () => new FinancialPeriodListView());
            _factory.Register("financial_period_new", () => new NewFinancialPeriodView());

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
                LoadCompanies();
            };

            this.Closing += MainWindow_Closing;

            MainContentBorder.Visibility = Visibility.Collapsed;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ModalOverlay?.Visibility == Visibility.Visible && ModalContent.Content is EditPersonView editView)
            {
                if (editView.IsDirty)
                {
                    var result = Controls.ModernDialog.ShowConfirm(
                        "تأیید خروج",
                        "شما تغییرات ذخیره‌نشده دارید. آیا از خروج مطمئن هستید؟",
                        Controls.ModernDialog.DialogType.Warning,
                        "خروج",
                        "انصراف",
                        this);

                    if (!result)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                CloseModal();
            }

            var confirmExit = Controls.ModernDialog.ShowConfirm(
                "تأیید خروج",
                "آیا از خروج از برنامه مطمئن هستید؟",
                Controls.ModernDialog.DialogType.Warning,
                "خروج",
                "انصراف",
                this);

            if (!confirmExit)
                e.Cancel = true;
        }

        private void LoadCompanies()
        {
            try
            {
                var companies = _companyApplication.GetCompanies();
                CompanySelector.ItemsSource = companies;

                if (companies != null && companies.Count > 0)
                {
                    CompanySelector.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to load companies: {ex.Message}");
            }
        }

        private void CompanySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CompanySelector.SelectedItem is CompanyViewModel selectedCompany)
            {
                System.Diagnostics.Debug.WriteLine($"[INFO] Company selected: {selectedCompany.Title} (ID: {selectedCompany.Id})");
            }
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

                var editView = new EditPersonView(personId);
                System.Diagnostics.Debug.WriteLine("[DEBUG] EditPersonView created successfully");
                ModalContent.Content = editView;
                System.Diagnostics.Debug.WriteLine("[DEBUG] ModalContent.Content set");
                ModalOverlay.Visibility = Visibility.Visible;
                System.Diagnostics.Debug.WriteLine("[DEBUG] ModalOverlay set to Visible");
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
            // کلیک روی پس‌زمینه تیره، فرم ویرایش را نمی‌بندد؛
            // بستن فقط از طریق دکمه‌های داخل خود فرم (ذخیره/انصراف) انجام می‌شود.
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            bool confirmed = Controls.ModernDialog.ShowConfirm(
                "تأیید خروج",
                "آیا از خروج از برنامه مطمئن هستید؟",
                Controls.ModernDialog.DialogType.Warning,
                "خروج",
                "انصراف",
                this);

            if (confirmed)
            {
                Application.Current.Shutdown();
            }
        }
    }
}