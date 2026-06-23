using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Application.Contract.City;
using GeneralInfoManagement.Application.Contract.Company;
using GeneralInfoManagement.Application.Contract.Province;
using GeneralInfoManagement.Domain.General.CityAgg;
using GeneralInfoManagement.Domain.General.ProvinceAgg;
using GeneralInfoManagement.Infrastructure.EFCore.Repository;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Taadol.Controls;

namespace Taadol.Views
{
    public partial class NewBranchView : UserControl, INotifyPropertyChanged
    {
        private readonly IBranchApplication _branchApplication;
        private readonly ICompanyApplication _companyApplication;
        private bool _isCodeAutomatic = true;
        private bool _isBranchCodeManual = false;
        private string _branchCode;
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
                if (_selectedProvinceId > 0)
                    LoadCitiesFromSubSystemAsync(_selectedProvinceId);
            }
        }

        private async Task LoadCitiesFromSubSystemAsync(long provinceId)
        {
            CityComboBox.IsEnabled = false;
            CityLoadingOverlay.Visibility = Visibility.Visible;
            Cities.Clear();

            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);

            await Task.Run(() =>
            {
                using var scope = App.ServiceProvider.CreateScope();
                var cityRepo = scope.ServiceProvider.GetRequiredService<ICityRepository>();
                var cities = cityRepo.GetCitiesByProvince(provinceId); // از زیرسیستم واقعی

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Cities = new ObservableCollection<CityComboItem>(
      cities.Select(c => new CityComboItem
      {
          Id = c.Id,
          Title = c.Title
      })
  );
                    OnPropertyChanged(nameof(Cities));

                    if (Cities.Count > 0)
                        SelectedCityId = Cities[0].Id;
                });
            });

            CityComboBox.IsEnabled = true;
            CityLoadingOverlay.Visibility = Visibility.Collapsed;
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
            }
        }
        private async Task LoadProvincesAsync()
        {
            ProvinceComboBox.IsEnabled = false;

            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);

            await Task.Run(() =>
            {
                using var scope = App.ServiceProvider.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IProvinceRepository>();

                var provincesFromBackend = repo.GetProvincesForSelectList();

                var mappedProvinces = provincesFromBackend.Select(p => new ProvinceComboItem
                {
                    Id = p.Id,
                    Title = p.Title
                }).ToList();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Provinces = new ObservableCollection<ProvinceComboItem>(mappedProvinces);
                    OnPropertyChanged(nameof(Provinces));
                    if (Provinces.Count > 0)
                        SelectedProvinceId = Provinces[0].Id;
                });
            });

            ProvinceComboBox.IsEnabled = true;
        }
        private async Task LoadCitiesAsync(long provinceId)
        {
            CityComboBox.IsEnabled = false;
            Cities.Clear();

            await Dispatcher.InvokeAsync(() => { }, DispatcherPriority.Background);

            await Task.Run(() =>
            {
                using var scope = App.ServiceProvider.CreateScope();
                var cityRepo = scope.ServiceProvider.GetRequiredService<ICityRepository>();

                var citiesFromBackend = cityRepo.GetCitiesByProvince(provinceId);

                var mappedCities = citiesFromBackend.Select(c => new CityComboItem
                {
                    Id = c.Id,
                    Title = c.Title
                }).ToList();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Cities = new ObservableCollection<CityComboItem>(mappedCities);
                    OnPropertyChanged(nameof(Cities));
                    if (Cities.Count > 0)
                        SelectedCityId = Cities[0].Id;
                });
            });

            CityComboBox.IsEnabled = true;
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
        public ObservableCollection<CompanyViewModel> Companies { get; set; }
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
            }
        }
        private async void NewBranchView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isLoadedOnce) return;
            _isLoadedOnce = true;

            await LoadInitialDataAsync();
            await LoadProvincesAsync(); // <-- صدا زده شود
        }

        private async Task LoadInitialDataAsync()
        {
            ShowCompanyComboLoading(true);

            // این خط باعث می‌شود اول UI و انیمیشن فرصت نمایش پیدا کند
            await Dispatcher.Yield(DispatcherPriority.Background);

            try
            {
                var companiesTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var companyApplication = scope.ServiceProvider.GetRequiredService<ICompanyApplication>();
                    return companyApplication.GetCompanies();
                });

                var codeTask = Task.Run(() =>
                {
                    return GenerateNextUniqueCodeFromDatabase();
                });

                var companies = await companiesTask;
                var nextCode = await codeTask;

                Companies = new ObservableCollection<CompanyViewModel>(companies);
                OnPropertyChanged(nameof(Companies));

                if (Companies.Count > 0)
                    SelectedCompanyId = Companies[0].Id;

                UniqueCode = nextCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطا در لود اطلاعات", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ShowCompanyComboLoading(false);
            }
        }
        private readonly IProvinceRepository _provinceRepository;
        private readonly ICityRepository _cityRepository;
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

        public string BranchCode
        {
            get => _branchCode;
            set
            {
                _branchCode = value;
                OnPropertyChanged(nameof(BranchCode));
            }
        }

        public bool IsBranchCodeManual
        {
            get => _isBranchCodeManual;
            set
            {
                _isBranchCodeManual = value;
                OnPropertyChanged(nameof(IsBranchCodeManual));
            }
        }
        private string GenerateNextBranchCode()
        {
            try
            {
                var branches = _branchApplication.GetBranches();

                long maxCode = 0;

                foreach (var branch in branches)
                {
                    if (long.TryParse(branch.Code, out var code))
                    {
                        if (code > maxCode)
                            maxCode = code;
                    }
                }

                return (maxCode + 1).ToString();
            }
            catch
            {
                return "1";
            }
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
            }
        }

        public bool IsUniqueCodeManual
        {
            get => _isUniqueCodeManual;
            set
            {
                _isUniqueCodeManual = value;
                OnPropertyChanged(nameof(IsUniqueCodeManual));
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
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public string LatitudeText
        {
            get => _latitudeText;
            set
            {
                _latitudeText = value;
                OnPropertyChanged(nameof(LatitudeText));
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
            }
        }
        public long SelectedCompanyId
        {
            get => _selectedCompanyId;
            set
            {
                _selectedCompanyId = value;
                OnPropertyChanged(nameof(SelectedCompanyId));
            }
        }

        public string BranchName
        {
            get => _branchName;
            set
            {
                _branchName = value;
                OnPropertyChanged(nameof(BranchName));
            }
        }

        public string EconomicCode
        {
            get => _economicCode;
            set
            {
                _economicCode = value;
                OnPropertyChanged(nameof(EconomicCode));
            }
        }

        public string RegisterNumber
        {
            get => _registerNumber;
            set
            {
                _registerNumber = value;
                OnPropertyChanged(nameof(RegisterNumber));
            }
        }



        public string TelePhone
        {
            get => _telePhone;
            set
            {
                _telePhone = value;
                OnPropertyChanged(nameof(TelePhone));
            }
        }

        public string MobilePhone
        {
            get => _mobilePhone;
            set
            {
                _mobilePhone = value;
                OnPropertyChanged(nameof(MobilePhone));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
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
            _provinceRepository = App.ServiceProvider.GetRequiredService<IProvinceRepository>();
            _cityRepository = App.ServiceProvider.GetRequiredService<ICityRepository>();

            DataContext = this;
            SaveCommand = new RelayCommand(SaveBranch);

            Loaded += NewBranchView_Loaded;
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
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
            }
        }
        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // فقط اعداد فارسی یا لاتین
            return phone.All(c => char.IsDigit(c)) && phone.Length >= 8 && phone.Length <= 11;
        }
        private void SaveBranch()
        {
            if (!string.IsNullOrEmpty(Email) && !IsValidEmail(Email))
            {
                MessageBox.Show("ایمیل وارد شده معتبر نیست.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            double parsedLat = double.TryParse(LatitudeText, out var lat) ? lat : double.NaN;
            double parsedLng = double.TryParse(LongitudeText, out var lng) ? lng : double.NaN;
            if (!IsValidPhone(MobilePhone))
            {
                MessageBox.Show("شماره موبایل وارد شده معتبر نیست.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidPhone(TelePhone))
            {
                MessageBox.Show("شماره تلفن وارد شده معتبر نیست.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // بررسی محدوده معتبر مختصات
          
            if (SelectedCompanyId <= 0)
            {
                MessageBox.Show("لطفاً شرکت را انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(BranchName))
            {
                MessageBox.Show("نام شعبه را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (_isCodeAutomatic)
            {
                UniqueCode = GenerateNextUniqueCodeFromDatabase();
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
                IsMain = BranchType == "Main"


            };
            try
            {
                MessageBox.Show(
    $"CompanyId: {command.CompanyId}\n" +
    $"CityId: {command.CityId}\n" +
    $"ProvinceId: {command.ProvinceId}\n" +
    $"Title: {command.Title}",
    "مقادیر ارسالی به ثبت شعبه"
);
                var operation = _branchApplication.Create(command);

                var message = GetOperationMessage(operation);

                if (!IsOperationSucceeded(operation))
                {
                    MessageBox.Show(
                        string.IsNullOrWhiteSpace(message) ? "ثبت شعبه انجام نشد." : message,
                        "خطا",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                if (IsActive)
                {
                    var savedBranch = _branchApplication
                        .GetBranches()
                        .Where(x =>
                            x.NationalId == NationalId.Trim() &&
                            x.CompanyId == SelectedCompanyId)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();

                    if (savedBranch == null)
                    {
                        MessageBox.Show(
                            "شعبه ثبت شد، ولی برای فعال‌سازی پیدا نشد.",
                            "خطا در فعال‌سازی",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        return;
                    }

                    var activateResult = _branchApplication.Activate(savedBranch.Id);

                    if (!IsOperationSucceeded(activateResult))
                    {
                        MessageBox.Show(
                            GetOperationMessage(activateResult),
                            "خطا در فعال‌سازی شعبه",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);

                        return;
                    }
                }
                MessageBox.Show(
                    "شعبه با موفقیت ثبت شد.",
                    "موفق",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                ClearForm();
            }
            catch (Exception ex)
            {
                var realError = ex.GetBaseException().Message;

                MessageBox.Show(
                    realError,
                    "خطای  ثبت شعبه",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void ClearForm()
        {
            BranchName = "";
            EconomicCode = "";
            NationalId = "";
            RegisterNumber = "";
            BranchCode = "";
            TelePhone = "";
            MobilePhone = "";
            Email = "";
            PostCode = "";
            Address = "";
            LatitudeText = "";
            LongitudeText = "";
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
            MessageBox.Show(isFirstSelected.ToString());
        }
        private string _branchType = "Main"; // Main = اصلی، Sub = فرعی
        public string BranchType
        {
            get => _branchType;
            set
            {
                _branchType = value;
                OnPropertyChanged(nameof(BranchType));
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

        private void TabTax_Checked(object sender, RoutedEventArgs e)
        {
            if (TabPricing == null || TabInventory == null) return;

            TabPricing.IsChecked = false;
            TabInventory.IsChecked = false;

            PricingContent.Visibility = Visibility.Collapsed;
            InventoryContent.Visibility = Visibility.Collapsed;
        }

        private void TabInventory_Checked(object sender, RoutedEventArgs e)
        {
            if (TabPricing == null) return;

            TabPricing.IsChecked = false;

            PricingContent.Visibility = Visibility.Collapsed;
            InventoryContent.Visibility = Visibility.Visible;

            InventoryContent.Height = PricingContent.ActualHeight > 0 ? PricingContent.ActualHeight : double.NaN;
        }

        private void TabPricing_Checked(object sender, RoutedEventArgs e)
        {
            if (TabInventory == null) return;

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