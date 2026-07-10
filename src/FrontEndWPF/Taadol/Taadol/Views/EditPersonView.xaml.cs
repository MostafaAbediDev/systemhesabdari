using _0_Framework.Application;
using BankManagement.Application.Contracts.BankBranch;
using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Application.Contract.City;
using GeneralInfoManagement.Application.Contract.Province;
using GeneralInfoManagement.Domain.General.CityAgg;
using GeneralInfoManagement.Domain.General.ProvinceAgg;
using Microsoft.Extensions.DependencyInjection;
using PersonManagement.Application.Contract.ContactTypes;
using PersonManagement.Application.Contract.PersonAddress;
using PersonManagement.Application.Contract.PersonBank;
using PersonManagement.Application.Contract.PersonCategory;
using PersonManagement.Application.Contract.PersonContact;
using PersonManagement.Application.Contract.Persons;
using PersonManagement.Application.Contract.PersonTypes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Taadol.Controls;

namespace Taadol.Views
{
    public partial class EditPersonView : UserControl, INotifyPropertyChanged
    {
        private readonly IPersonApplication _personApplication;
        private readonly IBranchApplication _branchApplication;
        private readonly IPersonTypeApplication _personTypeApplication;
        private readonly IContactTypeApplication _contactTypeApplication;
        private readonly IPersonContactApplication _personContactApplication;
        private readonly IPersonAddressApplication _personAddressApplication;
        private readonly IPersonBankApplication _personBankApplication;
        private readonly IProvinceRepository _provinceRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IBankBranchApplication? _bankBranchApplication;
        private readonly IPersonCategoryApplication _personCategoryApplication;

        private long _personId;
        private long? _selectedPersonCategoryId;
        private List<ContactTypeViewModel> _contactTypes = new();
        private readonly Dictionary<string, long> _contactTypeByName = new(StringComparer.OrdinalIgnoreCase);

        public ICommand SaveCommand { get; }

        public BulkObservableCollection<BranchComboItem> Branches { get; } = new();
        public BulkObservableCollection<ProvinceViewModel> Provinces { get; } = new();
        public BulkObservableCollection<CityViewModel> Cities { get; } = new();
        public BulkObservableCollection<BankBranchViewModel> BankBranches { get; } = new();

