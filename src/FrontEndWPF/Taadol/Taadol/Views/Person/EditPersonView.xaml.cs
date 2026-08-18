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

        // ★ IsDirty property — true when user has unsaved changes
        public bool IsDirty => HasUnsavedChanges();

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
        public bool IsLegal { get => _isLegal; set { _isLegal = value; OnPropertyChanged(); MarkUserChange(); UpdateLegalTypePanels(); } }
        public bool IsActive { get => _isActive; set { _isActive = value; OnPropertyChanged(); MarkUserChange(); } }
        public long SelectedBranchId { get => _selectedBranchId; set { _selectedBranchId = value; OnPropertyChanged(); MarkUserChange(); } }
        public long SelectedPersonTypeId
        {
            get => _selectedPersonTypeId;
            set { _selectedPersonTypeId = value; OnPropertyChanged(); MarkUserChange(); _ = LoadCategoriesAsync(value); }
        }
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(); MarkUserChange(); } }
        public string Mobile { get => _mobile; set { _mobile = value; OnPropertyChanged(); MarkUserChange(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); MarkUserChange(); } }
        public string PostalCode { get => _postalCode; set { _postalCode = value; OnPropertyChanged(); MarkUserChange(); } }
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); MarkUserChange(); } }
        public long SelectedProvinceId { get => _selectedProvinceId; set { _selectedProvinceId = value; OnPropertyChanged(); MarkUserChange(); _ = LoadCitiesAsync(value); } }
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
            BankAccounts.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (BankAccountRow item in e.NewItems)
                        item.PropertyChanged += (_, _) => { OnPropertyChanged(nameof(IsDirty)); MarkUserChange(); };
                }
                if (e.OldItems != null)
                {
                    foreach (BankAccountRow item in e.OldItems)
                        item.PropertyChanged -= (_, _) => { OnPropertyChanged(nameof(IsDirty)); MarkUserChange(); };
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

            // Debug toast
            System.Diagnostics.Debug.WriteLine($"[DEBUG] EditPersonView constructor called for personId={personId}");
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] EditPersonView.OnLoaded fired");
                if (CategorySearch != null)
                    CategorySearch.CategorySelected += OnCategorySelected;

                // Subscribe to image picker changes for IsDirty
                if (PersonImagePicker != null)
                {
                    PersonImagePicker.ImageSelected += (_, _) => { OnPropertyChanged(nameof(IsDirty)); MarkUserChange(); };
                    PersonImagePicker.ImageRemoved += (_, _) => { OnPropertyChanged(nameof(IsDirty)); MarkUserChange(); };
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
                LoadPersonData();
                System.Diagnostics.Debug.WriteLine("[DEBUG] EditPersonView: LoadPersonData completed");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] ERROR in EditPersonView.OnLoaded: {ex}");
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
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IProvinceRepository>();
                    return repo.GetProvincesForSelectList();
                });
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
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<ICityRepository>();
                    return repo.GetCitiesByProvince(provinceId);
                });
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
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IBankBranchApplication>();
                    return app.GetBankBranches();
                });
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
                var personId = _personId;

                // ★ همه‌ی کوئری‌های مستقل همزمان (موازی) اجرا می‌شوند تا فرم زودتر لود شود
                var detailsTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IPersonApplication>();
                    return app.GetDetails(personId);
                });
                var contactsTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IPersonContactApplication>();
                    return app.GetByPersonId(personId) ?? new List<PersonContactViewModel>();
                });
                var addressesTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IPersonAddressApplication>();
                    return app.GetByPersonId(personId) ?? new List<PersonAddressViewModel>();
                });
                var banksTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IPersonBankApplication>();
                    return app.GetByPersonId(personId) ?? new List<PersonBankViewModel>();
                });
                var picturesTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IPictureApplication>();
                    return app.GetByOwner(personId, PictureOwnerTypeDTO.Person);
                });

                var details = await detailsTask;

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

                _selectedPersonCategoryId = details.PersonCategoryId;

                _selectedPersonTypeId = details.PersonTypeId;
                OnPropertyChanged(nameof(SelectedPersonTypeId));

                var personSearch = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IPersonApplication>();
                    return app.Search(new PersonSearchModel { NationalCode = details.NationalCode });
                });
                var personVm = personSearch?.FirstOrDefault(x => x.Id == _personId);
                IsActive = personVm?.IsActive ?? true;

                UpdatePersonTypeToggleSelection();

                var contacts = await contactsTask;
                foreach (var c in contacts)
                {
                    if (c.ContactTypeTitle != null && c.ContactTypeTitle.Contains("موبایل") && !string.IsNullOrWhiteSpace(c.Value))
                        Mobile = c.Value;
                    else if (c.ContactTypeTitle != null && c.ContactTypeTitle.Contains("تلفن") && !string.IsNullOrWhiteSpace(c.Value))
                        Phone = c.Value;
                    else if (c.ContactTypeTitle != null && c.ContactTypeTitle.Contains("ایمیل") && !string.IsNullOrWhiteSpace(c.Value))
                        Email = c.Value;
                }

                var addresses = await addressesTask;
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
                    var banks = await banksTask;
                    System.Diagnostics.Debug.WriteLine($"🏦 LoadPersonData: loaded {banks.Count} bank(s)");

                    BankAccounts.Clear();
                    foreach (var b in banks)
                    {
                        BankAccounts.Add(new BankAccountRow
                        {
                            BankBranchId = b.BankBranchId,
                            BankName = b.BankName ?? "",
                            BranchName = b.BankBranchName ?? "",
                            CardNumber = b.CardNumber ?? "",
                            Shaba = b.Shaba ?? "",
                            AccountNumber = b.AccountNumber ?? "",
                            IsDefault = b.IsDefault
                        });
                    }

                    System.Diagnostics.Debug.WriteLine($"🏦 LoadPersonData: {BankAccounts.Count} bank(s) added to grid");

                    if (BankAccounts.Count >= 1 && !BankAccounts.Any(r => r.IsDefault))
                        BankAccounts[0].IsDefault = true;

                    ReindexBankAccounts();

                    MainBankName = "";
                    MainCardNumber = "";
                    MainShaba = "IR";
                    MainAccountNumber = "";
                    MainBankIsDefault = false;
                    SelectedBankBranchId = 0;
                    if (BankNameInput != null) BankNameInput.Text = "";
                    if (CardNumberInput != null) CardNumberInput.Text = "";
                    if (ShabaInput != null) ShabaInput.Text = "IR";
                    if (AccountNumberInput != null) AccountNumberInput.Text = "";
                    if (DefaultAccountToggle != null) DefaultAccountToggle.IsChecked = false;

                    if (BankBranchCombo != null)
                        BankBranchCombo.SelectedIndex = -1;
                }
                catch (Exception bankEx)
                {
                    System.Diagnostics.Debug.WriteLine("Bank load failed: " + bankEx.Message);
                }

                if (SelectedPersonTypeId > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"📊 LoadPersonData: calling LoadCategoriesAsync for personTypeId={SelectedPersonTypeId}");
                    await LoadCategoriesAsync(SelectedPersonTypeId);

                    if (_selectedPersonCategoryId.HasValue && _selectedPersonCategoryId.Value > 0 && CategorySearch != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"📊 LoadPersonData: selecting category {_selectedPersonCategoryId.Value}");
                        CategorySearch.SelectCategoryById(_selectedPersonCategoryId.Value);
                    }
                }

                try
                {
                    var pictures = await picturesTask;
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

                // ★ صبر می‌کنیم بارگذاری‌های موازی (شهرها و بایندینگ ComboBox ها) جا بیفتند؛
                // وگرنه snapshot زودتر از حالت نهایی فرم گرفته می‌شود و «انصراف» بدون تغییر،
                // اشتباهاً سؤال «ذخیره تغییرات» می‌پرسد.
                try { await LoadCitiesAsync(SelectedProvinceId); } catch { }
                await Task.Delay(80);

                CaptureInitialSnapshot();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در لود اطلاعات: " + ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
        private bool HasUnsavedChanges()
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

        private void CodeModeToggle_SelectionChanged(object sender, bool isFirstSelected) { }

        private void LegalTypeToggle_SelectionChanged(object sender, bool isFirstSelected)
        {
            IsLegal = !isFirstSelected;
        }

        private bool _isSaving;

        private async void SavePerson()
        {
            if (_isSaving) return;
            _isSaving = true;
            if (SaveButton != null)
            {
                SaveButton.IsEnabled = false;
                SaveButton.Text = "در حال ذخیره...";
            }

            // ★ بدون دیالوگ تأیید و بدون ولیدیشن — بعد از ذخیره فقط یک بار اطلاع داده می‌شود

            try
            {
                var personId = _personId;
                var isLegal = IsLegal;
                var companyName = CompanyName;
                var firstName = FirstName;
                var lastName = LastName;
                var contactFirstName = ContactFirstName;
                var contactLastName = ContactLastName;
                var nationalCode = NationalCode;
                var economicCode = EconomicCode;
                var registrationNumber = RegistrationNumber;
                var selectedPersonTypeId = SelectedPersonTypeId;
                var selectedBranchId = SelectedBranchId;
                var manualCode = ManualCode;
                var selectedPersonCategoryId = _selectedPersonCategoryId;
                var isActive = IsActive;

                var contactPhone = Phone?.Trim();
                var contactMobile = Mobile?.Trim();
                var contactEmail = Email?.Trim();
                var contactTypeNames = new Dictionary<string, long>(_contactTypeByName);

                var addressText = Address;
                var postalCode = PostalCode;
                var selectedProvinceId = SelectedProvinceId;
                var selectedCityId = SelectedCityId;

                var bankAccounts = BankAccounts.Select(r => new BankAccountRow
                {
                    BankBranchId = r.BankBranchId,
                    BankName = r.BankName,
                    CardNumber = r.CardNumber,
                    Shaba = r.Shaba,
                    AccountNumber = r.AccountNumber,
                    IsDefault = r.IsDefault
                }).ToList();

                var saveResult = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var sp = scope.ServiceProvider;
                    var personApp = sp.GetRequiredService<IPersonApplication>();
                    var contactApp = sp.GetRequiredService<IPersonContactApplication>();
                    var addressApp = sp.GetRequiredService<IPersonAddressApplication>();
                    var bankApp = sp.GetRequiredService<IPersonBankApplication>();

                    var command = new EditPerson
                    {
                        Id = personId,
                        FirstName = isLegal ? companyName : firstName,
                        LastName = isLegal ? "" : lastName,
                        ContactFirstName = isLegal ? (contactFirstName ?? "") : "",
                        ContactLastName = isLegal ? (contactLastName ?? "") : "",
                        NationalCode = isLegal ? null : nationalCode,
                        EconomicCode = isLegal ? economicCode : null,
                        RegistrationNumber = isLegal ? registrationNumber : null,
                        IsLegal = isLegal,
                        PersonTypeId = selectedPersonTypeId,
                        BranchId = selectedBranchId,
                        CreditLimit = 0,
                        IsCodeAutomatic = false,
                        ManualCode = manualCode,
                        PersonCategoryId = selectedPersonCategoryId
                    };

                    var result = personApp.Edit(command);
                    if (!result.IsSucceeded)
                        return result;

                    if (isActive)
                        personApp.Activate(personId);
                    else
                        personApp.Deactivate(personId);

                    // SaveContacts
                    var existingContacts = contactApp.GetByPersonId(personId) ?? new List<PersonContactViewModel>();
                    foreach (var c in existingContacts)
                        contactApp.Remove(c.Id);

                    if (!string.IsNullOrWhiteSpace(contactPhone) && contactTypeNames.TryGetValue("تلفن ثابت", out var phoneTypeId))
                        contactApp.Create(new CreatePersonContact { PersonId = personId, ContactTypeId = phoneTypeId, Value = contactPhone, Description = "", IsDefault = false });

                    if (!string.IsNullOrWhiteSpace(contactMobile) && contactTypeNames.TryGetValue("موبایل", out var mobileTypeId))
                        contactApp.Create(new CreatePersonContact { PersonId = personId, ContactTypeId = mobileTypeId, Value = contactMobile, Description = "", IsDefault = true });

                    if (!string.IsNullOrWhiteSpace(contactEmail) && contactTypeNames.TryGetValue("ایمیل", out var emailTypeId))
                        contactApp.Create(new CreatePersonContact { PersonId = personId, ContactTypeId = emailTypeId, Value = contactEmail, Description = "", IsDefault = false });

                    // SaveAddress
                    var existingAddresses = addressApp.GetByPersonId(personId) ?? new List<PersonAddressViewModel>();
                    foreach (var a in existingAddresses)
                        addressApp.Remove(a.Id);

                    if (!string.IsNullOrWhiteSpace(addressText) || selectedProvinceId > 0 || selectedCityId > 0)
                    {
                        if (selectedProvinceId > 0 && selectedCityId > 0)
                        {
                            addressApp.Create(new CreatePersonAddress
                            {
                                PersonId = personId,
                                Title = "آدرس اصلی",
                                Address = addressText ?? "",
                                PostalCode = postalCode ?? "",
                                ProvinceId = selectedProvinceId,
                                CityId = selectedCityId,
                                IsDefault = true
                            });
                        }
                    }

                    // SaveBank — all accounts from grid only
                    var existingBanks = bankApp.GetByPersonId(personId) ?? new List<PersonBankViewModel>();
                    foreach (var b in existingBanks)
                        bankApp.Remove(b.Id);

                    foreach (var row in bankAccounts)
                    {
                        if (string.IsNullOrWhiteSpace(row.Shaba) && string.IsNullOrWhiteSpace(row.CardNumber))
                            continue;
                        TryCreateBankAccountScoped(bankApp, personId, row.BankBranchId, row.BankName, row.AccountNumber, row.CardNumber, row.Shaba, row.IsDefault);
                    }

                    return result;
                });

                if (!saveResult.IsSucceeded)
                {
                    MessageBox.Show(string.IsNullOrWhiteSpace(saveResult.Message) ? "ویرایش ناموفق بود." : saveResult.Message,
                        "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ToastManager.Success("ویرایش شخص با موفقیت انجام شد.");

                BankAccounts.Clear();

                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.CloseModal();
                mainWindow?.NavigateTo("person_list");
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطا در ویرایش: " + ex.Message, "خطا", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void AddBankToTableButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BankNameInput.Text))
            {
                MessageBox.Show("لطفاً نام بانک را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SelectedBankBranchId <= 0)
            {
                MessageBox.Show("لطفاً شعبه بانک را انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(CardNumberInput.Text) && string.IsNullOrWhiteSpace(ShabaInput.Text))
            {
                MessageBox.Show("لطفاً حداقل شماره کارت یا شماره شبا را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void RemoveBankAccountFromTable_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BankAccountRow row)
            {
                var result = MessageBox.Show("آیا از حذف این حساب بانکی مطمئن هستید؟", "تایید حذف", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) return;

                BankAccounts.Remove(row);
                ReindexBankAccounts();
            }
        }

        private void EditBankAccount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BankAccountRow row)
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
        }

        private void TableContainerBorder_SizeChanged(object sender, SizeChangedEventArgs e) { }

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

        // ★ متد کمکی برای ساخت حساب بانکی (legacy - replaced by TryCreateBankAccountScoped in SavePerson)
        private void TryCreateBankAccount(long personId, long bankBranchId, string bankName, string accountNumber, string cardNumber, string shaba, bool isDefault)
        {
            if (bankBranchId <= 0) return;

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
                if (!IsValidNationalCode(NationalCode)) { MessageBox.Show("کد ملی باید دقیقاً ۱۰ رقم باشد.", "کد ملی", MessageBoxButton.OK, MessageBoxImage.Warning); return false; }
            }

            if (!string.IsNullOrWhiteSpace(Mobile) && !IsValidMobile(Mobile))
            {
                MessageBox.Show("فرمت موبایل صحیح نیست. مثال صحیح: 09121234567", "فرمت موبایل", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(Email) && !IsValidEmail(Email))
            {
                MessageBox.Show("فرمت ایمیل صحیح نیست. مثال صحیح: name@example.com", "فرمت ایمیل", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!string.IsNullOrWhiteSpace(MainShaba) && !IsValidShaba(MainShaba))
            {
                MessageBox.Show("فرمت شبا صحیح نیست. باید با IR شروع و در مجموع ۲۶ کاراکتر باشد.", "فرمت شبا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // ======================================================
        //  Validation Helpers
        // ======================================================
        private static bool IsValidNationalCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return false;
            code = code.Trim().Replace(" ", "").Replace("-", "");
            if (code.Length != 10 || !code.All(char.IsDigit)) return false;

            int[] weights = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int sum = 0;
            for (int i = 0; i < 9; i++)
                sum += (code[i] - '0') * weights[i];

            int remainder = sum % 11;
            int checkDigit = remainder < 2 ? remainder : 11 - remainder;
            return checkDigit == (code[9] - '0');
        }

        private static bool IsValidMobile(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile)) return false;
            mobile = mobile.Trim().Replace(" ", "").Replace("-", "");
            if (mobile.StartsWith("+98")) mobile = "0" + mobile.Substring(3);
            else if (mobile.StartsWith("0098")) mobile = "0" + mobile.Substring(4);
            return mobile.Length == 11 && mobile.StartsWith("09") && mobile.All(char.IsDigit);
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email.Trim());
                return addr.Address == email.Trim();
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidShaba(string shaba)
        {
            if (string.IsNullOrWhiteSpace(shaba)) return false;
            shaba = shaba.Trim().Replace(" ", "").ToUpper();
            return shaba.StartsWith("IR") && shaba.Length == 26 && shaba.Substring(2).All(char.IsDigit);
        }

        private void OnImageSelected(object sender, RoutedEventArgs e) { }
        private void OnImageRemoved(object sender, RoutedEventArgs e) { }
        private void ImagePickerControl_Loaded(object sender, RoutedEventArgs e) { }

        private void SavePerson_Click(object sender, RoutedEventArgs e) => SavePerson();

        private void Cancel_Click(object sender, MouseButtonEventArgs e)
        {
            if (HasUnsavedChanges())
            {
                var result = MessageBox.Show(
                    "تغییراتی که ایجاد کرده‌اید ذخیره نشده است.\nآیا می‌خواهید آن‌ها را ذخیره کنید؟",
                    "ذخیره تغییرات",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SavePerson();
                    return;
                }
                if (result == MessageBoxResult.Cancel)
                    return;
            }

            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.CloseModal();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
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
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            if (name != nameof(IsDirty))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDirty)));
        }

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
            private int _index;

            public int Index { get => _index; set { _index = value; OnPropertyChanged(); } }
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
