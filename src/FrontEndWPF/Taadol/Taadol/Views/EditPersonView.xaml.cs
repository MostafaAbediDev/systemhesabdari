using _0_Framework.Application;
using BankManagement.Application.Contracts.BankBranch;
using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Application.Contract.City;
using GeneralInfoManagement.Application.Contract.Picture;
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
using GeneralInfoManagement.Application.Contract.Picture;

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
        private readonly IPictureApplication _pictureApplication;
        private long _personId;
        private long? _selectedPersonCategoryId;
        private List<ContactTypeViewModel> _contactTypes = new();
        private readonly Dictionary<string, long> _contactTypeByName = new(StringComparer.OrdinalIgnoreCase);

        public ICommand SaveCommand { get; }

        public BulkObservableCollection<BranchComboItem> Branches { get; } = new();
        public BulkObservableCollection<ProvinceViewModel> Provinces { get; } = new();
        public BulkObservableCollection<CityViewModel> Cities { get; } = new();
        public BulkObservableCollection<BankBranchViewModel> BankBranches { get; } = new();

        // ★ لیست حساب‌های بانکی اضافه‌شده توسط کاربر (مثل NewPersonView)
        public ObservableCollection<BankAccountRow> BankAccounts { get; } = new();

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
            _pictureApplication = App.ServiceProvider.GetRequiredService<IPictureApplication>();
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
            if (personTypeId <= 0)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ LoadCategoriesAsync: personTypeId <= 0");
                return;
            }

            if (CategorySearch != null)
                CategorySearch.PersonTypeId = personTypeId;

            try
            {
                System.Diagnostics.Debug.WriteLine($"📊 LoadCategoriesAsync: loading for personTypeId={personTypeId}");

                var tree = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IPersonCategoryApplication>();
                    return app.GetTree(personTypeId);
                });

                System.Diagnostics.Debug.WriteLine($"✅ LoadCategoriesAsync: got {tree?.Count ?? 0} root categories");

                if (CategorySearch != null)
                {
                    CategorySearch.LoadFromTreeDto(tree);
                    System.Diagnostics.Debug.WriteLine("✅ LoadCategoriesAsync: LoadFromTreeDto called");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("❌ LoadCategoriesAsync: CategorySearch is null!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("❌ Categories load failed: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("❌ Stack: " + ex.StackTrace);
            }
        }

        private async void LoadPersonData()
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

                // ★ اگه PersonCategoryId داریم، اول اون رو نگه دار
                _selectedPersonCategoryId = details.PersonCategoryId;

                // ★ موقتاً SelectedPersonTypeId رو بدون LoadCategoriesAsync ست کن
                // (جلوگیری از صدا زدن async که کامل نمی‌شه)
                _selectedPersonTypeId = details.PersonTypeId;
                OnPropertyChanged(nameof(SelectedPersonTypeId));

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
                    // ★ لود همه بانک‌های شخص از دیتابیس
                    var banks = _personBankApplication.GetByPersonId(_personId) ?? new List<PersonBankViewModel>();
                    System.Diagnostics.Debug.WriteLine($"🏦 LoadPersonData: loaded {banks.Count} bank(s)");

                    if (banks.Count > 0)
                    {
                        // ★ اولین بانک (یا بانک پیش‌فرض) رو در فیلد اصلی قرار بده
                        var mainBank = banks.FirstOrDefault(b => b.IsDefault) ?? banks[0];

                        MainBankName = mainBank.BankName ?? "";
                        MainCardNumber = mainBank.CardNumber ?? "";
                        MainShaba = mainBank.Shaba ?? "";
                        MainAccountNumber = mainBank.AccountNumber ?? "";
                        MainBankIsDefault = mainBank.IsDefault;
                        if (mainBank.BankBranchId > 0)
                            SelectedBankBranchId = mainBank.BankBranchId;

                        // ★ بقیه بانک‌ها رو در لیست BankAccounts قرار بده
                        BankAccounts.Clear();
                        foreach (var b in banks.Where(b => b.Id != mainBank.Id))
                        {
                            BankAccounts.Add(new BankAccountRow
                            {
                                BankBranchId = b.BankBranchId,
                                BankName = b.BankName ?? "",
                                CardNumber = b.CardNumber ?? "",
                                Shaba = b.Shaba ?? "",
                                AccountNumber = b.AccountNumber ?? "",
                                IsDefault = b.IsDefault
                            });
                        }

                        System.Diagnostics.Debug.WriteLine($"🏦 LoadPersonData: main bank set, {BankAccounts.Count} additional bank(s) in list");
                    }
                }
                catch (Exception bankEx)
                {
                    System.Diagnostics.Debug.WriteLine("Bank load failed: " + bankEx.Message);
                }

                // ★ لود دسته‌بندی‌ها بعد از لود کامل داده‌ها
                // (با await تا مطمئن بشیم کامل لود می‌شه)
                if (SelectedPersonTypeId > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"📊 LoadPersonData: calling LoadCategoriesAsync for personTypeId={SelectedPersonTypeId}");
                    await LoadCategoriesAsync(SelectedPersonTypeId);

                    // ★ اگه PersonCategoryId داریم، در CategorySearch انتخابش کن
                    if (_selectedPersonCategoryId.HasValue && _selectedPersonCategoryId.Value > 0 && CategorySearch != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"📊 LoadPersonData: selecting category {_selectedPersonCategoryId.Value}");
                        CategorySearch.SelectCategoryById(_selectedPersonCategoryId.Value);
                    }
                }
                // ★ لود عکس شخص
                try
                {
                    var pictures = _pictureApplication.GetByOwner(_personId, PictureOwnerTypeDTO.Person);
                    var picture = pictures.FirstOrDefault();
                    if (picture != null && !string.IsNullOrWhiteSpace(picture.Url))
                    {
                        var fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, picture.Url);
                        if (System.IO.File.Exists(fullPath))
                        {
                            PersonImagePicker.ImagePath = fullPath;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Picture load failed: " + ex.Message);
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

            // اگه toggle از حالت checked خارج شد (Unchecked event)، IsChecked == false
            if (tb.IsChecked == true && tb.Tag != null && long.TryParse(tb.Tag.ToString(), out var id))
            {
                SelectedPersonTypeId = id;
                UncheckOtherPersonTypeToggles(tb);
            }

            // ★ نمایش/مخفی کردن بخش پرسنل بر اساس وضعیت PersonTypePersonnel
            UpdatePersonnelSectionVisibility();
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

            // ★ بعد از لود داده‌ها، بخش پرسنل رو هم آپدیت کن
            UpdatePersonnelSectionVisibility();
        }

        /// <summary>
        /// ★ بخش مشخصات پرسونل فقط وقتی نمایش داده می‌شه که Toggle پرسنل فعال باشه.
        /// </summary>
        private void UpdatePersonnelSectionVisibility()
        {
            if (PersonnelSection == null) return;
            PersonnelSection.Visibility =
                (PersonTypePersonnel?.IsChecked == true)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
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
            System.Diagnostics.Debug.WriteLine($"🏦 SaveBank START — personId={personId}, MainShaba='{MainShaba}', BankAccounts.Count={BankAccounts.Count}");

            // 1. حذف بانک‌های قبلی
            try
            {
                var existing = _personBankApplication.GetByPersonId(personId) ?? new List<PersonBankViewModel>();
                System.Diagnostics.Debug.WriteLine($"🏦 Found {existing.Count} existing bank(s) to remove");
                foreach (var b in existing)
                {
                    System.Diagnostics.Debug.WriteLine($"🏦 Removing bank Id={b.Id}, Shaba='{b.Shaba}'");
                    _personBankApplication.Remove(b.Id);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Bank existing load failed: {ex.Message}");
            }

            // 2. ذخیره حساب اصلی (اگه فیلد اصلی داده داره)
            if (!string.IsNullOrWhiteSpace(MainShaba) || !string.IsNullOrWhiteSpace(MainCardNumber))
            {
                System.Diagnostics.Debug.WriteLine($"🏦 Saving MAIN account — Shaba='{MainShaba}', CardNumber='{MainCardNumber}'");
                TryCreateBankAccount(personId, SelectedBankBranchId, MainBankName, MainAccountNumber, MainCardNumber, MainShaba, MainBankIsDefault);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("🏦 Main account is empty, skipping");
            }

            // 3. ذخیره حساب‌های اضافه‌شده
            int idx = 0;
            foreach (var row in BankAccounts)
            {
                idx++;
                if (string.IsNullOrWhiteSpace(row.Shaba) && string.IsNullOrWhiteSpace(row.CardNumber))
                {
                    System.Diagnostics.Debug.WriteLine($"🏦 Row #{idx} skipped — empty");
                    continue;
                }
                System.Diagnostics.Debug.WriteLine($"🏦 Saving Row #{idx} — Shaba='{row.Shaba}', CardNumber='{row.CardNumber}'");
                TryCreateBankAccount(personId, row.BankBranchId, row.BankName, row.AccountNumber, row.CardNumber, row.Shaba, row.IsDefault);
            }

            // 4. پاک کردن BankAccounts بعد از Save
            BankAccounts.Clear();
            System.Diagnostics.Debug.WriteLine("🏦 SaveBank END — BankAccounts cleared");
        }

        // ======================================================
        //  Bank Accounts (Add/Remove) — مثل NewPersonView
        // ======================================================
        private static bool IsBankAccountRowEmpty(BankAccountRow row)
        {
            return row.BankBranchId <= 0
                && string.IsNullOrWhiteSpace(row.BankName)
                && string.IsNullOrWhiteSpace(row.BranchName)
                && string.IsNullOrWhiteSpace(row.CardNumber)
                && string.IsNullOrWhiteSpace(row.Shaba)
                && string.IsNullOrWhiteSpace(row.AccountNumber);
        }

        private bool HasEmptyBankRow()
        {
            return BankAccounts.Any(IsBankAccountRowEmpty);
        }

        private bool MainBankHasData()
        {
            return !string.IsNullOrWhiteSpace(MainCardNumber) || !string.IsNullOrWhiteSpace(MainShaba);
        }

        private void AddBankAccountButton_Click(object sender, RoutedEventArgs e)
        {
            // ★ اگه فیلد اصلی داده داره، اطلاعات رو به لیست اضافه کن و فیلد رو خالی کن
            if (MainBankHasData())
            {
                // فقط در حالت انتقال، چک کن که ردیف خالی وجود نداشته باشه
                if (HasEmptyBankRow())
                {
                    MessageBox.Show("لطفاً ابتدا ردیف‌های قبلی را تکمیل کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                BankAccounts.Insert(0, new BankAccountRow
                {
                    BankBranchId = SelectedBankBranchId,
                    BankName = MainBankName ?? "",
                    CardNumber = MainCardNumber ?? "",
                    Shaba = MainShaba ?? "",
                    AccountNumber = MainAccountNumber ?? "",
                    IsDefault = MainBankIsDefault
                });

                MainBankName = "";
                MainCardNumber = "";
                MainShaba = "";
                MainAccountNumber = "";
                MainBankIsDefault = false;
                SelectedBankBranchId = 0;
            }
            // ★ وگرنه:
            // - اگه لیست خالی هست → کاربر باید اول فیلد اصلی رو پر کنه (ارور)
            // - اگه لیست حساب داره → می‌تونه ردیف جدید بسازه
            else
            {
                if (BankAccounts.Count == 0)
                {
                    MessageBox.Show(
                        "لطفاً ابتدا اطلاعات حساب بانکی را در فیلدهای بالا وارد کنید، سپس دکمه افزودن را بزنید.",
                        "حساب اول",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                // چک کن ردیف خالی وجود نداشته باشه
                if (HasEmptyBankRow())
                {
                    MessageBox.Show("لطفاً ابتدا ردیف‌های قبلی را تکمیل کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                BankAccounts.Insert(0, new BankAccountRow());
            }
        }

        private void RemoveMainBankButton_Click(object sender, RoutedEventArgs e)
        {
            MainBankName = "";
            MainCardNumber = "";
            MainShaba = "";
            MainAccountNumber = "";
            MainBankIsDefault = false;
            SelectedBankBranchId = 0;
        }

        private void AddBankAccountFromRow_Click(object sender, RoutedEventArgs e)
        {
            if (HasEmptyBankRow())
            {
                MessageBox.Show("لطفاً ابتدا ردیف‌های قبلی را تکمیل کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (sender is Button btn && btn.DataContext is BankAccountRow currentRow)
            {
                var currentIndex = BankAccounts.IndexOf(currentRow);
                BankAccounts.Insert(currentIndex, new BankAccountRow());
            }
            else
            {
                BankAccounts.Insert(0, new BankAccountRow());
            }
        }

        private void RemoveBankAccount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BankAccountRow row)
                BankAccounts.Remove(row);
        }

        private bool _isUpdatingDefault;

        private void MainDefaultToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (_isUpdatingDefault || !MainBankIsDefault) return;
            _isUpdatingDefault = true;

            var defaultRows = BankAccounts.Where(r => r.IsDefault).ToList();
            if (defaultRows.Count > 0)
            {
                var result = MessageBox.Show("فقط یک حساب می‌تواند پیش‌فرض باشد. آیا پیش‌فرض قبلی لغو شود؟", "تغییر حساب پیش‌فرض", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    foreach (var row in defaultRows)
                        row.IsDefault = false;
                }
                else
                {
                    MainBankIsDefault = false;
                }
            }

            _isUpdatingDefault = false;
        }

        private void RowDefaultToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (_isUpdatingDefault) return;

            var toggle = sender as ToggleButton;
            if (toggle == null) return;

            var row = toggle.DataContext as BankAccountRow;
            if (row == null || !row.IsDefault) return;

            _isUpdatingDefault = true;

            var otherRows = BankAccounts.Where(r => r.IsDefault && r != row).ToList();
            bool hasMainDefault = MainBankIsDefault;

            if (otherRows.Count > 0 || hasMainDefault)
            {
                var result = MessageBox.Show("فقط یک حساب می‌تواند پیش‌فرض باشد. آیا پیش‌فرض قبلی لغو شود؟", "تغییر حساب پیش‌فرض", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (hasMainDefault)
                        MainBankIsDefault = false;
                    foreach (var other in otherRows)
                        other.IsDefault = false;
                }
                else
                {
                    row.IsDefault = false;
                }
            }

            _isUpdatingDefault = false;
        }

        // ★ متد کمکی برای ساخت حساب بانکی
        private void TryCreateBankAccount(long personId, long bankBranchId, string bankName, string accountNumber, string cardNumber, string shaba, bool isDefault)
        {
            if (bankBranchId <= 0)
            {
                System.Diagnostics.Debug.WriteLine(
                    "⚠️ Bank account not saved: BankBranch is required. " +
                    $"BankBranchId = 0, BankName = {bankName}");
                return;
            }

            try
            {
                var result = _personBankApplication.Create(new CreatePersonBank
                {
                    PersonId = personId,
                    BankBranchId = bankBranchId,
                    AccountNumber = accountNumber ?? "",
                    CardNumber = cardNumber ?? "",
                    Shaba = shaba ?? "",
                    IsDefault = isDefault
                });

                if (!result.IsSucceeded)
                {
                    System.Diagnostics.Debug.WriteLine($"⚠️ Bank account creation failed: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ TryCreateBankAccount exception: {ex.Message}");
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
            // ★ دکمه ضربدر از XAML حذف شد، این متد برای امنیت نگه داشته شده
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.CloseModal();
        }

        // ★ Clip داینامیک برای حفظ گوشه‌های گرد در همه حالت‌ها
        // (حتی هنگام اسکرول و حرکت ScrollViewer)
        private void RootBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var border = sender as Border;
            if (border == null) return;

            var width = border.ActualWidth;
            var height = border.ActualHeight;
            if (width <= 0 || height <= 0) return;

            // RectangleGeometry با RadiusX/RadiusY برای clip واقعی گوشه‌های گرد
            //Rect باید دقیقاً هم‌اندازه RootBorder باشه تا گوشه‌های گرد clip بشن
            // ولی دایره‌های جداکننده (با margin منفی) داخل محدوده هستن و clip نمی‌شن
            border.Clip = new System.Windows.Media.RectangleGeometry(
                new Rect(0, 0, width, height),
                12, 12);
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
            private long _bankBranchId;
            private string _bankName = "";
            private string _branchName = "";
            private string _cardNumber = "";
            private string _shaba = "";
            private string _accountNumber = "";
            private bool _isDefault = false;

            /// <summary>شناسه‌ی شعبه بانک انتخاب‌شده برای این ردیف</summary>
            public long BankBranchId { get => _bankBranchId; set { _bankBranchId = value; OnPropertyChanged(); } }
            public string BankName { get => _bankName; set { _bankName = value; OnPropertyChanged(); } }
            public string BranchName { get => _branchName; set { _branchName = value; OnPropertyChanged(); } }
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
