using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GeneralInfoManagement.Application.Contract.Company;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;
using Taadol.Helpers;

namespace Taadol.Views
{

    public partial class EditCompanyView : UserControl, INotifyPropertyChanged, IUnsavedChangesAware
    {
        private readonly ICompanyApplication _companyApplication;
        private readonly long _companyId;

        private string _companyName = "";
        private string _officialName = "";
        private DateTime? _foundingDate;
        private bool _isActive = true;

        private bool _isLoading = true;
        private bool _userMadeChanges;
        private bool _isSaving;
        private CancellationTokenSource _loadCts = new();

        public ICommand SaveCommand { get; }

        public bool HasUnsavedChanges => _userMadeChanges;

        public string CompanyName
        {
            get => _companyName;
            set { _companyName = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public string OfficialName
        {
            get => _officialName;
            set { _officialName = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public DateTime? FoundingDate
        {
            get => _foundingDate;
            set { _foundingDate = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public EditCompanyView(long companyId)
        {
            InitializeComponent();

            _companyId = companyId;
            _companyApplication = App.ServiceProvider.GetRequiredService<ICompanyApplication>();

            SaveCommand = new RelayCommand(async () => await SaveCompanyAsync());
            DataContext = this;

            Loaded += OnLoaded;
            this.Unloaded += OnViewUnloaded;
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e)
        {
            var current = Interlocked.Exchange(ref _loadCts, null);
            if (current != null)
            {
                try { current.Cancel(); } catch (ObjectDisposedException) { }
                current.Dispose();
            }

            this.Unloaded -= OnViewUnloaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {

                var detailsTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<ICompanyApplication>();
                    return app.GetDetails(_companyId);
                });

                var companiesTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<ICompanyApplication>();
                    return app.GetCompanies();
                });

                var details = await detailsTask;
                if (details == null)
                {
                    ToastManager.Error("شرکت پیدا نشد.");
                    return;
                }

                CompanyName = details.Title ?? "";
                OfficialName = details.LegalName ?? "";

                var companyVm = (await companiesTask).FirstOrDefault(c => c.Id == _companyId);
                IsActive = companyVm?.IsActive ?? true;

                if (details.EstablishedDate != default)
                {

                    FoundingDate = details.EstablishedDate;
                    if (FoundingDatePicker != null)
                        FoundingDatePicker.SelectedDate = details.EstablishedDate;
                }

                if (!string.IsNullOrWhiteSpace(details.Logo))
                {
                    var fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, details.Logo);
                    if (System.IO.File.Exists(fullPath))
                        CompanyImagePicker.ImagePath = fullPath;
                }

                _isLoading = false;
            }
            catch (Exception ex)
            {
                _isLoading = false;
                System.Diagnostics.Debug.WriteLine($"[EditCompanyView] Load info error: {ex}");
                ToastManager.Error("خطا در لود اطلاعات");
            }
        }

        private void MarkUserChange()
        {
            if (_isLoading) return;
            _userMadeChanges = true;
        }

        private void DatePicker_DateChanged(object sender, RoutedEventArgs e)
        {
            var picker = sender as PersianDatePickerControl;
            if (picker == null) return;

            FoundingDate = picker.SelectedDate;
        }

        private void OnImageSelected(object sender, RoutedEventArgs e)
        {
            MarkUserChange();
        }

        private void OnImageRemoved(object sender, RoutedEventArgs e)
        {
            MarkUserChange();
        }

        private async Task SaveCompanyAsync()
        {
            if (_isSaving) return;

            if (string.IsNullOrWhiteSpace(CompanyName))
            {
                ToastManager.Warning("نام شرکت / کسب‌وکار را وارد کنید.");
                return;
            }

            if (string.IsNullOrWhiteSpace(OfficialName))
            {
                ToastManager.Warning("نام رسمی ثبتی را وارد کنید.");
                return;
            }

            if (!FoundingDate.HasValue)
            {
                ToastManager.Warning("تاریخ تاسیس را انتخاب کنید.");
                return;
            }

            if (FoundingDate.Value.Date > DateTime.Today)
            {
                ToastManager.Warning("تاریخ تاسیس نمی‌تواند در آینده باشد.");
                return;
            }

            _isSaving = true;
            if (SaveButton != null)
            {
                SaveButton.IsEnabled = false;
                SaveButton.Text = "در حال ذخیره...";
            }

            try
            {

                var command = new EditCompanies
                {
                    Id = _companyId,
                    Title = CompanyName.Trim(),
                    LegalName = OfficialName.Trim(),
                    EstablishedDate = FoundingDate.Value,
                    Logo = CompanyImagePicker?.ImagePath ?? ""
                };

                var operation = await Task.Run(() => _companyApplication.Edit(command));
                if (!operation.IsSucceeded)
                {
                    ToastManager.Warning(
                        string.IsNullOrWhiteSpace(operation.Message) ? "ویرایش شرکت انجام نشد." : operation.Message);
                    return;
                }

                if (IsActive)
                    await Task.Run(() => _companyApplication.Activate(_companyId));
                else
                    await Task.Run(() => _companyApplication.Deactivate(_companyId));

                ToastManager.Success("ویرایش شرکت با موفقیت انجام شد.");

                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.CloseModal();

                if (mainWindow?.MainContent.Content is CompanyListView listView)
                    _ = RefreshListViewSafeAsync(listView);
                else
                    mainWindow?.NavigateTo(NavKeys.CompanyList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EditCompanyView] Edit error: {ex}");
                ToastManager.Error("خطا در ویرایش");
            }
            finally
            {
                _isSaving = false;
                if (SaveButton != null)
                {
                    SaveButton.IsEnabled = true;
                    SaveButton.Text = "ویرایش";
                }
            }
        }

        private bool ConfirmCloseWithUnsavedWarning()
        {
            if (!_userMadeChanges)
                return true;

            var result = MessageBox.Show(
                "تغییراتی که ایجاد کرده‌اید ذخیره نشده است.\nآیا می‌خواهید آن‌ها را ذخیره کنید؟",
                "ذخیره تغییرات",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _ = SaveCompanyAsync();
                return false;
            }

            return result == MessageBoxResult.No;
        }

        private void Cancel_Click(object sender, MouseButtonEventArgs e)
        {
            if (!ConfirmCloseWithUnsavedWarning())
                return;

            (Window.GetWindow(this) as MainWindow)?.CloseModal();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmCloseWithUnsavedWarning())
                return;

            (Window.GetWindow(this) as MainWindow)?.CloseModal();
        }

        private async Task RefreshListViewSafeAsync(CompanyListView listView)
        {
            try
            {
                await listView.RefreshGridAsync();
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EditCompanyView] Error in RefreshListViewSafeAsync: {ex}");
                ToastManager.Error("خطا در بارگذاری اطلاعات. لطفاً اتصال به سرور را بررسی کنید.");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
