using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Application.Contract.City;
using GeneralInfoManagement.Application.Contract.Company;
using GeneralInfoManagement.Application.Contract.Province;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Taadol.Controls;
using Taadol.Helpers;

namespace Taadol.Views
{
    public partial class NewBranchView : UserControl, INotifyPropertyChanged
    {
        private CancellationTokenSource _loadCts = new();
        private readonly IBranchApplication _branchApplication;
        private readonly ICompanyApplication _companyApplication;
        private bool _isSaving;
        private bool _userMadeChanges;
        private bool _isLoading = true;
        private bool _isCodeAutomatic = true;
        private bool _isLoadedOnce = false;
        public ObservableCollection<CityComboItem> Cities { get; set; } = new();

        private long _selectedProvinceId;

        private long _selectedCityId;

        public long SelectedProvinceId
        {
            get => _selectedProvinceId;
            set
            {
                _selectedProvinceId = value;
                OnPropertyChanged(nameof(SelectedProvinceId));
                MarkUserChange();
                SelectedCityId = 0;
                UpdateCityState();
                if (_selectedProvinceId > 0)
                    _ = LoadCitiesFromSubSystemAsync(_selectedProvinceId);
                else
                    Cities.Clear();
            }
        }

        private async Task LoadCitiesFromSubSystemAsync(long provinceId)
        {
            CityComboBox.IsEnabled = false;
            CityLoadingOverlay.Visibility = Visibility.Visible;
            Cities.Clear();

            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);

