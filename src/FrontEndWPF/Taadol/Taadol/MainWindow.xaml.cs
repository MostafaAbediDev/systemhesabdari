using GeneralInfoManagement.Application.Contract.Company;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Taadol.Controls;
using Taadol.Helpers;
using Taadol.Services;
using Taadol.Views;
using Taadol.Views.Fund;
using Taadol.Views.Bank;

namespace Taadol
{
    public partial class MainWindow : Window
    {

        private NavigationService _nav;
        private ViewFactory _factory;
        private readonly IModalService _modalService;
        private ICompanyApplication _companyApplication;
        private CancellationTokenSource _loadCts = new();
        private int _loadVersion;

        public MainWindow()
        {
            InitializeComponent();

            _modalService = new ModalService(ModalContent, ModalOverlay);

            ToastManager.Initialize(ToastContainer);

            _factory = new ViewFactory(App.ServiceProvider);
            _companyApplication = App.ServiceProvider.GetRequiredService<ICompanyApplication>();

            ViewFactoryRegistrations.RegisterDefaults(_factory, CreateBankListView, CreateNewBankView);

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
                _ = LoadCompaniesAsync();
            };
            this.Closing += (s, e) =>
            {
                var cts = Interlocked.Exchange(ref _loadCts, null);
                if (cts != null)
                {
                    try { cts.Cancel(); } catch (ObjectDisposedException) { }
                    cts.Dispose();
                }
            };

            this.Closing += MainWindow_Closing;

            MainContentBorder.Visibility = Visibility.Collapsed;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            if (ModalOverlay?.Visibility == Visibility.Visible &&
                ModalContent.Content is IUnsavedChangesAware dirtyModal &&
                dirtyModal.HasUnsavedChanges)
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

        private async System.Threading.Tasks.Task LoadCompaniesAsync()
        {

            var version = Interlocked.Increment(ref _loadVersion);
            var cts = new CancellationTokenSource();
            var previousCts = Interlocked.Exchange(ref _loadCts, cts);
            if (previousCts != null)
            {
                try { previousCts.Cancel(); } catch (ObjectDisposedException) { }
                previousCts.Dispose();
            }
            var token = cts.Token;

            CompanySelector.IsEnabled = false;
            try
            {
                var companies = await System.Threading.Tasks.Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<ICompanyApplication>();
                    return app.GetCompanies();
                }, token);

                token.ThrowIfCancellationRequested();
                if (version != Volatile.Read(ref _loadVersion)) return;

                CompanySelector.ItemsSource = companies;

                if (companies != null && companies.Count > 0)
                {
                    CompanySelector.SelectedIndex = 0;
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {

            }
            catch (Exception ex)
            {
                if (version != Volatile.Read(ref _loadVersion)) return;
                System.Diagnostics.Debug.WriteLine($"[ERROR] Failed to load companies: {ex.Message}");
                ToastManager.Error("خطا در بارگذاری لیست شرکت‌ها");
            }
            finally
            {
                if (version == Volatile.Read(ref _loadVersion) && !token.IsCancellationRequested)
                    CompanySelector.IsEnabled = true;
            }
        }