        private string _firstName = "";
        private string _lastName = "";
        private string _contactFirstName = "";
        private string _contactLastName = "";
        private string _nationalCode = "";
        private string _companyName = "";
        private string _economicCode = "";
        private string _registrationNumber = "";
        private string _manualCode = "";
        private bool _isLegal = false;
        private bool _isActive = true;
        private long _selectedBranchId;
        private long _selectedPersonTypeId;
        private string _phone = "";
        private string _mobile = "";
        private string _email = "";
        private string _postalCode = "";
        private string _address = "";
        private long _selectedProvinceId;
        private long _selectedCityId;
        private string _mainBankName = "";
        private string _mainCardNumber = "";
        private string _mainShaba = "";
        private string _mainAccountNumber = "";
        private bool _mainBankIsDefault = true;
        private long _selectedBankBranchId;

        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(); } }
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(); } }
        public string ContactFirstName { get => _contactFirstName; set { _contactFirstName = value; OnPropertyChanged(); } }
        public string ContactLastName { get => _contactLastName; set { _contactLastName = value; OnPropertyChanged(); } }
        public string NationalCode { get => _nationalCode; set { _nationalCode = value; OnPropertyChanged(); } }
        public string CompanyName { get => _companyName; set { _companyName = value; OnPropertyChanged(); } }
        public string EconomicCode { get => _economicCode; set { _economicCode = value; OnPropertyChanged(); } }
        public string RegistrationNumber { get => _registrationNumber; set { _registrationNumber = value; OnPropertyChanged(); } }
        public string ManualCode { get => _manualCode; set { _manualCode = value; OnPropertyChanged(); } }
        public bool IsLegal { get => _isLegal; set { _isLegal = value; OnPropertyChanged(); UpdateLegalTypePanels(); } }
        public bool IsActive { get => _isActive; set { _isActive = value; OnPropertyChanged(); } }
        public long SelectedBranchId { get => _selectedBranchId; set { _selectedBranchId = value; OnPropertyChanged(); } }
        public long SelectedPersonTypeId
        {
            get => _selectedPersonTypeId;
            set { _selectedPersonTypeId = value; OnPropertyChanged(); _ = LoadCategoriesAsync(value); }
        }
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(); } }
        public string Mobile { get => _mobile; set { _mobile = value; OnPropertyChanged(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        public string PostalCode { get => _postalCode; set { _postalCode = value; OnPropertyChanged(); } }
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }
        public long SelectedProvinceId { get => _selectedProvinceId; set { _selectedProvinceId = value; OnPropertyChanged(); _ = LoadCitiesAsync(value); } }
        public long SelectedCityId { get => _selectedCityId; set { _selectedCityId = value; OnPropertyChanged(); } }
        public string MainBankName { get => _mainBankName; set { _mainBankName = value; OnPropertyChanged(); } }
        public string MainCardNumber { get => _mainCardNumber; set { _mainCardNumber = value; OnPropertyChanged(); } }
        public string MainShaba { get => _mainShaba; set { _mainShaba = value; OnPropertyChanged(); } }
        public string MainAccountNumber { get => _mainAccountNumber; set { _mainAccountNumber = value; OnPropertyChanged(); } }
        public bool MainBankIsDefault { get => _mainBankIsDefault; set { _mainBankIsDefault = value; OnPropertyChanged(); } }
        public long SelectedBankBranchId { get => _selectedBankBranchId; set { _selectedBankBranchId = value; OnPropertyChanged(); } }

        public EditPersonView(long personId)
        {
            InitializeComponent();   // ← اضافه کنید

            _personId = personId;

            _personApplication = App.ServiceProvider.GetRequiredService<IPersonApplication>();
            _branchApplication = App.ServiceProvider.GetRequiredService<IBranchApplication>();
            _personTypeApplication = App.ServiceProvider.GetRequiredService<IPersonTypeApplication>();
            _contactTypeApplication = App.ServiceProvider.GetRequiredService<IContactTypeApplication>();
            _personContactApplication = App.ServiceProvider.GetRequiredService<IPersonContactApplication>();
            _personAddressApplication = App.ServiceProvider.GetRequiredService<IPersonAddressApplication>();
            _personBankApplication = App.ServiceProvider.GetRequiredService<IPersonBankApplication>();
            _provinceRepository = App.ServiceProvider.GetRequiredService<IProvinceRepository>();
            _cityRepository = App.ServiceProvider.GetRequiredService<ICityRepository>();
            _bankBranchApplication = App.ServiceProvider.GetService<IBankBranchApplication>();
            _personCategoryApplication = App.ServiceProvider.GetRequiredService<IPersonCategoryApplication>();

            SaveCommand = new RelayCommand(() => SavePerson());
            DataContext = this;

            Loaded += OnLoaded;
            
            // Debug toast
            System.Diagnostics.Debug.WriteLine($"[DEBUG] EditPersonView constructor called for personId={personId}");
            ToastManager.Info($"EditPersonView created for ID: {personId}");
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] EditPersonView.OnLoaded fired");
                if (CategorySearch != null)
                    CategorySearch.CategorySelected += OnCategorySelected;
                System.Diagnostics.Debug.WriteLine("[DEBUG] EditPersonView: starting data load tasks");
                await Task.WhenAll(
                    LoadBranchesAsync(),
                    LoadPersonTypesAsync(),
                    LoadContactTypesAsync(),
                    LoadProvincesAsync(),
                    LoadBankBranchesAsync()
                );
                System.Diagnostics.Debug.WriteLine("[DEBUG] EditPersonView: all load tasks completed, calling LoadPersonData");
                LoadPersonData();
                System.Diagnostics.Debug.WriteLine("[DEBUG] EditPersonView: LoadPersonData completed");
                ToastManager.Success("اطلاعات شخص بارگذاری شد");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] ERROR in EditPersonView.OnLoaded: {ex}");
                ToastManager.Error("خطا در لود: " + ex.Message);
            }
        }

        private void OnCategorySelected(CategorySearchControl.CategoryItem category)
        {
            _selectedPersonCategoryId = category.Id;
        }

        private async Task LoadBranchesAsync()
        {
            try
            {
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IBranchApplication>();
                    return app.GetBranches().Select(b => new BranchComboItem { Id = b.Id, Title = b.Title }).ToList();
                });
                Branches.ReplaceAll(items);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Branches load failed: " + ex.Message);
            }
        }

        private async Task LoadPersonTypesAsync()
        {
            try
            {
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IPersonTypeApplication>();
                    return app.GetPersonTypes();
                });
                if (items.Count > 0 && SelectedPersonTypeId == 0)
                    SelectedPersonTypeId = items[0].Id;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PersonTypes load failed: " + ex.Message);
            }
        }

        private async Task LoadContactTypesAsync()
        {
            try
            {
                _contactTypes = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IContactTypeApplication>();
                    return app.GetActive();
                });
                _contactTypeByName.Clear();
                foreach (var ct in _contactTypes)
                    _contactTypeByName[ct.Title] = ct.Id;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ContactTypes load failed: " + ex.Message);
            }
        }

        private async Task LoadProvincesAsync()
        {
            try
            {
                var items = await Task.Run(() => _provinceRepository.GetProvincesForSelectList());
                Provinces.Clear();
                foreach (var p in items)
                    Provinces.Add(p);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Provinces load failed: " + ex.Message);
            }
        }

        private async Task LoadCitiesAsync(long provinceId)
        {
            if (provinceId <= 0) return;
            try
            {
                var items = await Task.Run(() => _cityRepository.GetCitiesByProvince(provinceId));
                Cities.Clear();
                foreach (var c in items)
                    Cities.Add(c);
                if (Cities.All(c => c.Id != SelectedCityId))
                    SelectedCityId = 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Cities load failed: " + ex.Message);
            }
        }

        private async Task LoadBankBranchesAsync()
        {
            if (_bankBranchApplication == null) return;
            try
            {
                var items = await Task.Run(() => _bankBranchApplication.GetBankBranches());
                BankBranches.Clear();
                foreach (var b in items)
                    BankBranches.Add(b);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BankBranches load failed: " + ex.Message);
            }
        }

        private async Task LoadCategoriesAsync(long personTypeId)
        {
            if (personTypeId <= 0) return;
            if (CategorySearch != null)
                CategorySearch.PersonTypeId = personTypeId;
            try
            {
                var tree = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IPersonCategoryApplication>();
                    return app.GetTree(personTypeId);
                });
                CategorySearch?.LoadFromTreeDto(tree);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Categories load failed: " + ex.Message);
            }
        }

        private void LoadPersonData()
        {
            try
            {
                var details = _personApplication.GetDetails(_personId);
                if (details == null)
                {
                    MessageBox.Show("شخص پیدا نشد.", "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                IsLegal = details.IsLegal;
                FirstName = details.FirstName ?? "";
                LastName = details.LastName ?? "";
                ContactFirstName = details.ContactFirstName ?? "";
                ContactLastName = details.ContactLastName ?? "";
                NationalCode = details.NationalCode ?? "";
                CompanyName = details.IsLegal ? details.FirstName : "";
                EconomicCode = details.EconomicCode ?? "";
                RegistrationNumber = details.RegistrationNumber ?? "";
                ManualCode = details.ManualCode ?? details.CurrentCode ?? "";
                SelectedBranchId = details.BranchId;
                SelectedPersonTypeId = details.PersonTypeId;
                _selectedPersonCategoryId = details.PersonCategoryId;

                var personSearch = _personApplication.Search(new PersonSearchModel { NationalCode = details.NationalCode });
                var personVm = personSearch?.FirstOrDefault(x => x.Id == _personId);
                IsActive = personVm?.IsActive ?? true;

                UpdatePersonTypeToggleSelection();

                var contacts = _personContactApplication.GetByPersonId(_personId) ?? new List<PersonContactViewModel>();
                foreach (var c in contacts)
                {
                    if (c.ContactTypeTitle != null && c.ContactTypeTitle.Contains("موبایل") && !string.IsNullOrWhiteSpace(c.Value))
                        Mobile = c.Value;
                    else if (c.ContactTypeTitle != null && c.ContactTypeTitle.Contains("تلفن") && !string.IsNullOrWhiteSpace(c.Value))
                        Phone = c.Value;
                    else if (c.ContactTypeTitle != null && c.ContactTypeTitle.Contains("ایمیل") && !string.IsNullOrWhiteSpace(c.Value))
                        Email = c.Value;
                }

                var addresses = _personAddressApplication.GetByPersonId(_personId) ?? new List<PersonAddressViewModel>();
                var addr = addresses.FirstOrDefault();
                if (addr != null)
                {
                    Address = addr.Address ?? "";
                    PostalCode = addr.PostalCode ?? "";
                    if (addr.ProvinceId > 0)
                        SelectedProvinceId = addr.ProvinceId;
                    if (addr.CityId > 0)
                        SelectedCityId = addr.CityId;
                }

                try
                {
                    var banks = _personBankApplication.GetByPersonId(_personId) ?? new List<PersonBankViewModel>();
                    var bank = banks.FirstOrDefault();
                    if (bank != null)
                    {
                        MainBankName = bank.BankName ?? "";
                        MainCardNumber = bank.CardNumber ?? "";
                        MainShaba = bank.Shaba ?? "";
                        MainAccountNumber = bank.AccountNumber ?? "";
                        MainBankIsDefault = bank.IsDefault;
                        if (bank.BankBranchId > 0)
                            SelectedBankBranchId = bank.BankBranchId;
                    }
                }
                catch (Exception bankEx)
                {
                    System.Diagnostics.Debug.WriteLine("Bank load failed: " + bankEx.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در لود اطلاعات: " + ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateLegalTypePanels()
        {
            if (FirstNamePanel == null) return;
            if (IsLegal)
            {
                FirstNamePanel.Visibility = Visibility.Collapsed;
                LastNamePanel.Visibility = Visibility.Collapsed;
                NationalCodePanel.Visibility = Visibility.Collapsed;
                CompanyNamePanel.Visibility = Visibility.Visible;
                EconomicCodePanel.Visibility = Visibility.Visible;
                RegistrationNumberPanel.Visibility = Visibility.Visible;
                ContactPersonPanel.Visibility = Visibility.Visible;
            }
            else
            {
                FirstNamePanel.Visibility = Visibility.Visible;
                LastNamePanel.Visibility = Visibility.Visible;
                NationalCodePanel.Visibility = Visibility.Visible;
                CompanyNamePanel.Visibility = Visibility.Collapsed;
                EconomicCodePanel.Visibility = Visibility.Collapsed;
                RegistrationNumberPanel.Visibility = Visibility.Collapsed;
                ContactPersonPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void PersonTypeToggle_Checked(object sender, RoutedEventArgs e)
        {
            var tb = sender as ToggleButton;
            if (tb == null) return;
            if (tb.IsChecked == true && tb.Tag != null && long.TryParse(tb.Tag.ToString(), out var id))
            {
                SelectedPersonTypeId = id;
                UncheckOtherPersonTypeToggles(tb);
            }
        }

        private void UncheckOtherPersonTypeToggles(ToggleButton keepChecked)
        {
            var buttons = new[] { PersonTypeCustomer, PersonTypeSupplier, PersonTypeBoth, PersonTypePersonnel };
            foreach (var b in buttons)
            {
                if (b != null && b != keepChecked && b.IsChecked == true)
                    b.IsChecked = false;
            }
        }

        private void UpdatePersonTypeToggleSelection()
        {
            var buttons = new[] { PersonTypeCustomer, PersonTypeSupplier, PersonTypeBoth, PersonTypePersonnel };
            foreach (var b in buttons)
            {
                if (b != null && b.Tag != null && long.TryParse(b.Tag.ToString(), out var id))
                    b.IsChecked = (id == SelectedPersonTypeId);
            }
        }

        private void CodeModeToggle_SelectionChanged(object sender, bool isFirstSelected) { }

        private void LegalTypeToggle_SelectionChanged(object sender, bool isFirstSelected)
        {
            IsLegal = !isFirstSelected;
        }

        private async void SavePerson()
        {
            var dialog = new CustomConfirmDialog();
            if (dialog.ShowDialog() != true) return;

            if (!ValidatePerson()) return;

            try
            {
                var command = new EditPerson
                {
                    Id = _personId,
                    FirstName = IsLegal ? CompanyName : FirstName,
                    LastName = IsLegal ? "" : LastName,
                    ContactFirstName = IsLegal ? (ContactFirstName ?? "") : "",
                    ContactLastName = IsLegal ? (ContactLastName ?? "") : "",
                    NationalCode = IsLegal ? null : NationalCode,
                    EconomicCode = IsLegal ? EconomicCode : null,
                    RegistrationNumber = IsLegal ? RegistrationNumber : null,
                    IsLegal = IsLegal,
                    PersonTypeId = SelectedPersonTypeId,
                    BranchId = SelectedBranchId,
                    CreditLimit = 0,
                    IsCodeAutomatic = false,
                    ManualCode = ManualCode,
                    PersonCategoryId = _selectedPersonCategoryId
                };

                var result = _personApplication.Edit(command);
                if (!result.IsSucceeded)
                {
                    MessageBox.Show(string.IsNullOrWhiteSpace(result.Message) ? "ویرایش ناموفق بود." : result.Message,
                        "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (IsActive)
                    _personApplication.Activate(_personId);
                else
                    _personApplication.Deactivate(_personId);

                SaveContacts(_personId);
                SaveAddress(_personId);
                SaveBank(_personId);

                MessageBox.Show("ویرایش شخص با موفقیت انجام شد.", "موفقیت", MessageBoxButton.OK, MessageBoxImage.Information);

                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.CloseModal();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در ویرایش: " + ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveContacts(long personId)
        {
            var existing = _personContactApplication.GetByPersonId(personId) ?? new List<PersonContactViewModel>();
            foreach (var c in existing)
                _personContactApplication.Remove(c.Id);

            if (!string.IsNullOrWhiteSpace(Phone) && _contactTypeByName.TryGetValue("تلفن ثابت", out var phoneTypeId))
                _personContactApplication.Create(new CreatePersonContact { PersonId = personId, ContactTypeId = phoneTypeId, Value = Phone.Trim(), Description = "", IsDefault = false });

            if (!string.IsNullOrWhiteSpace(Mobile) && _contactTypeByName.TryGetValue("موبایل", out var mobileTypeId))
                _personContactApplication.Create(new CreatePersonContact { PersonId = personId, ContactTypeId = mobileTypeId, Value = Mobile.Trim(), Description = "", IsDefault = true });

            if (!string.IsNullOrWhiteSpace(Email) && _contactTypeByName.TryGetValue("ایمیل", out var emailTypeId))
                _personContactApplication.Create(new CreatePersonContact { PersonId = personId, ContactTypeId = emailTypeId, Value = Email.Trim(), Description = "", IsDefault = false });
        }

        private void SaveAddress(long personId)
        {
            var existing = _personAddressApplication.GetByPersonId(personId) ?? new List<PersonAddressViewModel>();
            foreach (var a in existing)
                _personAddressApplication.Remove(a.Id);

            if (!string.IsNullOrWhiteSpace(Address) || SelectedProvinceId > 0 || SelectedCityId > 0)
            {
                if (SelectedProvinceId > 0 && SelectedCityId > 0)
                {
                    _personAddressApplication.Create(new CreatePersonAddress
                    {
                        PersonId = personId,
                        Title = "آدرس اصلی",
                        Address = Address ?? "",
                        PostalCode = PostalCode ?? "",
                        ProvinceId = SelectedProvinceId,
                        CityId = SelectedCityId,
                        IsDefault = true
                    });
                }
            }
        }

        private void SaveBank(long personId)
        {
            var existing = _personBankApplication.GetByPersonId(personId) ?? new List<PersonBankViewModel>();
            foreach (var b in existing)
                _personBankApplication.Remove(b.Id);

            if (!string.IsNullOrWhiteSpace(MainShaba) || !string.IsNullOrWhiteSpace(MainCardNumber))
            {
                _personBankApplication.Create(new CreatePersonBank
                {
                    PersonId = personId,
                    BankBranchId = SelectedBankBranchId,
                    CardNumber = MainCardNumber ?? "",
                    Shaba = MainShaba ?? "",
                    AccountNumber = MainAccountNumber ?? "",
                    IsDefault = MainBankIsDefault
                });
            }
        }

        private bool ValidatePerson()
        {
            if (SelectedBranchId <= 0) { MessageBox.Show("لطفاً شعبه را انتخاب کنید.", "انتخاب شعبه", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }
            if (SelectedPersonTypeId <= 0) { MessageBox.Show("لطفاً نوع شخص را انتخاب کنید.", "انتخاب نوع شخص", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }
            if (IsLegal)
            {
                if (string.IsNullOrWhiteSpace(CompanyName)) { MessageBox.Show("نام شرکت را وارد کنید.", "نام شرکت", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }
                if (string.IsNullOrWhiteSpace(EconomicCode)) { MessageBox.Show("کد اقتصادی را وارد کنید.", "کد اقتصادی", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(FirstName)) { MessageBox.Show("نام را وارد کنید.", "نام", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }
                if (string.IsNullOrWhiteSpace(LastName)) { MessageBox.Show("نام خانوادگی را وارد کنید.", "نام خانوادگی", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }
                if (string.IsNullOrWhiteSpace(NationalCode)) { MessageBox.Show("کد ملی را وارد کنید.", "کد ملی", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }
            }
            return true;
        }

private void OnImageSelected(object sender, RoutedEventArgs e) { }
        private void OnImageRemoved(object sender, RoutedEventArgs e) { }
        private void ImagePickerControl_Loaded(object sender, RoutedEventArgs e) { }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.CloseModal();
        }

        private void RootBorder_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void MyDatePicker_DateChanged(object sender, RoutedEventArgs e) { }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public class BulkObservableCollection<T> : ObservableCollection<T>
        {
            public void AddRange(IEnumerable<T> items)
            {
                CheckReentrancy();
                foreach (var item in items)
                    Items.Add(item);
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
            public void ReplaceAll(IEnumerable<T> items)
            {
                Items.Clear();
                foreach (var item in items)
                    Items.Add(item);
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public class BankAccountRow : INotifyPropertyChanged
        {
            private string _bankName = "";
            private string _cardNumber = "";
            private string _shaba = "";
            private string _accountNumber = "";
            private bool _isDefault = false;

            public string BankName { get => _bankName; set { _bankName = value; OnPropertyChanged(); } }
            public string CardNumber { get => _cardNumber; set { _cardNumber = value; OnPropertyChanged(); } }
            public string Shaba { get => _shaba; set { _shaba = value; OnPropertyChanged(); } }
            public string AccountNumber { get => _accountNumber; set { _accountNumber = value; OnPropertyChanged(); } }
            public bool IsDefault { get => _isDefault; set { _isDefault = value; OnPropertyChanged(); } }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string name = null) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
