using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Application.Contract.Company;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;

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
        public ICommand SaveCommand { get; }
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

        public NewBranchView()
        {
            InitializeComponent();

            _branchApplication = App.ServiceProvider.GetRequiredService<IBranchApplication>();
            _companyApplication = App.ServiceProvider.GetRequiredService<ICompanyApplication>();

            SaveCommand = new BranchSaveCommand(SaveBranch);

            IsUniqueCodeManual = false;

            DataContext = this;

            Loaded += NewBranchView_Loaded;
        }

        private async Task LoadCompaniesAsync()
        {
            ShowCompanyComboLoading(true);

            try
            {
                var companies = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var companyApplication = scope.ServiceProvider.GetRequiredService<ICompanyApplication>();
                    return companyApplication.GetCompanies();
                });

                Companies = new ObservableCollection<CompanyViewModel>(companies);
                OnPropertyChanged(nameof(Companies));

                if (Companies.Count > 0)
                    SelectedCompanyId = Companies[0].Id;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطا در لود شرکت‌ها", MessageBoxButton.OK, MessageBoxImage.Error);
                Companies = new ObservableCollection<CompanyViewModel>();
                OnPropertyChanged(nameof(Companies));
            }
            finally
            {
                ShowCompanyComboLoading(false);
            }
        }

        private void ShowCompanyComboLoading(bool show)
        {
            CompanyComboLoading.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            CompanyComboBox.IsEnabled = !show;
        }
        private void LoadCompanies()
        {
            try
            {
                Companies = new ObservableCollection<CompanyViewModel>(_companyApplication.GetCompanies());
                OnPropertyChanged(nameof(Companies));

                if (Companies.Count > 0)
                    SelectedCompanyId = Companies[0].Id;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطا در لود شرکت‌ها", MessageBoxButton.OK, MessageBoxImage.Error);
                Companies = new ObservableCollection<CompanyViewModel>();
            }
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
      
        private void SaveBranch()
        {
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

                Latitude = 0,
                Longitude = 0,

                CompanyId = SelectedCompanyId,

                CityId = 1392,
                ProvinceId = 6,
               

            
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