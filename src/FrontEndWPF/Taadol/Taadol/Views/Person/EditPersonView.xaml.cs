using _0_Framework.Application;
using BankManagement.Application.Contracts.BankBranch;
using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Application.Contract.City;
using GeneralInfoManagement.Application.Contract.Picture;
using GeneralInfoManagement.Application.Contract.Province;
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
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Taadol.Controls;
using Taadol.Helpers;

namespace Taadol.Views
{
    public partial class EditPersonView : UserControl, INotifyPropertyChanged, IUnsavedChangesAware
    {
        private CancellationTokenSource _loadCts = new();
        private CancellationTokenSource _saveCts = new();
        private CancellationTokenSource _cityLoadCts = new();

        private readonly IPersonApplication _personApplication;
        private readonly IBranchApplication _branchApplication;
        private readonly IPersonTypeApplication _personTypeApplication;
        private readonly IContactTypeApplication _contactTypeApplication;
        private readonly IPersonContactApplication _personContactApplication;
        private readonly IPersonAddressApplication _personAddressApplication;
        private readonly IBankBranchApplication? _bankBranchApplication;
        private readonly IPersonCategoryApplication _personCategoryApplication;
        private readonly IPictureApplication _pictureApplication;
        private long _personId;
        private long? _selectedPersonCategoryId;
        private List<ContactTypeViewModel> _contactTypes = new();
        private readonly Dictionary<string, long> _contactTypeByName = new(StringComparer.OrdinalIgnoreCase);

        public ICommand SaveCommand { get; }

        // ★ IsDirty property — true when user has unsaved changes
        public bool IsDirty => ComputeUnsavedChanges();

        public bool HasUnsavedChanges => IsDirty;

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

        // ★ snapshot اولیه برای تشخیص تغییرات قبل از بستن با «انصراف»
        private List<BankAccountRow> _initialBankAccounts = new();
        private string _initialImagePath = "";
        private bool _initialAddressLoaded;

        // ردیابی تغییرات واقعی کاربر:
        // تا پایان لود (IsLoading) تغییرات برنامه‌نویسی نادیده گرفته می‌شوند؛
        // بعد از آن هر تغییر = تغییر کاربر → انصراف فقط در این صورت سؤال می‌پرسد.
        private bool _isLoading = true;
        private bool _userMadeChanges;