            var loadToken = _loadCts?.Token ?? CancellationToken.None;
            try
            {
                var cities = await Task.Run(() =>
                {
                    loadToken.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();
                    var cityApplication = scope.ServiceProvider.GetRequiredService<ICityApplication>();
                    return cityApplication.GetCitiesByProvinceId(provinceId);
                }, loadToken);

                if (SelectedProvinceId != provinceId)
                    return;

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (SelectedProvinceId != provinceId || loadToken.IsCancellationRequested)
                        return;
                    Cities.Clear();
                    foreach (var c in cities)
                        Cities.Add(new CityComboItem { Id = c.Id, Title = c.Title });

                    if (Cities.All(c => c.Id != SelectedCityId))
                        SelectedCityId = 0;
                });
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[NewBranchView] LoadCitiesFromSubSystemAsync was cancelled");
            }
            finally
            {
                if (!loadToken.IsCancellationRequested && SelectedProvinceId == provinceId)
                {
                    CityComboBox.IsEnabled = true;
                    CityLoadingOverlay.Visibility = Visibility.Collapsed;
                }
            }
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
        private void ProvinceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProvinceComboBox.SelectedValue is long id)
                SelectedProvinceId = id;
        }

        public ObservableCollection<ProvinceComboItem> Provinces { get; set; } = new();

        public long SelectedCityId
        {
            get => _selectedCityId;
            set
            {
                _selectedCityId = value;
                OnPropertyChanged(nameof(SelectedCityId));
                MarkUserChange();
            }
        }

        private void UpdateCityState()
        {
            if (CityBlockOverlay == null) return;

            bool blocked = SelectedProvinceId <= 0;
            CityBlockOverlay.Visibility = blocked ? Visibility.Visible : Visibility.Collapsed;

            if (!blocked && CityErrorText != null)
                CityErrorText.Visibility = Visibility.Collapsed;
        }

        private void CityBlockOverlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (CityErrorText != null)
                CityErrorText.Visibility = Visibility.Visible;
            e.Handled = true;
        }

        private async Task LoadProvincesAsync()
        {
            ProvinceComboBox.IsEnabled = false;

            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);

            var loadToken = _loadCts?.Token ?? CancellationToken.None;
            try
            {
                await Task.Run(async () =>
                {
                    loadToken.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();
                    var provinceApplication = scope.ServiceProvider.GetRequiredService<IProvinceApplication>();

                    var provincesFromBackend = provinceApplication.GetProvinces();

                    var mappedProvinces = provincesFromBackend.Select(p => new ProvinceComboItem
                    {
                        Id = p.Id,
                        Title = p.Title
                    }).ToList();

                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        if (loadToken.IsCancellationRequested)
                            return;
                        Provinces.Clear();
                        foreach (var p in mappedProvinces)
                            Provinces.Add(p);
                    });
                }, loadToken);
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[NewBranchView] LoadProvincesAsync was cancelled");
            }
            catch (Exception ex)
            {
                if (_loadCts?.IsCancellationRequested != true)
                {
                    System.Diagnostics.Debug.WriteLine($"[NewBranchView] Load provinces error: {ex}");
                    ToastManager.Error("خطا در لود استان‌ها");
                }
            }
            finally
            {
                if (_loadCts?.IsCancellationRequested != true)
                    ProvinceComboBox.IsEnabled = true;
            }
        }
        private async Task LoadCitiesAsync(long provinceId)
        {
            CityComboBox.IsEnabled = false;
            Cities.Clear();

            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);

            var loadToken = _loadCts?.Token ?? CancellationToken.None;
            try
            {
                var citiesFromBackend = await Task.Run(() =>
                {
                    loadToken.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();
                    var cityApplication = scope.ServiceProvider.GetRequiredService<ICityApplication>();
                    return cityApplication.GetCitiesByProvinceId(provinceId);
                }, loadToken);

                if (SelectedProvinceId != provinceId)
                    return;

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    if (SelectedProvinceId != provinceId || loadToken.IsCancellationRequested)
                        return;
                    Cities.Clear();
                    foreach (var c in citiesFromBackend)
                        Cities.Add(new CityComboItem { Id = c.Id, Title = c.Title });

                    if (Cities.All(c => c.Id != SelectedCityId))
                        SelectedCityId = 0;
                });
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[NewBranchView] LoadCitiesAsync was cancelled");
            }
            finally
            {
                if (_loadCts?.IsCancellationRequested != true)
                    CityComboBox.IsEnabled = true;
            }
        }
        private void UniqueCodeMode_SelectionChanged(object sender, bool isAutomatic)
        {
            _isCodeAutomatic = isAutomatic;

            if (isAutomatic)
            {
                IsUniqueCodeManual = false;
                UniqueCode = GenerateNextUniqueCodeFromDatabase();
            }
            else
            {
                IsUniqueCodeManual = true;
                UniqueCode = "";
            }
        }
        public ObservableCollection<CompanyViewModel> Companies { get; set; } = new();
        private string _postCode;
        private string _address;
        private string _latitudeText;
        private string _longitudeText;
        private long _selectedCompanyId;
        private string _branchName;
        private string _economicCode;
        private string _registerNumber;
        private string _telePhone;
        private string _mobilePhone;
        private string _email;
        private bool _isActive = true;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged(nameof(IsActive));
                MarkUserChange();
            }
        }
        private static void CancelAndDispose(ref CancellationTokenSource cts)
        {
            var current = Interlocked.Exchange(ref cts, null);
            if (current == null) return;
            try { current.Cancel(); } catch (ObjectDisposedException) { }
            current.Dispose();
        }

        private async void NewBranchView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoadedOnce) return;
            _isLoadedOnce = true;

            try
            {
                await LoadInitialDataAsync();
                await LoadProvincesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewBranchView] Load branch info error: {ex}");
                ToastManager.Error("خطا در بارگذاری اطلاعات شعبه");
            }

            _isLoading = false;
        }

        private async Task LoadInitialDataAsync()
        {
            ShowCompanyComboLoading(true);
            var loadToken = _loadCts?.Token ?? CancellationToken.None;

            await Dispatcher.Yield(DispatcherPriority.Background);

            try
            {
                var companiesTask = Task.Run(() =>
                {
                    loadToken.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();
                    var companyApplication = scope.ServiceProvider.GetRequiredService<ICompanyApplication>();
                    var companies = companyApplication.GetCompanies();
                    loadToken.ThrowIfCancellationRequested();
                    return companies;
                }, loadToken);

                var codeTask = Task.Run(() =>
                {
                    loadToken.ThrowIfCancellationRequested();
                    return GenerateNextUniqueCodeFromDatabase();
                }, loadToken);

                var companies = await companiesTask;
                var nextCode = await codeTask;
                loadToken.ThrowIfCancellationRequested();

                Companies.Clear();
                foreach (var c in companies)
                    Companies.Add(c);

                if (Companies.Count > 0)
                    SelectedCompanyId = Companies[0].Id;

                UniqueCode = nextCode;
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[NewBranchView] LoadInitialDataAsync was cancelled");
            }
            catch (Exception ex)
            {
                if (_loadCts?.IsCancellationRequested != true)
                {
                    System.Diagnostics.Debug.WriteLine($"[NewBranchView] Load info error: {ex}");
                    ToastManager.Error("خطا در لود اطلاعات");
                }
            }
            finally
            {
                if (loadToken.IsCancellationRequested == false)
                    ShowCompanyComboLoading(false);
            }
        }
        private string GenerateNextUniqueCodeFromDatabase()
        {
            using var scope = App.ServiceProvider.CreateScope();

            var branchApplication = scope.ServiceProvider.GetRequiredService<IBranchApplication>();
            var branches = branchApplication.GetBranches();

            var existingCodes = branches
                .Select(b => NormalizeDigits(GetStringPropertyValue(b, "Code")))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .ToList();

            if (existingCodes.Count == 0)
                return "1";

            var candidates = existingCodes
                .Select(code =>
                {
                    var match = Regex.Match(code, @"^(?<prefix>.*?)(?<number>\d+)$");

                    if (!match.Success)
                        return null;

                    return new
                    {
                        Code = code,
                        Prefix = match.Groups["prefix"].Value,
                        NumberText = match.Groups["number"].Value,
                        Number = long.Parse(match.Groups["number"].Value)
                    };
                })
                .Where(x => x != null)
                .OrderByDescending(x => x.Number)
                .FirstOrDefault();

            if (candidates == null)
                return "1";

            long nextNumber = candidates.Number + 1;
            int digitLength = candidates.NumberText.Length;
            string prefix = candidates.Prefix;

            string nextCode;

            do
            {
                nextCode = prefix + nextNumber.ToString("D" + digitLength);
                nextNumber++;
            }
            while (existingCodes.Contains(nextCode));

            return nextCode;
        }

        private string GetStringPropertyValue(object obj, string propertyName)
        {
            if (obj == null) return "";

            var prop = obj.GetType().GetProperty(propertyName);

            return prop?.GetValue(obj)?.ToString() ?? "";
        }

        private string NormalizeDigits(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "";

            return input
                .Replace("۰", "0")
                .Replace("۱", "1")
                .Replace("۲", "2")
                .Replace("۳", "3")
                .Replace("۴", "4")
                .Replace("۵", "5")
                .Replace("۶", "6")
                .Replace("۷", "7")
                .Replace("۸", "8")
                .Replace("۹", "9");
        }

        private string _uniqueCode;
        private bool _isUniqueCodeManual;

        public string UniqueCode
        {
            get => _uniqueCode;
            set
            {
                _uniqueCode = value;
                OnPropertyChanged(nameof(UniqueCode));
                MarkUserChange();
            }
        }

        public bool IsUniqueCodeManual
        {
            get => _isUniqueCodeManual;
            set
            {
                _isUniqueCodeManual = value;
                OnPropertyChanged(nameof(IsUniqueCodeManual));
                MarkUserChange();
            }
        }
        public ICommand SaveCommand { get; private set; }

        public string PostCode
        {
            get => _postCode;
            set
            {
                _postCode = value;
                OnPropertyChanged(nameof(PostCode));
                MarkUserChange();
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
                MarkUserChange();
            }
        }

        public string LatitudeText
        {
            get => _latitudeText;
            set
            {
                _latitudeText = value;
                OnPropertyChanged(nameof(LatitudeText));
                MarkUserChange();
            }
        }
        private double ToDoubleOrZero(string value)
        {
            value = NormalizeDigits(value);

            if (double.TryParse(value, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var result))
                return result;

            return 0;
        }

        public string LongitudeText
        {
            get => _longitudeText;
            set
            {
                _longitudeText = value;
                OnPropertyChanged(nameof(LongitudeText));
                MarkUserChange();
            }
        }
        public long SelectedCompanyId
        {
            get => _selectedCompanyId;
            set
            {
                _selectedCompanyId = value;
                OnPropertyChanged(nameof(SelectedCompanyId));
                MarkUserChange();
            }
        }

        public string BranchName
        {
            get => _branchName;
            set
            {
                _branchName = value;
                OnPropertyChanged(nameof(BranchName));
                MarkUserChange();
            }
        }

        public string EconomicCode
        {
            get => _economicCode;
            set
            {
                _economicCode = value;
                OnPropertyChanged(nameof(EconomicCode));
                MarkUserChange();
            }
        }

        public string RegisterNumber
        {
            get => _registerNumber;
            set
            {
                _registerNumber = value;
                OnPropertyChanged(nameof(RegisterNumber));
                MarkUserChange();
            }
        }

        public string TelePhone
        {
            get => _telePhone;
            set
            {
                _telePhone = value;
                OnPropertyChanged(nameof(TelePhone));
                MarkUserChange();
            }
        }

        public string MobilePhone
        {
            get => _mobilePhone;
            set
            {
                _mobilePhone = value;
                OnPropertyChanged(nameof(MobilePhone));
                MarkUserChange();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
                MarkUserChange();
            }
        }
        public class RelayCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;

            public RelayCommand(Action execute, Func<bool> canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

            public void Execute(object parameter) => _execute();

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }
        }
        public NewBranchView()
        {
            InitializeComponent();

            _branchApplication = App.ServiceProvider.GetRequiredService<IBranchApplication>();

            DataContext = this;
            SaveCommand = new RelayCommand(async () => await SaveBranchAsync());

            UpdateCityState();

            Loaded += NewBranchView_Loaded;
            this.Unloaded += OnViewUnloaded;
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e)
        {
            CancelAndDispose(ref _loadCts);
            this.Unloaded -= OnViewUnloaded;
        }

        private void ShowCompanyComboLoading(bool show)
        {
            CompanyComboLoading.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            CompanyComboBox.IsEnabled = !show;
        }

        private string _nationalId;
        public string NationalId
        {
            get => _nationalId;
            set
            {
                _nationalId = value;
                OnPropertyChanged(nameof(NationalId));
                MarkUserChange();
            }
        }

        private async Task SaveBranchAsync()
        {
            if (_isSaving) return;

            if (!string.IsNullOrEmpty(Email) && !ValidationHelper.IsValidEmail(Email))
            {
                ToastManager.Warning("ایمیل وارد شده معتبر نیست.");
                return;
            }
            double parsedLat = double.TryParse(LatitudeText, out var lat) ? lat : double.NaN;
            double parsedLng = double.TryParse(LongitudeText, out var lng) ? lng : double.NaN;
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

            if (SelectedProvinceId <= 0 || SelectedCityId <= 0)
            {
                ToastManager.Warning("لطفاً استان و شهر را به درستی انتخاب کنید.");
                return;
            }

            _isSaving = true;
            if (SaveButton != null)
            {
                SaveButton.IsEnabled = false;
                SaveButton.ButtonText = "در حال ذخیره...";
            }

            try
            {
                if (_isCodeAutomatic)
                {
                    UniqueCode = await Task.Run(() => GenerateNextUniqueCodeFromDatabase());
                }

                var command = new CreateBranches
                {
                    Title = BranchName.Trim(),
                    Code = UniqueCode ?? "",
                    ManualCode = _isCodeAutomatic ? "" : UniqueCode ?? "",
                    IsCodeAutomatic = _isCodeAutomatic,

                    NationalId = NationalId?.Trim() ?? "",
                    EconomicCode = EconomicCode?.Trim() ?? "",
                    RegisterNumber = RegisterNumber?.Trim() ?? "",

                    Email = Email?.Trim() ?? "",
                    MobilePhone = MobilePhone?.Trim() ?? "",
                    TelePhone = TelePhone?.Trim() ?? "",

                    Address = Address?.Trim() ?? "",
                    PostCode = PostCode?.Trim() ?? "",

                    Latitude = double.TryParse(LatitudeText, out var latitudeValue) ? latitudeValue : 0,
                    Longitude = double.TryParse(LongitudeText, out var longitudeValue) ? longitudeValue : 0,
                    CompanyId = SelectedCompanyId,

                    CityId = SelectedCityId,
                    ProvinceId = SelectedProvinceId,
                    IsMain = IsMainBranch
                };

                var operation = await Task.Run(() => _branchApplication.Create(command));

                var message = GetOperationMessage(operation);

                if (!IsOperationSucceeded(operation))
                {
                    ToastManager.Warning(
                        string.IsNullOrWhiteSpace(message) ? "ثبت شعبه انجام نشد." : message);

                    return;
                }

                if (IsActive)
                {
                    var savedBranch = await Task.Run(() => _branchApplication
                        .GetBranches()
                        .Where(x =>
                            x.NationalId == NationalId.Trim() &&
                            x.CompanyId == SelectedCompanyId)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault());

                    if (savedBranch == null)
                    {
                        ToastManager.Warning(
                            "شعبه ثبت شد، ولی برای فعال‌سازی پیدا نشد.");

                        return;
                    }

                    var activateResult = await Task.Run(() => _branchApplication.Activate(savedBranch.Id));

                    if (!IsOperationSucceeded(activateResult))
                    {
                        ToastManager.Warning(
                            GetOperationMessage(activateResult));

                        return;
                    }
                }
                ToastManager.Success("شعبه با موفقیت ثبت شد.");

                ClearForm();

                MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
            catch (Exception ex)
            {
                var realError = ex.GetBaseException().Message;

                if (realError.Contains("FK_Branches_Cities_CityId") || realError.Contains("Cities") && realError.Contains("FK"))
                {
                    ToastManager.Error("شهر انتخاب‌شده در سیستم معتبر نیست. لطفاً مجدداً شهر را انتخاب کنید.");
                }
                else
                {
                    ToastManager.Error(realError);
                }
            }
            finally
            {
                _isSaving = false;
                if (SaveButton != null)
                {
                    SaveButton.IsEnabled = true;
                    SaveButton.ButtonText = "ثبت شعبه";
                }
            }
        }

        private void ClearForm()
        {
            BranchName = "";
            EconomicCode = "";
            NationalId = "";
            RegisterNumber = "";
            TelePhone = "";
            MobilePhone = "";
            Email = "";
            PostCode = "";
            Address = "";
            LatitudeText = "";
            LongitudeText = "";
            SelectedProvinceId = 0;
            SelectedCityId = 0;
        }

        private void NavigateToBranchList()
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.NavigateTo(NavKeys.BranchList);
        }

        private void MarkUserChange()
        {
            if (_isLoading) return;
            _userMadeChanges = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (_userMadeChanges)
            {
                var result = MessageBox.Show(
                    "تغییراتی که ایجاد کرده‌اید ذخیره نشده است.\nآیا مایل به خروج هستید؟",
                    "خروج",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            NavigateToBranchList();
        }

        private void HeaderClose_Click(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow == null) return;
            if (mainWindow.ModalContent.Content == this)
                mainWindow.CloseModal();
            else
                mainWindow.CloseCurrentForm();
        }

        private bool IsOperationSucceeded(object operation)
        {
            if (operation == null) return false;

            var type = operation.GetType();

            foreach (var name in new[] { "IsSucceeded", "Succeeded", "Success", "IsSuccess" })
            {
                var prop = type.GetProperty(name);

                if (prop != null && prop.PropertyType == typeof(bool))
                    return (bool)prop.GetValue(operation);
            }

            return true;
        }
        private void BranchTypeToggle_SelectionChanged(object sender, bool isFirstSelected)
        {
            IsMainBranch = isFirstSelected;
        }
        private bool _isMainBranch = true;
        public bool IsMainBranch
        {
            get => _isMainBranch;
            set
            {
                _isMainBranch = value;
                OnPropertyChanged(nameof(IsMainBranch));
                MarkUserChange();
            }
        }
        private string GetOperationMessage(object operation)
        {
            if (operation == null) return "";

            var prop = operation.GetType().GetProperty("Message");

            return prop?.GetValue(operation)?.ToString() ?? "";
        }

        private void UniqueIdToggle_SelectionChanged(object sender, bool isFirstSelected)
        {
            _isCodeAutomatic = isFirstSelected;
        }

        private void MyDatePicker_DateChanged(object sender, RoutedEventArgs e)
        {
            var picker = sender as PersianDatePickerControl;
            if (picker == null || !picker.SelectedDate.HasValue) return;

            DateTime selected = picker.SelectedDate.Value;
            Console.WriteLine(selected.ToString("yyyy/MM/dd"));
        }

        private void Radio_Checked(object sender, RoutedEventArgs e)
        {
            var radio = sender as ToggleRadioControl;
            if (radio == null) return;
        }

        private void OnImageSelected(object sender, RoutedEventArgs e)
        {
        }

        private void OnImageRemoved(object sender, RoutedEventArgs e)
        {
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {
        }

        private void TextBox_TextChanged_3(object sender, TextChangedEventArgs e)
        {
        }

        private void TabInventory_Checked(object sender, RoutedEventArgs e)
        {
            if (TabPricing == null ) return;

            TabPricing.IsChecked = false;

            PricingContent.Visibility = Visibility.Collapsed;
            InventoryContent.Visibility = Visibility.Visible;

            InventoryContent.Height = PricingContent.ActualHeight > 0 ? PricingContent.ActualHeight : double.NaN;
        }

        private void TabPricing_Checked(object sender, RoutedEventArgs e)
        {
            if (TabInventory == null ) return;

            TabInventory.IsChecked = false;

            PricingContent.Visibility = Visibility.Visible;
            InventoryContent.Visibility = Visibility.Collapsed;

            PricingContent.Height = InventoryContent.ActualHeight > 0 ? InventoryContent.ActualHeight : double.NaN;
        }

        private void ImagePickerControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void ToggleSwitchControl_SelectionChanged(object sender, bool e)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BranchSaveCommand : ICommand
    {
        private readonly Action _execute;

        public BranchSaveCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged;
    }
}