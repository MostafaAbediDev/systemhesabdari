using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Application.Contract.City;
using GeneralInfoManagement.Application.Contract.Company;
using GeneralInfoManagement.Application.Contract.Province;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;
using Taadol.Helpers;

namespace Taadol.Views
{
    /// <summary>
    /// فرم ویرایش شعبه — الگوی EditCompanyView:
    /// لود اطلاعات از GetDetails (+ کد و وضعیت فعال از لیست)، ذخیره با EditBranch،
    /// ردیابی تغییرات ذخیره‌نشده (IUnsavedChangesAware) و رفرش لیست پشت مودال.
    /// بدون تغییر بک‌اند — متد Edit بک‌اند سالم است (چک‌ها Id خود رکورد را مستثنی می‌کنند).
    /// </summary>
    public partial class EditBranchView : UserControl, INotifyPropertyChanged, IUnsavedChangesAware
    {
        private CancellationTokenSource _loadCts = new();
        private readonly IBranchApplication _branchApplication;
        private readonly long _branchId;

        private string _uniqueCode = "";
        private bool _isUniqueCodeManual = true;
        private bool _isCodeAutomatic = false;
        private string _branchName = "";
        private long _selectedCompanyId;
        private bool _isMain = true;
        private bool _isActive = true;

        private string _telePhone = "";
        private string _mobilePhone = "";
        private string _email = "";

        private string _postCode = "";
        private string _address = "";
        private string _nationalId = "";
        private string _economicCode = "";
        private string _registerNumber = "";
        private string _latitudeText = "";
        private string _longitudeText = "";

        private long _selectedProvinceId;
        private long _selectedCityId;
        private long _preferredCityId; // هنگام لود اولیه، شهر واقعی شعبه انتخاب شود نه اولین شهر

        // ردیابی تغییرات واقعی کاربر:
        // تا پایان لود (IsLoading) تغییرات برنامه‌نویسی نادیده گرفته می‌شوند؛
        // بعد از آن هر تغییر = تغییر کاربر → انصراف فقط در این صورت سؤال می‌پرسد.
        private bool _isLoading = true;
        private bool _userMadeChanges;
        private bool _isSaving;

        public ObservableCollection<CompanyViewModel> Companies { get; set; } = new();
        public ObservableCollection<ProvinceComboItem> Provinces { get; set; } = new();
        public ObservableCollection<CityComboItem> Cities { get; set; } = new();

        public ICommand SaveCommand { get; }

        public bool HasUnsavedChanges => _userMadeChanges;