        private (string FirstName, string LastName, string ContactFirstName, string ContactLastName,
                string NationalCode, string CompanyName, string EconomicCode, string RegistrationNumber,
                string ManualCode, bool IsLegal, bool IsActive, long SelectedBranchId, long SelectedPersonTypeId,
                string Phone, string Mobile, string Email, string PostalCode, string Address,
                long SelectedProvinceId, long SelectedCityId) _initialPerson;

        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(); MarkUserChange(); } }
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(); MarkUserChange(); } }
        public string ContactFirstName { get => _contactFirstName; set { _contactFirstName = value; OnPropertyChanged(); MarkUserChange(); } }
        public string ContactLastName { get => _contactLastName; set { _contactLastName = value; OnPropertyChanged(); MarkUserChange(); } }
        public string NationalCode { get => _nationalCode; set { _nationalCode = value; OnPropertyChanged(); MarkUserChange(); } }
        public string CompanyName { get => _companyName; set { _companyName = value; OnPropertyChanged(); MarkUserChange(); } }
        public string EconomicCode { get => _economicCode; set { _economicCode = value; OnPropertyChanged(); MarkUserChange(); } }
        public string RegistrationNumber { get => _registrationNumber; set { _registrationNumber = value; OnPropertyChanged(); MarkUserChange(); } }
        public string ManualCode { get => _manualCode; set { _manualCode = value; OnPropertyChanged(); MarkUserChange(); } }
        private string _creditLimitText = "";
        public string CreditLimitText { get => _creditLimitText; set { _creditLimitText = value; OnPropertyChanged(); MarkUserChange(); } }
        public bool IsLegal { get => _isLegal; set { _isLegal = value; OnPropertyChanged(); MarkUserChange(); UpdateLegalTypePanels(); } }
        public bool IsActive { get => _isActive; set { _isActive = value; OnPropertyChanged(); MarkUserChange(); } }
        public long SelectedBranchId { get => _selectedBranchId; set { _selectedBranchId = value; OnPropertyChanged(); MarkUserChange(); } }
        public long SelectedPersonTypeId
        {
            get => _selectedPersonTypeId;
            set { _selectedPersonTypeId = value; OnPropertyChanged(); MarkUserChange(); _ = LoadCategoriesSafeAsync(value); }
        }
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(); MarkUserChange(); } }
        public string Mobile { get => _mobile; set { _mobile = value; OnPropertyChanged(); MarkUserChange(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); MarkUserChange(); } }
        public string PostalCode { get => _postalCode; set { _postalCode = value; OnPropertyChanged(); MarkUserChange(); } }
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); MarkUserChange(); } }
        public long SelectedProvinceId
        {
            get => _selectedProvinceId;
            set
            {
                _selectedProvinceId = value;
                OnPropertyChanged();
                MarkUserChange();
                _cityLoadCts.Cancel();
                _cityLoadCts.Dispose();
                _cityLoadCts = new CancellationTokenSource();
                _ = LoadCitiesSafeAsync(value, _cityLoadCts.Token);
            }
        }
        public long SelectedCityId { get => _selectedCityId; set { _selectedCityId = value; OnPropertyChanged(); MarkUserChange(); } }
        public string MainBankName { get => _mainBankName; set { _mainBankName = value; OnPropertyChanged(); MarkUserChange(); } }
        public string MainCardNumber { get => _mainCardNumber; set { _mainCardNumber = value; OnPropertyChanged(); MarkUserChange(); } }
        public string MainShaba { get => _mainShaba; set { _mainShaba = value; OnPropertyChanged(); MarkUserChange(); } }
        public string MainAccountNumber { get => _mainAccountNumber; set { _mainAccountNumber = value; OnPropertyChanged(); MarkUserChange(); } }
        public bool MainBankIsDefault { get => _mainBankIsDefault; set { _mainBankIsDefault = value; OnPropertyChanged(); MarkUserChange(); } }
        public long SelectedBankBranchId { get => _selectedBankBranchId; set { _selectedBankBranchId = value; OnPropertyChanged(); MarkUserChange(); } }

        public EditPersonView(long personId)
        {
            InitializeComponent();   // ← اضافه کنید

            // رویدادهای جدول حساب‌های بانکی (کنترل مشترک) — در کد وصل می‌شوند چون delegate سفارشی دارد
            BankAccountsTable.EditRequested += BankAccountsTable_EditRequested;
            BankAccountsTable.RemoveRequested += BankAccountsTable_RemoveRequested;

            _personId = personId;

            _personApplication = App.ServiceProvider.GetRequiredService<IPersonApplication>();
            _branchApplication = App.ServiceProvider.GetRequiredService<IBranchApplication>();
            _personTypeApplication = App.ServiceProvider.GetRequiredService<IPersonTypeApplication>();
            _contactTypeApplication = App.ServiceProvider.GetRequiredService<IContactTypeApplication>();
            _personContactApplication = App.ServiceProvider.GetRequiredService<IPersonContactApplication>();
            _personAddressApplication = App.ServiceProvider.GetRequiredService<IPersonAddressApplication>();
            _bankBranchApplication = App.ServiceProvider.GetService<IBankBranchApplication>();
            _personCategoryApplication = App.ServiceProvider.GetRequiredService<IPersonCategoryApplication>();
            _pictureApplication = App.ServiceProvider.GetRequiredService<IPictureApplication>();
            SaveCommand = new RelayCommand(() => SavePerson());
            DataContext = this;

            Loaded += OnLoaded;
            this.Unloaded += OnViewUnloaded;
            BankAccounts.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (BankAccountRow item in e.NewItems)
                        item.PropertyChanged += BankAccountRow_PropertyChanged;
                }
                if (e.OldItems != null)
                {
                    foreach (BankAccountRow item in e.OldItems)
                        item.PropertyChanged -= BankAccountRow_PropertyChanged;
                }
                OnPropertyChanged(nameof(IsDirty));
                MarkUserChange();
            };

            ShabaInput.Text = "IR";
            DependencyPropertyDescriptor
                .FromProperty(TextBox.TextProperty, typeof(TextBox))
                .AddValueChanged(ShabaInput.PART_TextBox, (s, ev) => ShabaInput_TextChanged());
            DependencyPropertyDescriptor
                .FromProperty(TextBox.TextProperty, typeof(TextBox))
                .AddValueChanged(CardNumberInput.PART_TextBox, (s, ev) => CardNumberInput_TextChanged());
            DependencyPropertyDescriptor
                .FromProperty(TextBox.TextProperty, typeof(TextBox))
                .AddValueChanged(NationalCodeInput.PART_TextBox, (s, ev) => NationalCodeInput_TextChanged());

            // Debug toast
            System.Diagnostics.Debug.WriteLine($"[DEBUG] EditPersonView constructor called for personId={personId}");
        }

        private void BankAccountRow_PropertyChanged(object? sender, PropertyChangedEventArgs? e)
        {
            OnPropertyChanged(nameof(IsDirty));
            MarkUserChange();
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e)
        {
            if (CategorySearch != null)
                CategorySearch.CategorySelected -= OnCategorySelected;
            if (PersonImagePicker != null)
            {
                PersonImagePicker.ImageSelected -= PersonImagePicker_ImageSelected;
                PersonImagePicker.ImageRemoved -= PersonImagePicker_ImageRemoved;
            }

            _loadCts?.Cancel();
            _saveCts?.Cancel();
            _loadCts?.Dispose();
            _saveCts?.Dispose();
            _cityLoadCts.Cancel();
            _cityLoadCts.Dispose();
            _loadCts = new CancellationTokenSource();
            _saveCts = new CancellationTokenSource();
            _cityLoadCts = new CancellationTokenSource();
            this.Unloaded -= OnViewUnloaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_loadCts.IsCancellationRequested) return;
            this.Unloaded -= OnViewUnloaded;
            this.Unloaded += OnViewUnloaded;
            try
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] EditPersonView.OnLoaded fired");
                if (CategorySearch != null)
                {
                    CategorySearch.CategorySelected -= OnCategorySelected;
                    CategorySearch.CategorySelected += OnCategorySelected;
                }

                // Subscribe to image picker changes for IsDirty
                if (PersonImagePicker != null)
                {
                    PersonImagePicker.ImageSelected -= PersonImagePicker_ImageSelected;
                    PersonImagePicker.ImageSelected += PersonImagePicker_ImageSelected;
                    PersonImagePicker.ImageRemoved -= PersonImagePicker_ImageRemoved;
                    PersonImagePicker.ImageRemoved += PersonImagePicker_ImageRemoved;
                }

                System.Diagnostics.Debug.WriteLine("[DEBUG] EditPersonView: starting data load tasks");
                await Task.WhenAll(
                    LoadBranchesAsync(),
                    LoadPersonTypesAsync(),
                    LoadContactTypesAsync(),
                    LoadProvincesAsync(),
                    LoadBankBranchesAsync()
                );
                System.Diagnostics.Debug.WriteLine("[DEBUG] EditPersonView: all load tasks completed, calling LoadPersonData");
                await LoadPersonData();
                System.Diagnostics.Debug.WriteLine("[DEBUG] EditPersonView: LoadPersonData completed");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[EditPersonView] OnLoaded was cancelled");
            }
            catch (Exception ex)
            {
                if (_loadCts?.IsCancellationRequested != true)
                {
                    System.Diagnostics.Debug.WriteLine($"[DEBUG] ERROR in EditPersonView.OnLoaded: {ex}");
                    ToastManager.Error("خطا در بارگذاری اطلاعات شخص");
                }
            }
            finally
            {
                if (_loadCts?.IsCancellationRequested != true)
                    _isLoading = false;
            }
        }

        private void OnCategorySelected(CategorySearchControl.CategoryItem category)
        {
            _selectedPersonCategoryId = category.Id;
        }

        private void PersonImagePicker_ImageSelected(object? sender, RoutedEventArgs e)
        {
            OnPropertyChanged(nameof(IsDirty));
            MarkUserChange();
        }

        private void PersonImagePicker_ImageRemoved(object? sender, RoutedEventArgs e)
        {
            OnPropertyChanged(nameof(IsDirty));
            MarkUserChange();
        }

        private async Task LoadBranchesAsync()
        {
            var items = await PersonFormHelper.LoadBranchesAsync(
                replaceAll: list => Branches.ReplaceAll(list),
                onError: ex => System.Diagnostics.Debug.WriteLine("[EditPersonView] LoadBranchesAsync FAILED: " + ex.Message));
        }

        private async Task LoadPersonTypesAsync()
        {
            // EditPersonView doesn't populate a PersonTypes collection — pass null
            var items = await PersonFormHelper.LoadPersonTypesAsync(
                null,
                ex => System.Diagnostics.Debug.WriteLine("[EditPersonView] LoadPersonTypesAsync FAILED: " + ex.Message));
            if (items.Count > 0 && SelectedPersonTypeId == 0)
                SelectedPersonTypeId = items[0].Id;
        }

        private async Task LoadContactTypesAsync()
        {
            await PersonFormHelper.LoadContactTypesAsync(
                _contactTypes,
                _contactTypeByName,
                ex => System.Diagnostics.Debug.WriteLine("[EditPersonView] LoadContactTypesAsync FAILED: " + ex.Message));
        }

        private async Task LoadProvincesAsync()
        {
            await PersonFormHelper.LoadProvincesAsync(
                Provinces,
                ex => System.Diagnostics.Debug.WriteLine("Provinces load failed: " + ex.Message));
        }

        private async Task LoadCitiesAsync(long provinceId, CancellationToken cancellationToken)
        {
            await PersonFormHelper.LoadCitiesAsync(
                cities: Cities,
                provinceId: provinceId,
                token: cancellationToken,
                getCurrentProvinceId: () => SelectedProvinceId,
                getCurrentCityId: () => SelectedCityId,
                setSelectedCityId: id => SelectedCityId = id,
                formName: "EditPersonView",
                onError: ex => System.Diagnostics.Debug.WriteLine("Cities load failed: " + ex.Message));
        }

        private async Task LoadBankBranchesAsync()
        {
            await PersonFormHelper.LoadBankBranchesAsync(
                bankBranches: BankBranches,
                hasBankBranchApp: _bankBranchApplication != null,
                onError: ex => System.Diagnostics.Debug.WriteLine("[EditPersonView] LoadBankBranchesAsync FAILED: " + ex.Message),
                onSkipped: msg => System.Diagnostics.Debug.WriteLine("[EditPersonView] LoadBankBranchesAsync SKIPPED (null)"));
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

            System.Diagnostics.Debug.WriteLine($"📊 LoadCategoriesAsync: loading for personTypeId={personTypeId}");

            var token = _loadCts?.Token ?? CancellationToken.None;
            var tree = await PersonFormHelper.LoadCategoryTreeAsync(
                personTypeId,
                token,
                onError: ex =>
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Categories load failed: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"❌ Stack: {ex.StackTrace}");
                },
                onCancelled: () => System.Diagnostics.Debug.WriteLine("[EditPersonView] LoadCategoriesAsync was cancelled")
            );

            if (tree == null) return;

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

        /// <summary>Safe wrapper for LoadCategoriesAsync with error handling at call site.</summary>
        private async Task LoadCategoriesSafeAsync(long personTypeId)
        {
            try
            {
                await LoadCategoriesAsync(personTypeId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EditPersonView] Error in LoadCategoriesSafeAsync: {ex}");
            }
        }

        /// <summary>Safe wrapper for LoadCitiesAsync with error handling at call site.</summary>
        private async Task LoadCitiesSafeAsync(long provinceId, CancellationToken cancellationToken)
        {
            try
            {
                await LoadCitiesAsync(provinceId, cancellationToken);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EditPersonView] Error in LoadCitiesSafeAsync: {ex}");
            }
        }

        /// <summary>Safe wrapper for RefreshGridAsync with error handling at call site.</summary>
        private async Task RefreshListViewSafeAsync(PersonListView listView)
        {
            try
            {
                await listView.RefreshGridAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EditPersonView] Error in RefreshListViewSafeAsync: {ex}");
            }
        }

        private async Task LoadPersonData()
        {
            try
            {
                var personId = _personId;

                // 1) Parallel DB loads
                var detailsTask = LoadDetailsFromDbAsync(personId);
                var contactsTask = LoadContactsFromDbAsync(personId);
                var addressesTask = LoadAddressesFromDbAsync(personId);
                var banksTask = LoadBanksFromDbAsync(personId);
                var picturesTask = LoadPicturesFromDbAsync(personId);

                var details = await detailsTask;
                if (details == null) { ToastManager.Error("شخص پیدا نشد."); return; }

                // 2) Populate main fields
                PopulatePersonDetails(details);

                // 3) Populate contacts, address, banks + IsActive
                PopulateContactsFromList(await contactsTask);
                PopulateAddressFromList(await addressesTask);
                PopulateBankAccountsFromList(await banksTask);
                await PopulateIsActiveAsync(details);

                // 4) Load categories
                await LoadCategoriesAndSelectAsync();

                // 5) Load picture
                await LoadAndSetPictureAsync(await picturesTask);

                // 6) Final sync: cities + snapshot
                try { await LoadCitiesAsync(SelectedProvinceId, _cityLoadCts.Token); } catch { }
                await Task.Delay(80);
                System.Diagnostics.Debug.WriteLine($"[EditPersonView] After LoadPersonData: Provinces={Provinces.Count}, Cities={Cities.Count}, SelectedProvinceId={SelectedProvinceId}, SelectedCityId={SelectedCityId}, SelectedBranchId={SelectedBranchId}, FirstName={FirstName}");
                CaptureInitialSnapshot();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EditPersonView] Load info error: {ex}");
                ToastManager.Error("خطا در لود اطلاعات");
            }
        }

        // --- DB loaders ---

        private Task<EditPerson?> LoadDetailsFromDbAsync(long personId) => Task.Run(() =>
        {
            using var scope = App.ServiceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IPersonApplication>().GetDetails(personId);
        });

        private Task<List<PersonContactViewModel>> LoadContactsFromDbAsync(long personId) => Task.Run(() =>
        {
            using var scope = App.ServiceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IPersonContactApplication>().GetByPersonId(personId) ?? new();
        });

        private Task<List<PersonAddressViewModel>> LoadAddressesFromDbAsync(long personId) => Task.Run(() =>
        {
            using var scope = App.ServiceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IPersonAddressApplication>().GetByPersonId(personId) ?? new();
        });

        private Task<List<PersonBankViewModel>> LoadBanksFromDbAsync(long personId) => Task.Run(() =>
        {
            using var scope = App.ServiceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IPersonBankApplication>().GetByPersonId(personId) ?? new();
        });

        private Task<List<PictureViewModel>> LoadPicturesFromDbAsync(long personId) => Task.Run(() =>
        {
            using var scope = App.ServiceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IPictureApplication>().GetByOwner(personId, PictureOwnerTypeDTO.Person);
        });

        // --- UI population ---

        private void PopulatePersonDetails(EditPerson d)
        {
            IsLegal = d.IsLegal;
            FirstName = d.FirstName ?? ""; LastName = d.LastName ?? "";
            ContactFirstName = d.ContactFirstName ?? ""; ContactLastName = d.ContactLastName ?? "";
            NationalCode = d.NationalCode ?? "";
            CompanyName = d.IsLegal ? d.FirstName : "";
            EconomicCode = d.EconomicCode ?? ""; RegistrationNumber = d.RegistrationNumber ?? "";
            var loadedCode = d.ManualCode ?? d.CurrentCode;
            ManualCode = loadedCode ?? "";
            // وضعیت کد (خودکار/دستی) مستقیماً از DTO دریافتی از GetDetails خوانده می‌شود — بدون حدس زدن
            _isCodeAutomatic = d.IsCodeAutomatic;
            if (CodeModeToggle != null)
            {
                CodeModeToggle.IsFirstSelected = _isCodeAutomatic;
                if (ManualCodeTextBox != null) ManualCodeTextBox.IsEnabled = !_isCodeAutomatic;
            }
            CreditLimitText = d.CreditLimit.HasValue ? d.CreditLimit.Value.ToString("0.##", CultureInfo.InvariantCulture) : "";
            SelectedBranchId = d.BranchId; _selectedPersonCategoryId = d.PersonCategoryId;
            _selectedPersonTypeId = d.PersonTypeId; OnPropertyChanged(nameof(SelectedPersonTypeId));
        }

        private async Task PopulateIsActiveAsync(EditPerson d)
        {
            var personSearch = await Task.Run(() =>
            {
                using var scope = App.ServiceProvider.CreateScope();
                return scope.ServiceProvider.GetRequiredService<IPersonApplication>().Search(new PersonSearchModel { NationalCode = d.NationalCode });
            });
            IsActive = personSearch?.FirstOrDefault(x => x.Id == _personId)?.IsActive ?? true;
        }

        private void PopulateContactsFromList(List<PersonContactViewModel> contacts)
        {
            foreach (var c in contacts)
            {
                if (c.ContactTypeTitle?.Contains("موبایل") == true && !string.IsNullOrWhiteSpace(c.Value)) Mobile = c.Value;
                else if (c.ContactTypeTitle?.Contains("تلفن") == true && !string.IsNullOrWhiteSpace(c.Value)) Phone = c.Value;
                else if (c.ContactTypeTitle?.Contains("ایمیل") == true && !string.IsNullOrWhiteSpace(c.Value)) Email = c.Value;
            }
            UpdatePersonTypeToggleSelection();
        }

        private void PopulateAddressFromList(List<PersonAddressViewModel> addresses)
        {
            var addr = addresses.FirstOrDefault();
            if (addr == null) return;
            Address = addr.Address ?? ""; PostalCode = addr.PostalCode ?? "";
            if (addr.ProvinceId > 0) SelectedProvinceId = addr.ProvinceId;
            if (addr.CityId > 0) SelectedCityId = addr.CityId;
        }

        private void PopulateBankAccountsFromList(List<PersonBankViewModel> banks)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"🏦 LoadPersonData: loaded {banks.Count} bank(s)");
                BankAccounts.Clear();
                foreach (var b in banks)
                    BankAccounts.Add(new BankAccountRow { BankBranchId = b.BankBranchId, BankName = b.BankName ?? "", BranchName = b.BankBranchName ?? "", CardNumber = b.CardNumber ?? "", Shaba = b.Shaba ?? "", AccountNumber = b.AccountNumber ?? "", IsDefault = b.IsDefault });
                if (BankAccounts.Count >= 1 && !BankAccounts.Any(r => r.IsDefault)) BankAccounts[0].IsDefault = true;
                ReindexBankAccounts();
                // Reset main bank form fields
                MainBankName = ""; MainCardNumber = ""; MainShaba = "IR"; MainAccountNumber = ""; MainBankIsDefault = false; SelectedBankBranchId = 0;
                if (BankNameInput != null) BankNameInput.Text = "";
                if (CardNumberInput != null) CardNumberInput.Text = "";
                if (ShabaInput != null) ShabaInput.Text = "IR";
                if (AccountNumberInput != null) AccountNumberInput.Text = "";
                if (DefaultAccountToggle != null) DefaultAccountToggle.IsChecked = false;
                if (BankBranchCombo != null) BankBranchCombo.SelectedIndex = -1;
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Bank load failed: " + ex.Message); }
        }

        private async Task LoadCategoriesAndSelectAsync()
        {
            if (SelectedPersonTypeId <= 0) return;
            await LoadCategoriesAsync(SelectedPersonTypeId);
            if (_selectedPersonCategoryId.HasValue && _selectedPersonCategoryId.Value > 0 && CategorySearch != null)
                CategorySearch.SelectCategoryById(_selectedPersonCategoryId.Value);
        }

        private async Task LoadAndSetPictureAsync(List<PictureViewModel> pictures)
        {
            try
            {
                var picture = pictures.FirstOrDefault();
                if (picture == null || string.IsNullOrWhiteSpace(picture.Url)) return;
                var fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, picture.Url);
                if (System.IO.File.Exists(fullPath)) PersonImagePicker.ImagePath = fullPath;
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Picture load failed: " + ex.Message); }
        }

        private void CaptureInitialSnapshot()
        {
            _initialPerson = (
                FirstName, LastName, ContactFirstName, ContactLastName,
                NationalCode, CompanyName, EconomicCode, RegistrationNumber,
                ManualCode, IsLegal, IsActive, SelectedBranchId, SelectedPersonTypeId,
                Phone, Mobile, Email, PostalCode, Address,
                SelectedProvinceId, SelectedCityId);

            _initialBankAccounts = BankAccounts.Select(b => new BankAccountRow
            {
                BankBranchId = b.BankBranchId,
                BankName = b.BankName ?? "",
                BranchName = b.BranchName ?? "",
                CardNumber = b.CardNumber ?? "",
                Shaba = b.Shaba ?? "",
                AccountNumber = b.AccountNumber ?? "",
                IsDefault = b.IsDefault
            }).ToList();

            _initialImagePath = PersonImagePicker.ImagePath ?? "";

            // لود کامل شد — از این به بعد هر تغییری = تغییر کاربر
            _isLoading = false;
        }

        /// <summary>
        /// فقط تغییرات واقعی کاربر (بعد از پایان لود) محاسبه می‌شود؛
        /// بارگذاری موازی داده‌ها دیگر باعث سؤال اشتباه «ذخیره تغییرات» نمی‌شود.
        /// </summary>
        private bool ComputeUnsavedChanges()
        {
            return _userMadeChanges;
        }

        /// <summary>
        /// هر تغییری (توسط کاربر یا بایندینگ) از اینجا می‌گذرد؛
        /// تا وقتی فرم در حال لود است نادیده گرفته می‌شود.
        /// </summary>
        private void MarkUserChange()
        {
            if (_isLoading) return;
            _userMadeChanges = true;
            OnPropertyChanged(nameof(IsDirty));
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

        private void PersonTypeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            var tb = sender as ToggleButton;
            if (tb == null) return;

            var buttons = new[] { PersonTypeCustomer, PersonTypeSupplier, PersonTypeBoth, PersonTypePersonnel };
            bool anyChecked = false;
            foreach (var b in buttons)
            {
                if (b != null && b.IsChecked == true)
                {
                    anyChecked = true;
                    break;
                }
            }

            if (!anyChecked)
            {
                tb.IsChecked = true;
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

        // وضعیت تاگل «شناسه یکتا»: true = خودکار (کد را سیستم می‌سازد)، false = دستی
        private bool _isCodeAutomatic;

        private void CodeModeToggle_SelectionChanged(object sender, bool isFirstSelected)
        {
            _isCodeAutomatic = isFirstSelected;

            // در حالت خودکار فیلد کد غیرفعال است؛ بک‌اند خودش کد جدید می‌سازد
            if (ManualCodeTextBox != null)
                ManualCodeTextBox.IsEnabled = !isFirstSelected;

            MarkUserChange();
        }

        private void LegalTypeToggle_SelectionChanged(object sender, bool isFirstSelected)
        {
            IsLegal = !isFirstSelected;
        }

        private bool _isSaving;

        // Note: async void here is safe because:
        // 1. try/catch wraps the entire body
        // 2. _isSaving guard prevents reentrancy
        // 3. SaveButton is disabled during save
        private async void SavePerson()
        {
            if (_isSaving) return;
            if (_saveCts?.IsCancellationRequested == true) return;
            _isSaving = true;
            if (SaveButton != null) { SaveButton.IsEnabled = false; SaveButton.Text = "در حال ذخیره..."; }

            try
            {
                if (!ValidatePerson()) return;
                _saveCts.Token.ThrowIfCancellationRequested();

                var snapshot = CaptureEditSnapshot();
                var saveResult = await ExecuteEditSaveAsync(snapshot);

                if (!saveResult.IsSucceeded)
                {
                    ToastManager.Error(string.IsNullOrWhiteSpace(saveResult.Message) ? "ویرایش ناموفق بود." : saveResult.Message);
                    return;
                }

                ToastManager.Success("ویرایش شخص با موفقیت انجام شد.");
                BankAccounts.Clear();
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.CloseModal();
                if (mainWindow?.MainContent.Content is PersonListView listView)
                    _ = RefreshListViewSafeAsync(listView);
                else mainWindow?.NavigateTo(NavKeys.PersonList);
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[EditPersonView] SavePerson was cancelled");
            }
            catch (Exception ex)
            {
                if (_saveCts?.IsCancellationRequested != true)
                {
                    System.Diagnostics.Debug.WriteLine($"[EditPersonView] Edit error: {ex}");
                    ToastManager.Error("خطا در ویرایش");
                }
            }
            finally
            {
                if (_saveCts?.IsCancellationRequested != true)
                {
                    _isSaving = false;
                    if (SaveButton != null) { SaveButton.IsEnabled = true; SaveButton.Text = "ویرایش"; }
                }
            }
        }

        private record EditSaveSnapshot(
            long PersonId, bool IsLegal, string CompanyName, string FirstName, string LastName,
            string ContactFirstName, string ContactLastName, string NationalCode, string EconomicCode,
            string RegistrationNumber, long PersonTypeId, long BranchId, string ManualCode,
            long? PersonCategoryId, bool IsActive, bool IsCodeAutomatic,
            decimal CreditLimit,
            string Phone, string Mobile, string Email, Dictionary<string, long> ContactTypeNames,
            string Address, string PostalCode, long ProvinceId, long CityId,
            List<BankAccountRow> BankAccounts);

        private EditSaveSnapshot CaptureEditSnapshot()
        {
            return new EditSaveSnapshot(
                _personId, IsLegal, CompanyName, FirstName, LastName,
                ContactFirstName, ContactLastName, NationalCode, EconomicCode,
                RegistrationNumber, SelectedPersonTypeId, SelectedBranchId, ManualCode,
                _selectedPersonCategoryId, IsActive, _isCodeAutomatic,
                ParseCreditLimit(CreditLimitText),
                Phone?.Trim() ?? "", Mobile?.Trim() ?? "", Email?.Trim() ?? "",
                new Dictionary<string, long>(_contactTypeByName),
                Address, PostalCode, SelectedProvinceId, SelectedCityId,
                BankAccounts.Select(r => new BankAccountRow { BankBranchId = r.BankBranchId, BankName = r.BankName, CardNumber = r.CardNumber, Shaba = r.Shaba, AccountNumber = r.AccountNumber, IsDefault = r.IsDefault }).ToList());
        }

        /// <summary>
        /// تبدیل متن واردشده سقف اعتبار به decimal؛ ارقام فارسی به انگلیسی نرمال می‌شوند.
        /// </summary>
        private static decimal ParseCreditLimit(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0m;
            var sb = new System.Text.StringBuilder();
            foreach (var c in text.Trim())
            {
                if (c >= '۰' && c <= '۹') sb.Append((char)('0' + (c - '۰')));
                else sb.Append(c);
            }
            return decimal.TryParse(sb.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out var v) ? v : 0m;
        }

        private Task<OperationResult> ExecuteEditSaveAsync(EditSaveSnapshot s)
        {
            return Task.Run(() =>
            {
                using var scope = App.ServiceProvider.CreateScope();
                var sp = scope.ServiceProvider;
                var personApp = sp.GetRequiredService<IPersonApplication>();
                var contactApp = sp.GetRequiredService<IPersonContactApplication>();
                var addressApp = sp.GetRequiredService<IPersonAddressApplication>();
                var bankApp = sp.GetRequiredService<IPersonBankApplication>();

                var command = new EditPerson
                {
                    Id = s.PersonId, FirstName = s.IsLegal ? s.CompanyName : s.FirstName,
                    LastName = s.IsLegal ? "" : s.LastName,
                    ContactFirstName = s.IsLegal ? (s.ContactFirstName ?? "") : "",
                    ContactLastName = s.IsLegal ? (s.ContactLastName ?? "") : "",
                    NationalCode = s.IsLegal ? null : s.NationalCode,
                    EconomicCode = s.IsLegal ? s.EconomicCode : null,
                    RegistrationNumber = s.IsLegal ? s.RegistrationNumber : null,
                    IsLegal = s.IsLegal, PersonTypeId = s.PersonTypeId, BranchId = s.BranchId,
                    CreditLimit = s.CreditLimit, IsCodeAutomatic = s.IsCodeAutomatic,
                    ManualCode = s.ManualCode, PersonCategoryId = s.PersonCategoryId
                };
                var result = personApp.Edit(command);
                if (!result.IsSucceeded) return result;
                if (s.IsActive) personApp.Activate(s.PersonId); else personApp.Deactivate(s.PersonId);
                EditSaveContacts(contactApp, s.PersonId, s.ContactTypeNames, s.Phone, s.Mobile, s.Email);
                EditSaveAddress(addressApp, s.PersonId, s);
                EditSaveBanks(bankApp, s.PersonId, s.BankAccounts);
                return result;
            });
        }

        private static void EditSaveContacts(IPersonContactApplication app, long personId, Dictionary<string, long> typeNames, string phone, string mobile, string email)
        {
            foreach (var c in app.GetByPersonId(personId) ?? new List<PersonContactViewModel>()) app.Remove(c.Id);
            if (!string.IsNullOrWhiteSpace(phone) && typeNames.TryGetValue("تلفن ثابت", out var pt)) app.Create(new CreatePersonContact { PersonId = personId, ContactTypeId = pt, Value = phone, Description = "", IsDefault = false });
            if (!string.IsNullOrWhiteSpace(mobile) && typeNames.TryGetValue("موبایل", out var mt)) app.Create(new CreatePersonContact { PersonId = personId, ContactTypeId = mt, Value = mobile, Description = "", IsDefault = true });
            if (!string.IsNullOrWhiteSpace(email) && typeNames.TryGetValue("ایمیل", out var et)) app.Create(new CreatePersonContact { PersonId = personId, ContactTypeId = et, Value = email, Description = "", IsDefault = false });
        }

        private static void EditSaveAddress(IPersonAddressApplication app, long personId, EditSaveSnapshot s)
        {
            foreach (var a in app.GetByPersonId(personId) ?? new List<PersonAddressViewModel>()) app.Remove(a.Id);
            if (string.IsNullOrWhiteSpace(s.Address) && s.ProvinceId <= 0 && s.CityId <= 0) return;
            if (s.ProvinceId <= 0 || s.CityId <= 0) return;
            app.Create(new CreatePersonAddress { PersonId = personId, Title = "آدرس اصلی", Address = s.Address ?? "", PostalCode = s.PostalCode ?? "", ProvinceId = s.ProvinceId, CityId = s.CityId, IsDefault = true });
        }

        private static void EditSaveBanks(IPersonBankApplication app, long personId, List<BankAccountRow> rows)
        {
            foreach (var b in app.GetByPersonId(personId) ?? new List<PersonBankViewModel>()) app.Remove(b.Id);
            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.Shaba) && string.IsNullOrWhiteSpace(row.CardNumber)) continue;
                TryCreateBankAccountScoped(app, personId, row.BankBranchId, row.BankName, row.AccountNumber, row.CardNumber, row.Shaba, row.IsDefault);
            }
        }

        private static void TryCreateBankAccountScoped(IPersonBankApplication bankApp, long personId, long bankBranchId, string bankName, string accountNumber, string cardNumber, string shaba, bool isDefault)
        {
            if (bankBranchId <= 0) return;
            try
            {
                bankApp.Create(new CreatePersonBank
                {
                    PersonId = personId,
                    BankBranchId = bankBranchId,
                    AccountNumber = accountNumber ?? "",
                    CardNumber = cardNumber ?? "",
                    Shaba = shaba ?? "",
                    IsDefault = isDefault
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TryCreateBankAccountScoped exception: {ex.Message}");
            }
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
                    ToastManager.Warning("لطفاً ابتدا ردیف‌های قبلی را تکمیل کنید.");
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
                    ToastManager.Info(
                        "لطفاً ابتدا اطلاعات حساب بانکی را در فیلدهای بالا وارد کنید، سپس دکمه افزودن را بزنید.");
                    return;
                }

                // چک کن ردیف خالی وجود نداشته باشه
                if (HasEmptyBankRow())
                {
                    ToastManager.Warning("لطفاً ابتدا ردیف‌های قبلی را تکمیل کنید.");
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
                ToastManager.Warning("لطفاً ابتدا ردیف‌های قبلی را تکمیل کنید.");
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

        private void AddBankToTableButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BankNameInput.Text))
            {
                ToastManager.Warning("لطفاً نام بانک را وارد کنید.");
                return;
            }
            if (SelectedBankBranchId <= 0)
            {
                ToastManager.Warning("لطفاً شعبه بانک را انتخاب کنید.");
                return;
            }
            if (string.IsNullOrWhiteSpace(CardNumberInput.Text) && string.IsNullOrWhiteSpace(ShabaInput.Text))
            {
                ToastManager.Warning("لطفاً حداقل شماره کارت یا شماره شبا را وارد کنید.");
                return;
            }

            bool wantsDefault = DefaultAccountToggle.IsChecked == true;

            if (wantsDefault && BankAccounts.Any(r => r.IsDefault))
            {
                var result = MessageBox.Show(
                    "قابلیت پیش‌فرض فقط برای یک حساب فعال است. آیا از تغییر حساب پیش‌فرض مطمئن هستید؟",
                    "تغییر حساب پیش‌فرض",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            var branchName = "";
            if (BankBranchCombo.SelectedItem is BankBranchViewModel branch)
                branchName = branch.BranchName ?? "";

            if (wantsDefault)
            {
                foreach (var row in BankAccounts.Where(r => r.IsDefault))
                    row.IsDefault = false;
            }

            BankAccounts.Add(new BankAccountRow
            {
                BankBranchId = SelectedBankBranchId,
                BankName = BankNameInput.Text ?? "",
                BranchName = branchName,
                CardNumber = CardNumberInput.Text ?? "",
                Shaba = ShabaInput.Text ?? "",
                AccountNumber = AccountNumberInput.Text ?? "",
                IsDefault = wantsDefault
            });

            // اگه فقط یک حساب داریم و هیچ حساب پیش‌فرض نیست، پیش‌فرض شود
            if (!BankAccounts.Any(r => r.IsDefault) && !MainBankHasData())
                BankAccounts[0].IsDefault = true;

            ReindexBankAccounts();
            ClearBankForm();
        }

        private void BankAccountsTable_RemoveRequested(object sender, BankAccountRow row)
        {
            BankAccounts.Remove(row);
            ReindexBankAccounts();
        }

        private void BankAccountsTable_EditRequested(object sender, BankAccountRow row)
        {
            BankNameInput.Text = row.BankName;
            CardNumberInput.Text = row.CardNumber;
            var shaba = row.Shaba ?? "";
            ShabaInput.Text = shaba.StartsWith("IR") ? shaba : "IR" + shaba;
            AccountNumberInput.Text = row.AccountNumber;
            DefaultAccountToggle.IsChecked = row.IsDefault;
            SelectedBankBranchId = row.BankBranchId;

            BankAccounts.Remove(row);
            ReindexBankAccounts();
        }


        private static bool IsDigitChar(char c)
            => char.IsDigit(c) || (c >= '۰' && c <= '۹');

        private static int CountDigits(string text, int upToIndex)
        {
            int count = 0;
            int limit = Math.Min(upToIndex, text.Length);
            for (int i = 0; i < limit; i++)
                if (IsDigitChar(text[i]))
                    count++;
            return count;
        }

        private static int FindCaretPosition(string formatted, int digitIndex)
        {
            int count = 0;
            for (int i = 0; i < formatted.Length; i++)
            {
                if (IsDigitChar(formatted[i]))
                {
                    count++;
                    if (count >= digitIndex)
                        return i + 1;
                }
            }
            return formatted.Length;
        }

        // فرمت‌بندی شماره کارت به صورت ۴ رقمی
        private bool _isUpdatingCard;
        private void CardNumberInput_TextChanged()
        {
            if (_isUpdatingCard) return;
            var text = CardNumberInput.PART_TextBox.Text ?? "";

            int caret = CardNumberInput.PART_TextBox.CaretIndex;
            int digitsBefore = CountDigits(text, caret);

            var normalized = text
                .Replace("۰", "0").Replace("۱", "1").Replace("۲", "2").Replace("۳", "3").Replace("۴", "4")
                .Replace("۵", "5").Replace("۶", "6").Replace("۷", "7").Replace("۸", "8").Replace("۹", "9");

            var digits = new string(normalized.Where(char.IsDigit).Take(16).ToArray());
            var formatted = string.Join(" ", Enumerable.Range(0, (digits.Length + 3) / 4)
                .Select(i => digits.Substring(i * 4, Math.Min(4, digits.Length - i * 4))));

            if (text != formatted)
            {
                _isUpdatingCard = true;
                CardNumberInput.Text = formatted;
                CardNumberInput.PART_TextBox.CaretIndex = FindCaretPosition(formatted, digitsBefore);
                _isUpdatingCard = false;
            }
        }

        // آیکون درستی کد ملی هنگام تایپ
        private void NationalCodeInput_TextChanged()
        {
            string text = NationalCodeInput.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(text))
            {
                // فیلد خالی شده — state رو ریست کن
                NationalCodeInput.ValidationState = Controls.ValidationState.None;
                NationalCodeInput.ValidationMessage = "";
            }
            else if (text.Length < 10)
            {
                // هنوز کامل نشده — اگر قبلاً Invalid بوده، قرمز رو حفظ کن
                // فقط اگر None یا Valid بوده، Nothing نشون بده
                if (NationalCodeInput.ValidationState == Controls.ValidationState.Invalid)
                    return; // border قرمز حفظ شود تا ۱۰ رقم تکمیل شود
                NationalCodeInput.ValidationState = Controls.ValidationState.None;
                NationalCodeInput.ValidationMessage = "";
            }
            else if (ValidationHelper.IsValidNationalCode(text))
            {
                NationalCodeInput.ValidationState = Controls.ValidationState.Valid;
                NationalCodeInput.ValidationMessage = "";
            }
            else
            {
                NationalCodeInput.ValidationState = Controls.ValidationState.Invalid;
                NationalCodeInput.ValidationMessage = "کد ملی وارد شده صحیح نیست.";
            }
        }

        // فرمت‌بندی شماره شبا با پیشوند IR
        private bool _isUpdatingShaba;
        private void ShabaInput_TextChanged()
        {
            if (_isUpdatingShaba) return;
            var text = ShabaInput.PART_TextBox.Text;
            if (text == null) return;
            if (!text.StartsWith("IR"))
            {
                _isUpdatingShaba = true;
                var clean = text.Replace("IR", "").TrimStart();
                ShabaInput.Text = "IR" + clean;
                ShabaInput.PART_TextBox.CaretIndex = ShabaInput.Text.Length;
                _isUpdatingShaba = false;
            }
        }

        private void ReindexBankAccounts()
        {
            for (int i = 0; i < BankAccounts.Count; i++)
                BankAccounts[i].Index = i + 1;
        }

        private void ClearBankForm()
        {
            BankNameInput.Text = "";
            CardNumberInput.Text = "";
            ShabaInput.Text = "IR";
            AccountNumberInput.Text = "";
            DefaultAccountToggle.IsChecked = false;
            SelectedBankBranchId = 0;
            BankBranchCombo.SelectedIndex = -1;
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

        private bool ValidatePerson()
        {
            if (SelectedBranchId <= 0) { ToastManager.Warning("لطفاً شعبه را انتخاب کنید."); return false; }
            if (SelectedPersonTypeId <= 0) { ToastManager.Warning("لطفاً نوع شخص را انتخاب کنید."); return false; }

            // شماره موبایل: خالی = بدون آیکون؛ پر = باید 11 رقم و با 09 شروع شود
            if (string.IsNullOrWhiteSpace(Mobile))
            {
                MobileInput.ValidationState = Controls.ValidationState.None;
                MobileInput.ValidationMessage = "";
            }
            else if (!ValidationHelper.IsValidMobile(Mobile))
            {
                MobileInput.ValidationState = Controls.ValidationState.Invalid;
                MobileInput.ValidationMessage = "شماره موبایل باید ۱۱ رقم و با 09 شروع شود.";
                return false;
            }
            else
            {
                MobileInput.ValidationState = Controls.ValidationState.Valid;
                MobileInput.ValidationMessage = "";
            }

            // شماره تلفن ثابت: خالی = بدون آیکون؛ پر = 8 تا 11 رقم
            if (string.IsNullOrWhiteSpace(Phone))
            {
                PhoneInput.ValidationState = Controls.ValidationState.None;
                PhoneInput.ValidationMessage = "";
            }
            else if (!ValidationHelper.IsValidPhone(Phone))
            {
                PhoneInput.ValidationState = Controls.ValidationState.Invalid;
                PhoneInput.ValidationMessage = "شماره تلفن باید ۸ تا ۱۱ رقم باشد.";
                return false;
            }
            else
            {
                PhoneInput.ValidationState = Controls.ValidationState.Valid;
                PhoneInput.ValidationMessage = "";
            }

            if (IsLegal)
            {
                if (string.IsNullOrWhiteSpace(CompanyName)) { ToastManager.Warning("نام شرکت را وارد کنید."); return false; }
                if (string.IsNullOrWhiteSpace(EconomicCode)) { ToastManager.Warning("کد اقتصادی را وارد کنید."); return false; }
                if (string.IsNullOrWhiteSpace(ContactFirstName)) { ToastManager.Warning("نام فرد رابط را وارد کنید."); return false; }
                if (string.IsNullOrWhiteSpace(ContactLastName)) { ToastManager.Warning("نام خانوادگی فرد رابط را وارد کنید."); return false; }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(FirstName)) { ToastManager.Warning("نام را وارد کنید."); return false; }
                if (string.IsNullOrWhiteSpace(LastName)) { ToastManager.Warning("نام خانوادگی را وارد کنید."); return false; }
                if (string.IsNullOrWhiteSpace(NationalCode)) { ToastManager.Warning("کد ملی را وارد کنید."); return false; }
                if (NationalCode.Count(char.IsDigit) != 10) { ToastManager.Warning("کد ملی باید دقیقاً ۱۰ رقم باشد."); return false; }
                if (!ValidationHelper.IsValidNationalCode(NationalCode)) { ToastManager.Warning("کد ملی وارد شده صحیح نیست."); return false; }
            }

            if (!string.IsNullOrWhiteSpace(Email) && !ValidationHelper.IsValidEmail(Email))
            {
                ToastManager.Warning("فرمت ایمیل صحیح نیست. مثال صحیح: name@example.com");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(PostalCode) && !ValidationHelper.IsValidPostalCode(PostalCode))
            {
                ToastManager.Warning("کد پستی باید دقیقاً ۱۰ رقم باشد.");
                return false;
            }

            // اعتبارسنجی شماره شبا ردیف‌های جدول حساب‌های بانکی
            foreach (var row in BankAccounts)
            {
                var shaba = (row.Shaba ?? "").Trim().Replace(" ", "").ToUpper();
                if (shaba.Length > 2 && shaba != "IR" && !ValidationHelper.IsValidShaba(shaba))
                {
                    ToastManager.Warning(
                        $"فرمت شبا برای حساب «{row.BankName}» صحیح نیست. باید با IR شروع و در مجموع ۲۶ کاراکتر باشد.");
                    return false;
                }
            }

            return true;
        }

        private void SavePerson_Click(object sender, RoutedEventArgs e) => SavePerson();

        /// <summary>
        /// اگر تغییرات ذخیره‌نشده وجود داشته باشد، از کاربر می‌پرسد (ذخیره/انصراف/بستن).
        /// خروجی false یعنی بستن ادامه پیدا نکند (کاربر Cancel زده یا انتخاب کرده ذخیره کند).
        /// </summary>
        private bool ConfirmCloseWithUnsavedWarning()
        {
            if (!ComputeUnsavedChanges())
                return true;

            var result = MessageBox.Show(
                "تغییراتی که ایجاد کرده‌اید ذخیره نشده است.\nآیا می‌خواهید آن‌ها را ذخیره کنید؟",
                "ذخیره تغییرات",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SavePerson();
                return false; // ذخیره خودش فرم را می‌بندد
            }

            return result == MessageBoxResult.No;
        }

        private void Cancel_Click(object sender, MouseButtonEventArgs e)
        {
            if (!ConfirmCloseWithUnsavedWarning())
                return;

            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.CloseModal();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmCloseWithUnsavedWarning())
                return;

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


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            if (name != nameof(IsDirty))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDirty)));
        }

    }
}