        private void CompanySelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CompanySelector.SelectedItem is CompanyViewModel selectedCompany)
            {
                System.Diagnostics.Debug.WriteLine($"[INFO] Company selected: {selectedCompany.Title} (ID: {selectedCompany.Id})");
            }
        }

        private string _pendingNavigationTag;
        private bool _navigationQueued;

        private void OnSubMenuClicked(string tag)
        {
            if (_navigationQueued)
            {
                _pendingNavigationTag = tag;
                return;
            }
            _navigationQueued = true;
            try
            {

                if (ModalOverlay?.Visibility == Visibility.Visible &&
                    ModalContent.Content is IUnsavedChangesAware dirtyModal &&
                    dirtyModal.HasUnsavedChanges)
                {
                    var confirmNav = Controls.ModernDialog.ShowConfirm(
                        "تغییرات ذخیره‌نشده",
                        "فرم بازشده تغییرات ذخیره‌نشده دارد. آیا بدون ذخیره از آن خارج می‌شوید؟",
                        Controls.ModernDialog.DialogType.Warning,
                        "خروج از فرم",
                        "بازگشت",
                        this);

                    if (!confirmNav)
                    {

                        Sidebar.DeselectActiveSubMenu();
                        return;
                    }
                }

                if (ModalOverlay?.Visibility == Visibility.Visible)
                    CloseModal();

                if (MainContent.Content is IUnsavedChangesAware dirtyForm && dirtyForm.HasUnsavedChanges)
                {
                    var confirmNav = Controls.ModernDialog.ShowConfirm(
                        "تغییرات ذخیره‌نشده",
                        "فرم فعلی تغییرات ذخیره‌نشده دارد. آیا بدون ذخیره از آن خارج می‌شوید؟",
                        Controls.ModernDialog.DialogType.Warning,
                        "خروج از فرم",
                        "بازگشت",
                        this);

                    if (!confirmNav)
                    {

                        Sidebar.DeselectActiveSubMenu();
                        return;
                    }
                }

                MainContentBorder.Visibility = Visibility.Visible;
                _nav.Navigate(tag);
            }
            finally
            {

                _navigationQueued = false;
            }

            var pending = _pendingNavigationTag;
            _pendingNavigationTag = null;
            if (pending != null)
            {
                OnSubMenuClicked(pending);
            }

            Dispatcher.BeginInvoke(new Action(() => MainContentBorder.InvalidateVisual()),
                System.Windows.Threading.DispatcherPriority.Render);
        }

        private void Sidebar_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void NavigateTo(string tag)
        {
            MainContentBorder.Visibility = Visibility.Visible;
            _nav.Navigate(tag);
        }

        private BankListView CreateBankListView()
        {
            var bankListView = new BankListView();
            bankListView.EditBankRequestedForModal += OnEditBankRequestedForModal;
            bankListView.Unloaded += BankListView_Unloaded;
            return bankListView;
        }

        private void BankListView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is not BankListView bankListView)
                return;

            bankListView.EditBankRequestedForModal -= OnEditBankRequestedForModal;
            bankListView.Unloaded -= BankListView_Unloaded;
        }

        private void OnEditBankRequestedForModal(long bankId)
        {
            try
            {
                var editView = new EditBankView(bankId);
                Action? bankUpdatedHandler = null;
                RoutedEventHandler? unloadedHandler = null;

                bankUpdatedHandler = () =>
                {
                    editView.ViewModel.BankUpdated -= bankUpdatedHandler;
                    editView.Unloaded -= unloadedHandler;

                    if (MainContent.Content is BankListView listView)
                        _ = RefreshBankListSafeAsync(listView);

                    if (ReferenceEquals(_modalService.Current, editView))
                        CloseModal();
                };

                unloadedHandler = (_, _) =>
                {
                    editView.ViewModel.BankUpdated -= bankUpdatedHandler;
                    editView.Unloaded -= unloadedHandler;
                };

                editView.ViewModel.BankUpdated += bankUpdatedHandler;
                editView.Unloaded += unloadedHandler;

                _modalService.Open(editView);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"[MainWindow] خطا در باز کردن فرم ویرایش بانک: {exception}");
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                    ToastManager.Error("خطا در باز کردن فرم ویرایش بانک.")));
            }
        }

        private void OpenEditModal<TView>(long id, Func<long, TView> factory) where TView : UserControl
        {
            try
            {
                var view = factory(id);
                _modalService.Open(view);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OpenEditModal<{typeof(TView).Name}> error: {ex}");
                ToastManager.Error($"خطا در باز کردن فرم ویرایش.");
            }
        }

        public void NavigateToEditPerson(long personId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] NavigateToEditPerson called with personId={personId}");

                var editView = new EditPersonView(personId);
                System.Diagnostics.Debug.WriteLine("[DEBUG] EditPersonView created successfully");
                _modalService.Open(editView);
                System.Diagnostics.Debug.WriteLine("[DEBUG] ModalContent.Content set");
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

        public void NavigateToEditCompany(long companyId)
            => OpenEditModal(companyId, id => new EditCompanyView(id));

        public void NavigateToEditFinancialPeriod(long periodId)
            => OpenEditModal(periodId, id => new EditFinancialPeriodView(id));

        public void NavigateToEditBranch(long branchId)
            => OpenEditModal(branchId, id => new EditBranchView(id));

        public void OpenNewPerson()
        {
            var newView = new NewPersonView();
            newView.PersonSaved += () =>
            {
                if (MainContent.Content is PersonListView listView)
                    _ = listView.RefreshGridAsync();
            };

            _modalService.Open(newView);
        }

        public void OpenNewBank()
        {
            var newView = CreateNewBankView();
            _modalService.Open(newView);
        }

        private NewBankView CreateNewBankView()
        {
            var newView = new NewBankView();
            newView.ViewModel.BankSaved += () =>
            {
                if (MainContent.Content is BankListView listView)
                    _ = RefreshBankListSafeAsync(listView);

                if (ReferenceEquals(_modalService.Current, newView))
                    CloseModal();
                else if (MainContent.Content is not BankListView)
                    NavigateTo(NavKeys.BankList);
            };
            return newView;
        }

        private async System.Threading.Tasks.Task RefreshBankListSafeAsync(BankListView listView)
        {
            try
            {
                await listView.RefreshGridAsync();
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"[MainWindow] خطا در رفرش لیست بانک: {exception}");
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                    ToastManager.Error("خطا در بروزرسانی لیست بانک‌ها")));
            }
        }

        public void CloseCurrentForm()
        {
            MainContentBorder.Visibility = Visibility.Collapsed;
            MainContent.Content = null;
            Sidebar.DeselectActiveSubMenu();
        }

        public void CloseModal() => _modalService.Close();

        private void ModalOverlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            Close();
        }
    }
}