        public string UniqueCode
        {
            get => _uniqueCode;
            set { _uniqueCode = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public bool IsUniqueCodeManual
        {
            get => _isUniqueCodeManual;
            set { _isUniqueCodeManual = value; OnPropertyChanged(); }
        }

        public string BranchName
        {
            get => _branchName;
            set { _branchName = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public long SelectedCompanyId
        {
            get => _selectedCompanyId;
            set { _selectedCompanyId = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public bool IsMain
        {
            get => _isMain;
            set { _isMain = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public string TelePhone
        {
            get => _telePhone;
            set { _telePhone = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public string MobilePhone
        {
            get => _mobilePhone;
            set { _mobilePhone = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public string PostCode
        {
            get => _postCode;
            set { _postCode = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public string NationalId
        {
            get => _nationalId;
            set { _nationalId = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public string EconomicCode
        {
            get => _economicCode;
            set { _economicCode = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public string RegisterNumber
        {
            get => _registerNumber;
            set { _registerNumber = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public string LatitudeText
        {
            get => _latitudeText;
            set { _latitudeText = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public string LongitudeText
        {
            get => _longitudeText;
            set { _longitudeText = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public long SelectedProvinceId
        {
            get => _selectedProvinceId;
            set
            {
                _selectedProvinceId = value;
                OnPropertyChanged(nameof(SelectedProvinceId));
                if (_isLoading) return;
                SelectedCityId = 0;
                if (_selectedProvinceId > 0)
                    _ = LoadCitiesSafeAsync(_selectedProvinceId);
                else
                    Cities.Clear();
            }
        }

        public long SelectedCityId
        {
            get => _selectedCityId;
            set { _selectedCityId = value; OnPropertyChanged(nameof(SelectedCityId)); MarkUserChange(); }
        }

        public EditBranchView(long branchId)
        {
            InitializeComponent();

            _branchId = branchId;
            _branchApplication = App.ServiceProvider.GetRequiredService<IBranchApplication>();

            SaveCommand = new RelayCommand(async () => await SaveBranchAsync());
            DataContext = this;

            Loaded += OnLoaded;
            this.Unloaded += OnViewUnloaded;
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e)
        {
            CancelAndDispose(ref _loadCts);
            this.Unloaded -= OnViewUnloaded;
        }

        private static void CancelAndDispose(ref CancellationTokenSource cts)
        {
            var current = Interlocked.Exchange(ref cts, null);
            if (current == null) return;
            try { current.Cancel(); } catch (ObjectDisposedException) { }
            current.Dispose();
        }

        public class ProvinceComboItem
        {
            public long Id { get; set; }
            public string Title { get; set; }
        }

        public class CityComboItem
        {
            public long Id { get; set; }
            public string Title { get; set; }
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // جزئیات شعبه، وضعیت فعال، شرکت‌ها و استان‌ها به‌صورت موازی لود می‌شوند
                var detailsTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IBranchApplication>();
                    return app.GetDetails(_branchId);
                });

                var branchVmTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IBranchApplication>();
                    return app.GetBranches().FirstOrDefault(b => b.Id == _branchId);
                });

                var companiesTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<ICompanyApplication>();
                    return app.GetCompanies();
                });

                var provincesTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var provinceApplication = scope.ServiceProvider.GetRequiredService<IProvinceApplication>();
                    return provinceApplication.GetProvinces();
                });

                var details = await detailsTask;
                if (details == null)
                {
                    ToastManager.Error("شعبه پیدا نشد.");
                    return;
                }

                var branchVm = await branchVmTask;
                var companies = await companiesTask;
                var provinces = await provincesTask;

                Companies.Clear();
                foreach (var c in companies)
                    Companies.Add(c);

                Provinces.Clear();
                foreach (var p in provinces)
                    Provinces.Add(new ProvinceComboItem { Id = p.Id, Title = p.Title });

                // کد فعلی شعبه — وضعیت خودکار/دستی مستقیماً از DTO دریافتی از GetDetails خوانده می‌شود (بدون حدس زدن)
                var currentCode = details.ManualCode ?? details.CurrentCode ?? "";
                UniqueCode = currentCode;
                _isCodeAutomatic = details.IsCodeAutomatic;
                IsUniqueCodeManual = !_isCodeAutomatic;
                if (CodeModeToggle != null) CodeModeToggle.IsFirstSelected = _isCodeAutomatic;

                BranchName = details.Title ?? "";
                NationalId = details.NationalId ?? "";
                EconomicCode = details.EconomicCode ?? "";
                RegisterNumber = details.RegisterNumber ?? "";
                Email = details.Email ?? "";
                MobilePhone = details.MobilePhone ?? "";
                TelePhone = details.TelePhone ?? "";
                Address = details.Address ?? "";
                PostCode = details.PostCode ?? "";
                LatitudeText = details.Latitude.ToString(CultureInfo.InvariantCulture);
                LongitudeText = details.Longitude.ToString(CultureInfo.InvariantCulture);

                // GetDetails وضعیت فعال را برنمی‌گرداند؛ از لیست شعبه‌ها می‌خوانیم
                IsActive = branchVm?.IsActive ?? true;

                if (details.CompanyId > 0)
                    SelectedCompanyId = details.CompanyId;

                if (details.ProvinceId > 0)
                {
                    _preferredCityId = details.CityId;
                    SelectedProvinceId = details.ProvinceId;
                    await LoadCitiesAsync(details.ProvinceId);
                }

                // لود کامل شد — از این به بعد هر تغییری = تغییر کاربر
                _isLoading = false;
            }
            catch (Exception ex)
            {
                _isLoading = false;
                System.Diagnostics.Debug.WriteLine($"[EditBranchView] Load info error: {ex}");
                ToastManager.Error("خطا در لود اطلاعات");
            }
        }

        private async Task LoadCitiesAsync(long provinceId)
        {
            CityComboBox.IsEnabled = false;

            try
            {
                var token = _loadCts?.Token ?? CancellationToken.None;
                var cities = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();
                    var cityApplication = scope.ServiceProvider.GetRequiredService<ICityApplication>();
                    return cityApplication.GetCitiesByProvinceId(provinceId)
                        .Select(c => new CityComboItem { Id = c.Id, Title = c.Title })
                        .ToList();
                }, token);

                if (SelectedProvinceId != provinceId)
                    return;

                Cities.Clear();
                foreach (var c in cities)
                    Cities.Add(c);

                long target = _preferredCityId > 0
                    ? _preferredCityId
                    : (Cities.All(c => c.Id != SelectedCityId) ? 0 : SelectedCityId);
                _preferredCityId = 0;

                if (target > 0)
                    SelectedCityId = target;
                else if (Cities.All(c => c.Id != SelectedCityId))
                    SelectedCityId = 0;
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[EditBranchView] LoadCitiesAsync was cancelled");
            }
            catch (Exception ex)
            {
                if (_loadCts?.IsCancellationRequested != true)
                {
                    System.Diagnostics.Debug.WriteLine($"[EditBranchView] Load cities error: {ex}");
                    ToastManager.Error("خطا در لود شهرستان‌ها");
                }
            }
            finally
            {
                if (_loadCts?.IsCancellationRequested != true)
                    CityComboBox.IsEnabled = true;
            }
        }

        /// <summary>Safe wrapper for LoadCitiesAsync with error handling at call site.</summary>
        private async Task LoadCitiesSafeAsync(long provinceId)
        {
            try
            {
                await LoadCitiesAsync(provinceId);
            }
            catch (OperationCanceledException)
            {
                // Cancellation is expected when the province or view changes.
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EditBranchView] Error in LoadCitiesSafeAsync: {ex}");
                ToastManager.Error("خطا در بارگذاری اطلاعات. لطفاً اتصال به سرور را بررسی کنید.");
            }
        }

        /// <summary>Safe wrapper for RefreshGridAsync with error handling at call site.</summary>
        private async Task RefreshListViewSafeAsync(BranchListView listView)
        {
            try
            {
                await listView.RefreshGridAsync();
            }
            catch (OperationCanceledException)
            {
                // Cancellation is expected when the view is closed or superseded.
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EditBranchView] Error in RefreshListViewSafeAsync: {ex}");
                ToastManager.Error("خطا در بارگذاری اطلاعات. لطفاً اتصال به سرور را بررسی کنید.");
            }
        }

        private void ProvinceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProvinceComboBox.SelectedValue is long id)
                SelectedProvinceId = id;
        }

        private void CodeModeToggle_SelectionChanged(object sender, bool isFirstSelected)
        {
            // isFirstSelected = true → اتوماتیک (کد جدید تولید می‌شود)، false → دستی (کد فعلی حفظ می‌شود)
            _isCodeAutomatic = isFirstSelected;
            IsUniqueCodeManual = !isFirstSelected;
            MarkUserChange();
        }

        private void BranchTypeToggle_SelectionChanged(object sender, bool isFirstSelected)
        {
            IsMain = isFirstSelected;
        }

        private void MarkUserChange()
        {
            if (_isLoading) return;
            _userMadeChanges = true;
        }

        // Validation methods moved to Taadol.Helpers.ValidationHelper

        private async Task SaveBranchAsync()
        {
            if (_isSaving) return;

            // اعتبارسنجی اول — دکمه فقط وقتی وارد حالت «در حال ذخیره» می‌شود که فرم معتبر باشد
            if (!_isCodeAutomatic && string.IsNullOrWhiteSpace(UniqueCode))
            {
                ToastManager.Warning("شناسه یکتا را وارد کنید.");
                return;
            }

            if (SelectedCompanyId <= 0)
            {
                ToastManager.Warning("لطفاً شرکت را انتخاب کنید.");
                return;
            }

            if (string.IsNullOrWhiteSpace(BranchName))
            {
                ToastManager.Warning("نام شعبه را وارد کنید.");
                return;
            }

            if (string.IsNullOrWhiteSpace(NationalId))
            {
                ToastManager.Warning("شناسه ملی را وارد کنید.");
                return;
            }

            if (!string.IsNullOrEmpty(Email) && !ValidationHelper.IsValidEmail(Email))
            {
                ToastManager.Warning("ایمیل وارد شده معتبر نیست.");
                return;
            }

            if (!ValidationHelper.IsValidPhone(MobilePhone))
            {
                ToastManager.Warning("شماره موبایل وارد شده معتبر نیست.");
                return;
            }

            if (!ValidationHelper.IsValidPhone(TelePhone))
            {
                ToastManager.Warning("شماره تلفن وارد شده معتبر نیست.");
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
                var command = new EditBranch
                {
                    Id = _branchId,
                    Title = BranchName.Trim(),
                    Code = UniqueCode ?? "",
                    ManualCode = UniqueCode ?? "",
                    IsCodeAutomatic = _isCodeAutomatic,
                    NationalId = NationalId?.Trim() ?? "",
                    EconomicCode = EconomicCode?.Trim() ?? "",
                    RegisterNumber = RegisterNumber?.Trim() ?? "",
                    Email = Email?.Trim() ?? "",
                    MobilePhone = MobilePhone?.Trim() ?? "",
                    TelePhone = TelePhone?.Trim() ?? "",
                    Address = Address?.Trim() ?? "",
                    PostCode = PostCode?.Trim() ?? "",
                    Latitude = double.TryParse(LatitudeText, NumberStyles.Any, CultureInfo.InvariantCulture, out var lat) ? lat : 0,
                    Longitude = double.TryParse(LongitudeText, NumberStyles.Any, CultureInfo.InvariantCulture, out var lng) ? lng : 0,
                    CompanyId = SelectedCompanyId,
                    ProvinceId = SelectedProvinceId,
                    CityId = SelectedCityId,
                    IsMain = IsMain
                };

                var operation = await Task.Run(() => _branchApplication.Edit(command));
                if (!operation.IsSucceeded)
                {
                    ToastManager.Warning(
                        string.IsNullOrWhiteSpace(operation.Message) ? "ویرایش شعبه انجام نشد." : operation.Message);
                    return;
                }

                // هماهنگ‌سازی وضعیت فعال/غیرفعال با لیست
                if (IsActive)
                    await Task.Run(() => _branchApplication.Activate(_branchId));
                else
                    await Task.Run(() => _branchApplication.Deactivate(_branchId));

                ToastManager.Success("ویرایش شعبه با موفقیت انجام شد.");

                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.CloseModal();

                // اگر پشت مودال لیست شعبه‌ها بود همان لیست درجا رفرش می‌شود (بدون از دست رفتن State)
                if (mainWindow?.MainContent.Content is BranchListView listView)
                    _ = RefreshListViewSafeAsync(listView);
                else
                    mainWindow?.NavigateTo(NavKeys.BranchList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EditBranchView] Edit error: {ex}");
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

        /// <summary>
        /// اگر تغییرات ذخیره‌نشده وجود داشته باشد، از کاربر می‌پرسد (ذخیره/انصراف/بستن).
        /// خروجی false یعنی بستن ادامه پیدا نکند (کاربر Cancel زده یا انتخاب کرده ذخیره کند).
        /// </summary>
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
                _ = SaveBranchAsync();
                return false; // ذخیره خودش فرم را می‌بندد
            }

            return result == MessageBoxResult.No;
        }

        private void Cancel_Click(object sender, MouseButtonEventArgs e)
        {
            if (!ConfirmCloseWithUnsavedWarning())
                return;

            (Window.GetWindow(this) as MainWindow)?.CloseModal();
        }

        private void CloseButton_Click(object sender, MouseButtonEventArgs e)
        {
            if (!ConfirmCloseWithUnsavedWarning())
                return;

            (Window.GetWindow(this) as MainWindow)?.CloseModal();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
