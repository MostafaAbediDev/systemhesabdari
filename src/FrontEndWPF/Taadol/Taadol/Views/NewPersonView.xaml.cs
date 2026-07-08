using _0_Framework.Application;
using BankManagement.Application.Contracts.BankBranch;
using CodeManagement.Application.Contracts.Code;
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
    /// <summary>
    /// فرم ثبت/ویرایش شخص.
    /// همه‌ی فیلدها به Property هایی با INotifyPropertyChanged بایند می‌شوند
    /// و در زمان ذخیره، تمام اطلاعات شخص + تماس‌ها + آدرس + حساب‌های بانکی
    /// از طریق Application های مربوطه ذخیره می‌شود.
    ///
    /// ★ محدودیت‌های بک‌اند (نسخه فعلی):
    ///   - CreatePerson/EditPerson فیلد IsActive ندارن.
    ///     → برای تنظیم IsActive از IPersonApplication.Activate(id) / Deactivate(id) استفاده می‌کنیم.
    ///   - OperationResult.Id برنمی‌گرده.
    ///     → بعد از Create، با Search(NationalCode/EconomicCode) شخص جدید رو پیدا می‌کنیم.
    ///   - GetDetails (EditPerson) IsActive برنمی‌گردونه.
    ///     → برای لود IsActive در Edit mode از Search استفاده می‌کنیم.
    /// </summary>
    public partial class NewPersonView : UserControl, INotifyPropertyChanged
    {
        // ===== Services (از DI رزولو می‌شوند) =====
        private readonly IPersonApplication _personApplication;
        private readonly IBranchApplication _branchApplication;
        private readonly IPersonTypeApplication _personTypeApplication;
        private readonly IContactTypeApplication _contactTypeApplication;
        private readonly IPersonContactApplication _personContactApplication;
        private readonly IPersonAddressApplication _personAddressApplication;
        private readonly IPersonBankApplication _personBankApplication;
        private readonly IProvinceRepository _provinceRepository;
        private readonly ICityRepository _cityRepository;
        // این سرویس اختیاری است (ممکن است در App.xaml.cs ثبت نشده باشد)
        private readonly IBankBranchApplication? _bankBranchApplication;
        private readonly ICodeGeneratorService _codeGeneratorService;
        private readonly IPersonCategoryApplication _personCategoryApplication;
        private long? _selectedPersonCategoryId;

        // ===== وضعیت ویرایش =====
        public bool IsEditMode { get; set; }
        public long PersonId { get; set; }

        // برای تشخیص تغییر IsActive در حالت Edit (آیا فعال/غیرفعال شده؟)
        private bool _originalIsActive = true;

        // ===== Command =====
        public ICommand SavePersonCommand { get; }

        // ===== Combo Sources =====
        public BulkObservableCollection<BranchComboItem> Branches { get; } = new();
        public BulkObservableCollection<PersonTypeViewModel> PersonTypes { get; } = new();
        public BulkObservableCollection<ProvinceViewModel> Provinces { get; } = new();
        public BulkObservableCollection<CityViewModel> Cities { get; } = new();
        public BulkObservableCollection<BankBranchViewModel> BankBranches { get; } = new();
        public ObservableCollection<BankAccountRow> BankAccounts { get; } = new();

        // ===== Lookup Cache =====
        private List<ContactTypeViewModel> _contactTypes = new();
        private readonly Dictionary<string, long> _contactTypeByName = new(StringComparer.OrdinalIgnoreCase);

        // ===== Property backing fields =====
        private string _firstName = "";
        private string _lastName = "";
        private string _contactFirstName = "";
        private string _contactLastName = "";
        private string _nationalCode = "";
        private string _companyName = "";
        private string _economicCode = "";
        private string _registrationNumber = "";
        private string _manualCode = "";
        private bool _isCodeAutomatic = true;
        private bool _isLegal = false;
        private bool _isActive = true;
        private long _selectedBranchId;
        private long _selectedPersonTypeId;
        private decimal _creditLimit;

        // Contact tab
        private string _phone = "";
        private string _mobile = "";
        private string _email = "";
        private string _postalCode = "";
        private string _address = "";

        // Address tab
        private long _selectedProvinceId;
        private long _selectedCityId;

        // Main bank account (موجودیت اول که همیشه در فرم نمایش داده می‌شود)
        private string _mainBankName = "";
        private string _mainBranchName = "";
        private string _mainCardNumber = "";
        private string _mainShaba = "";
        private string _mainAccountNumber = "";
        private bool _mainBankIsDefault = true;
        private long _selectedBankBranchId;

        // ===== Properties =====
        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(); } }
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(); } }

        /// <summary>نام فرد رابط (فقط برای شخص حقوقی) — فردی که از طرف شرکت با ما در ارتباط است</summary>
        public string ContactFirstName { get => _contactFirstName; set { _contactFirstName = value; OnPropertyChanged(); } }

        /// <summary>نام خانوادگی فرد رابط (فقط برای شخص حقوقی)</summary>
        public string ContactLastName { get => _contactLastName; set { _contactLastName = value; OnPropertyChanged(); } }

        public string NationalCode { get => _nationalCode; set { _nationalCode = value; OnPropertyChanged(); } }
        public string CompanyName { get => _companyName; set { _companyName = value; OnPropertyChanged(); } }
        public string EconomicCode { get => _economicCode; set { _economicCode = value; OnPropertyChanged(); } }
        public string RegistrationNumber { get => _registrationNumber; set { _registrationNumber = value; OnPropertyChanged(); } }
        public string ManualCode { get => _manualCode; set { _manualCode = value; OnPropertyChanged(); } }

        public bool IsCodeAutomatic
        {
            get => _isCodeAutomatic;
            set { _isCodeAutomatic = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// وقتی true باشه، TextBox شناسه یکتا قابل ویرایش است (حالت دستی).
        /// وقتی false باشه، TextBox غیرفعال و مقدارش از CodeGeneratorService میاد.
        /// </summary>
        public bool IsUniqueCodeManual { get => !_isCodeAutomatic; }

        public bool IsLegal
        {
            get => _isLegal;
            set { _isLegal = value; OnPropertyChanged(); UpdateLegalTypePanels(); }
        }

        public bool IsActive { get => _isActive; set { _isActive = value; OnPropertyChanged(); } }

        public long SelectedBranchId
        {
            get => _selectedBranchId;
            set { _selectedBranchId = value; OnPropertyChanged(); }
        }

        public long SelectedPersonTypeId
        {
            get => _selectedPersonTypeId;
            set
            {
                if (_selectedPersonTypeId == value) return;
                _selectedPersonTypeId = value;
                OnPropertyChanged();
                _ = LoadCategoriesAsync(value);
            }
        }
        public class BulkObservableCollection<T> : ObservableCollection<T>
        {
            public void AddRange(IEnumerable<T> items)
            {
                CheckReentrancy();
                foreach (var item in items)
                    Items.Add(item); // مستقیم به لیست داخلی، بدون Notify per-item

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
        private async Task LoadCategoriesAsync(long personTypeId)
        {
            if (personTypeId <= 0) return;

            // ★ این خط کلیدیه — بدون این، PersonTypeId کنترل همیشه 0 می‌مونه
            if (CategorySearch != null)
                CategorySearch.PersonTypeId = personTypeId;

            if (CategorySearch2 != null)
                CategorySearch2.PersonTypeId = personTypeId;

            if (CategorySearc3 != null)
                CategorySearc3.PersonTypeId = personTypeId;

            try
            {
                var tree = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IPersonCategoryApplication>();
                    return app.GetTree(personTypeId);
                });

                CategorySearch?.LoadFromTreeDto(tree);
                CategorySearch?.ClearSelection();

                // اگه CategorySearch2/3 باید درخت جدا (دپارتمان/شغل) داشته باشن،
                // باید GetTree جداگونه با PersonTypeId متفاوت صدا بزنید.
                // اگه فعلاً همون درخت رو نشون میدن، همینطور لود کنید:
                CategorySearch2?.LoadFromTreeDto(tree);
                CategorySearch2?.ClearSelection();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Categories load failed: " + ex.Message);
            }
        }
        public decimal CreditLimit { get => _creditLimit; set { _creditLimit = value; OnPropertyChanged(); } }

        // Contact
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(); } }
        public string Mobile { get => _mobile; set { _mobile = value; OnPropertyChanged(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }
        public string PostalCode { get => _postalCode; set { _postalCode = value; OnPropertyChanged(); } }
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }

        // Address
        public long SelectedProvinceId
        {
            get => _selectedProvinceId;
            set { _selectedProvinceId = value; OnPropertyChanged(); _ = LoadCitiesAsync(value); }
        }

        public long SelectedCityId
        {
            get => _selectedCityId;
            set { _selectedCityId = value; OnPropertyChanged(); }
        }

        // Main bank
        public string MainBankName { get => _mainBankName; set { _mainBankName = value; OnPropertyChanged(); } }
        public string MainBranchName { get => _mainBranchName; set { _mainBranchName = value; OnPropertyChanged(); } }
        public string MainCardNumber { get => _mainCardNumber; set { _mainCardNumber = value; OnPropertyChanged(); } }
        public string MainShaba { get => _mainShaba; set { _mainShaba = value; OnPropertyChanged(); } }
        public string MainAccountNumber { get => _mainAccountNumber; set { _mainAccountNumber = value; OnPropertyChanged(); } }
        public bool MainBankIsDefault { get => _mainBankIsDefault; set { _mainBankIsDefault = value; OnPropertyChanged(); } }
        public long SelectedBankBranchId { get => _selectedBankBranchId; set { _selectedBankBranchId = value; OnPropertyChanged(); } }

        // ======================================================
        //  Constructor
        // ======================================================
        public NewPersonView()
        {
            InitializeComponent();

            // سرویس‌های ضروری — اگه نباشن، فرم باز نمی‌شه
            _personApplication = App.ServiceProvider.GetRequiredService<IPersonApplication>();
            _branchApplication = App.ServiceProvider.GetRequiredService<IBranchApplication>();
            _personTypeApplication = App.ServiceProvider.GetRequiredService<IPersonTypeApplication>();
            _contactTypeApplication = App.ServiceProvider.GetRequiredService<IContactTypeApplication>();
            _personContactApplication = App.ServiceProvider.GetRequiredService<IPersonContactApplication>();
            _personAddressApplication = App.ServiceProvider.GetRequiredService<IPersonAddressApplication>();
            _personBankApplication = App.ServiceProvider.GetRequiredService<IPersonBankApplication>();
            _provinceRepository = App.ServiceProvider.GetRequiredService<IProvinceRepository>();
            _cityRepository = App.ServiceProvider.GetRequiredService<ICityRepository>();

            // سرویس اختیاری — اگه BankManagementBoostrapper در App.xaml.cs ثبت نشده باشه،
            // null برمی‌گردانه و فرم باز می‌شه (بدون لیست شعب بانک)
            _bankBranchApplication = App.ServiceProvider.GetService<IBankBranchApplication>();

            if (_bankBranchApplication == null)
            {
                System.Diagnostics.Debug.WriteLine(
                    "⚠️ IBankBranchApplication ثبت نشده. اطمینان حاصل کن که BankManagementBoostrapper.Configure در App.xaml.cs صدا زده شده.");
            }

            // سرویس تولید کد یکتا (از CodeManagement)
            _codeGeneratorService = App.ServiceProvider.GetRequiredService<ICodeGeneratorService>();
            _personCategoryApplication = App.ServiceProvider.GetRequiredService<IPersonCategoryApplication>();
            SavePersonCommand = new RelayCommand(() => SavePerson());
            DataContext = this;

            // مقداردهی اولیه شناسه یکتا در حالت اتوماتیک
            try
            {
                ManualCode = GenerateNextUniqueCode();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Initial code generation failed: " + ex.Message);
            }

            Loaded += OnLoaded;
            CategorySearch.CategorySelected += OnCategorySelected;
        }

        private void OnCategorySelected(CategorySearchControl.CategoryItem category)
        {
            _selectedPersonCategoryId = category.Id;
        }

        // ======================================================
        //  Async Loaders
        // ======================================================
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Branches.Count > 0) return;

            await Task.WhenAll(
                LoadBranchesAsync(),
                LoadPersonTypesAsync(),
                LoadContactTypesAsync(),
                LoadProvincesAsync(),
                LoadBankBranchesAsync()
            );

            // در حالت ویرایش، فرم را با داده‌ی شخص پر می‌کنیم
            if (IsEditMode && PersonId > 0)
                LoadPersonForEdit(PersonId);
        }

        private async Task LoadBranchesAsync()
        {
            ShowBranchComboLoading(true);

            try
            {
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IBranchApplication>();
                    return app.GetBranches()
                               .Select(b => new BranchComboItem { Id = b.Id, Title = b.Title })
                               .ToList(); // ← تبدیل هم داخل Task.Run انجام بشه، نه روی UI Thread
                });

                Branches.ReplaceAll(items); // ← یک Notify به‌جای N تا

                if (Branches.Count > 0 && SelectedBranchId == 0)
                    SelectedBranchId = Branches[0].Id;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطا در لود شعبه‌ها", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ShowBranchComboLoading(false);
            }
        }

        private void ShowBranchComboLoading(bool show)
        {
            if (BranchComboLoading != null)
                BranchComboLoading.Visibility = show ? Visibility.Visible : Visibility.Collapsed;

            if (BranchCombo != null)
                BranchCombo.IsEnabled = !show;
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

                PersonTypes.Clear();
                foreach (var t in items)
                    PersonTypes.Add(t);

                if (PersonTypes.Count > 0 && SelectedPersonTypeId == 0)
                {
                    SelectedPersonTypeId = PersonTypes[0].Id;
                    UpdatePersonTypeToggleSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطا در لود انواع شخص", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.Message, "خطا در لود انواع تماس", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(ex.Message, "خطا در لود استان‌ها", MessageBoxButton.OK, MessageBoxImage.Error);
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

                // اگر شهر انتخاب‌شده دیگر متعلق به این استان نیست، ریستش کن
                if (Cities.All(c => c.Id != SelectedCityId))
                    SelectedCityId = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطا در لود شهرها", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadBankBranchesAsync()
        {
            if (_bankBranchApplication == null)
            {
                System.Diagnostics.Debug.WriteLine("⚠️ IBankBranchApplication null — لیست شعب بانک لود نخواهد شد.");
                return;
            }

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

        // ======================================================
        //  Toggle Handlers
        // ======================================================
        private void CodeModeToggle_SelectionChanged(object sender, bool isFirstSelected)
        {
            IsCodeAutomatic = isFirstSelected;
            OnPropertyChanged(nameof(IsUniqueCodeManual));

            if (isFirstSelected)
            {
                ManualCode = GenerateNextUniqueCode();
                if (ManualCodeTextBox != null)
                    ManualCodeTextBox.IsEnabled = false;
            }
            else
            {
                ManualCode = "";
                if (ManualCodeTextBox != null)
                    ManualCodeTextBox.IsEnabled = true;
            }
        }

        private string GenerateNextUniqueCode()
        {
            try
            {
                return _codeGeneratorService.Generate(CodeOwnerTypeDTO.Person);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CodeGenerator failed: " + ex.Message);
                return "";
            }
        }

        private void LegalTypeToggle_SelectionChanged(object sender, bool isFirstSelected)
        {
            // isFirst = حقیقی → IsLegal = false
            IsLegal = !isFirstSelected;
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

                if (ContactPersonPanel != null)
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

                if (ContactPersonPanel != null)
                    ContactPersonPanel.Visibility = Visibility.Collapsed;
            }
        }

        // ======================================================
        //  Person Type Toggle buttons (مشتری/تامین‌کننده/هردو/پرسنل)
        // ======================================================
        private void PersonTypeToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton tb && tb.Tag != null)
            {
                if (long.TryParse(tb.Tag.ToString(), out var id))
                {
                    SelectedPersonTypeId = id;
                    UncheckOtherPersonTypeToggles(tb);
                }
            }
        }

        private void UncheckOtherPersonTypeToggles(ToggleButton keepChecked)
        {
            if (PersonTypeCustomer == null) return;
            var buttons = new[] { PersonTypeCustomer, PersonTypeSupplier, PersonTypeBoth, PersonTypePersonnel };
            foreach (var b in buttons)
            {
                if (b != null && b != keepChecked && b.IsChecked == true)
                    b.IsChecked = false;
            }
        }

        private void UpdatePersonTypeToggleSelection()
        {
            if (PersonTypeCustomer == null) return;
            var buttons = new[] { PersonTypeCustomer, PersonTypeSupplier, PersonTypeBoth, PersonTypePersonnel };
            foreach (var b in buttons)
            {
                if (b != null && b.Tag != null && long.TryParse(b.Tag.ToString(), out var id))
                    b.IsChecked = (id == SelectedPersonTypeId);
            }
        }

        // ======================================================
        //  Save
        // ======================================================
        private void SavePerson_Click(object sender, RoutedEventArgs e) => SavePerson();

        private void SavePerson(object obj = null)
        {
            // 1) تأیید کاربر
            var dialog = new CustomConfirmDialog();
            if (dialog.ShowDialog() != true) return;

            // 2) اعتبارسنجی
            if (!ValidatePerson()) return;

            // 3) ساخت Command (بدون IsActive — این فیلد در بک‌اند فعلی وجود نداره)
            var command = new CreatePerson
            {
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
                CreditLimit = CreditLimit,
                // ★ همیشه مقدار نمایش‌داده‌شده در TextBox را ذخیره کن.
                // بک‌اند وقتی IsAutomatic=true باشه، ManualCode را نادیده می‌گیره و کد جدید تولید می‌کنه
                // که باعث می‌شه کدی که کاربر می‌بینه با کدی که در DB ذخیره می‌شه فرق کنه.
                // چون IsCodeAutomatic در DB اصلاً ذخیره نمی‌شه (فقط flag موقت بک‌اند هست)،
                // ما IsAutomatic=false می‌فرستیم تا بک‌اند دقیقاً همون مقدار ManualCode را ذخیره کنه.
                // این کار هم برای حالت اتوماتیک (که کد از قبل توسط CodeGenerator تولید شده)
                // و هم حالت دستی (که کاربر خودش کد را تایپ کرده) درست کار می‌کنه.
                IsCodeAutomatic = false,
                ManualCode = ManualCode,
                PersonCategoryId = _selectedPersonCategoryId
           
        };

            try
            {
                OperationResult personResult;
                long personIdForChildren = 0;

                if (IsEditMode && PersonId > 0)
                {
                    var editCommand = new EditPerson
                    {
                        Id = PersonId,
                        FirstName = command.FirstName,
                        LastName = command.LastName,
                        ContactFirstName = command.ContactFirstName,
                        ContactLastName = command.ContactLastName,
                        NationalCode = command.NationalCode,
                        EconomicCode = command.EconomicCode,
                        RegistrationNumber = command.RegistrationNumber,
                        IsLegal = command.IsLegal,
                        PersonTypeId = command.PersonTypeId,
                        BranchId = command.BranchId,
                        CreditLimit = command.CreditLimit,
                        IsCodeAutomatic = command.IsCodeAutomatic,
                        ManualCode = command.ManualCode,
                        PersonCategoryId = _selectedPersonCategoryId
                    };
                    personResult = _personApplication.Edit(editCommand);
                    personIdForChildren = PersonId;

                    // ★ مدیریت IsActive در Edit mode:
                    // اگه کاربر وضعیت رو تغییر داده، Activate/Deactivate صدا بزن
                    if (personResult.IsSucceeded && IsActive != _originalIsActive)
                    {
                        if (IsActive)
                            _personApplication.Activate(personIdForChildren);
                        else
                            _personApplication.Deactivate(personIdForChildren);
                    }
                }
                else
                {
                    // Create
                    personResult = _personApplication.Create(command);

                    // ★ بعد از Create موفق، Id شخص جدید رو با Search پیدا کن
                    if (personResult.IsSucceeded)
                    {
                        personIdForChildren = GetCreatedPersonId(command);

                        // ★ مدیریت IsActive در Create mode:
                        // بک‌اند فعلی در Create هیچ راهی برای تنظیم IsActive نداره.
                        // پیش‌فرض دیتابیس معمولاً false هست، پس اگه کاربر فعال خواست، صراحتاً Activate صدا بزن.
                        if (personIdForChildren > 0)
                        {
                            if (IsActive)
                                _personApplication.Activate(personIdForChildren);
                            else
                                _personApplication.Deactivate(personIdForChildren);
                        }
                    }
                }

                if (!personResult.IsSucceeded)
                {
                    MessageBox.Show(
                        string.IsNullOrWhiteSpace(personResult.Message) ? "ثبت شخص ناموفق بود." : personResult.Message,
                        "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 4) ذخیره/همگام‌سازی موجودیت‌های فرزند (تماس/آدرس/بانک)
                if (personIdForChildren > 0)
                {
                    if (IsEditMode)
                    {
                        // در حالت ویرایش: ابتدا رکوردهای قدیمی حذف، سپس جدید ثبت می‌شه
                        SyncContactsInEdit(personIdForChildren);
                        SyncAddressInEdit(personIdForChildren);
                        SyncBanksInEdit(personIdForChildren);
                    }
                    else
                    {
                        SaveContacts(personIdForChildren);
                        SaveAddress(personIdForChildren);
                        SaveBanks(personIdForChildren);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "شخص ثبت شد ولی پیدا کردن شناسه‌ی او برای ذخیره‌ی تماس/آدرس/بانک ناموفق بود.",
                        "هشدار", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                MessageBox.Show(
                    IsEditMode ? "ویرایش شخص با موفقیت انجام شد." : "ثبت شخص با موفقیت انجام شد.",
                    "موفق", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearForm();
            }
            catch (Exception ex)
            {
                var fullMessage = BuildFullExceptionMessage(ex);
                MessageBox.Show(fullMessage, "خطا در ثبت شخص",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                try
                {
                    System.IO.File.AppendAllText(
                        System.IO.Path.Combine(System.IO.Path.GetTempPath(), "taadol-person-save-error.log"),
                        $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}]{Environment.NewLine}{fullMessage}{Environment.NewLine}{new string('-', 80)}{Environment.NewLine}");
                }
                catch
                {
                    // اگه نتونست در فایل بنویسه، کاری نمی‌کنیم
                }
            }
        }

        private static string BuildFullExceptionMessage(Exception ex)
        {
            if (ex == null) return "خطای ناشناخته.";

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Type: {ex.GetType().Name}");
            sb.AppendLine($"Message: {ex.Message}");

            var inner = ex.InnerException;
            int depth = 1;
            while (inner != null && depth <= 10)
            {
                sb.AppendLine();
                sb.AppendLine($"--- Inner Exception #{depth} ({inner.GetType().Name}) ---");
                sb.AppendLine($"Message: {inner.Message}");
                inner = inner.InnerException;
                depth++;
            }

            sb.AppendLine();
            sb.AppendLine("--- Stack Trace ---");
            sb.AppendLine(ex.StackTrace);

            return sb.ToString();
        }

        private bool ValidatePerson()
        {
            if (SelectedBranchId <= 0)
            {
                MessageBox.Show("لطفاً شعبه را انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (SelectedPersonTypeId <= 0)
            {
                MessageBox.Show("لطفاً نوع شخص را انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (IsLegal)
            {
                if (string.IsNullOrWhiteSpace(CompanyName))
                {
                    MessageBox.Show("نام شرکت را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (string.IsNullOrWhiteSpace(EconomicCode))
                {
                    MessageBox.Show("کد اقتصادی را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(ContactFirstName))
                {
                    MessageBox.Show("نام فرد رابط را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (string.IsNullOrWhiteSpace(ContactLastName))
                {
                    MessageBox.Show("نام خانوادگی فرد رابط را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(FirstName))
                {
                    MessageBox.Show("نام را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (string.IsNullOrWhiteSpace(LastName))
                {
                    MessageBox.Show("نام خانوادگی را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (string.IsNullOrWhiteSpace(NationalCode))
                {
                    MessageBox.Show("کد ملی را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                // ★ اعتبارسنجی فرمت کد ملی: باید ۱۰ رقم باشه
                if (!IsValidNationalCode(NationalCode))
                {
                    MessageBox.Show("کد ملی باید دقیقاً ۱۰ رقم باشد.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            if (!IsCodeAutomatic && string.IsNullOrWhiteSpace(ManualCode))
            {
                MessageBox.Show("شناسه یکتای دستی را وارد کنید یا حالت اتوماتیک را فعال کنید.",
                    "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // ★ در حالت اتوماتیک هم اطمینان حاصل کن که کد تولید شده و خالی نیست
            // (ممکنه CodeGenerator به هر دلیلی fail کرده باشه)
            if (IsCodeAutomatic && string.IsNullOrWhiteSpace(ManualCode))
            {
                // سعی کن یک بار دیگه کد تولید کنی
                ManualCode = GenerateNextUniqueCode();
                if (string.IsNullOrWhiteSpace(ManualCode))
                {
                    MessageBox.Show(
                        "تولید شناسه یکتای اتوماتیک ناموفق بود. لطفاً حالت دستی را انتخاب کرده و کد را وارد کنید.",
                        "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            // ★ اعتبارسنجی فرمت موبایل (در صورت وارد شدن)

            // ★ اعتبارسنجی فرمت موبایل (در صورت وارد شدن)
            if (!string.IsNullOrWhiteSpace(Mobile) && !IsValidMobile(Mobile))
            {
                MessageBox.Show("فرمت موبایل صحیح نیست. مثال صحیح: 09121234567", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // ★ اعتبارسنجی فرمت ایمیل (در صورت وارد شدن)
            if (!string.IsNullOrWhiteSpace(Email) && !IsValidEmail(Email))
            {
                MessageBox.Show("فرمت ایمیل صحیح نیست. مثال صحیح: name@example.com", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // ★ اعتبارسنجی فرمت شبا (در صورت وارد شدن)
            if (!string.IsNullOrWhiteSpace(MainShaba) && !IsValidShaba(MainShaba))
            {
                MessageBox.Show("فرمت شبا صحیح نیست. باید با IR شروع و در مجموع ۲۶ کاراکتر باشد.", "خطا",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        // ======================================================
        //  Validation Helpers (فرمت)
        // ======================================================
        private static bool IsValidNationalCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return false;
            code = code.Trim().Replace(" ", "");
            return code.Length == 10 && code.All(char.IsDigit);
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

        // ======================================================
        //  GetCreatedPersonId (بدون تغییر بک‌اند)
        // ======================================================

        /// <summary>
        /// از آنجا که OperationResult.Id برنمی‌گرده، شخص تازه‌ایجادشده رو
        /// با NationalCode (برای حقیقی) یا EconomicCode (برای حقوقی) جست‌وجو می‌کنیم.
        /// نکته: این روش شکننده‌ست — اگه دو کاربر هم‌زمان ثبت کنن، ممکنه Id اشتباه بگیریم.
        /// ولی در عمل برای کاربر تک‌نفری OK هست.
        /// </summary>
        private long GetCreatedPersonId(CreatePerson command)
        {
            try
            {
                var search = new PersonSearchModel
                {
                    // بک‌اند در Search، NationalCode رو هم روی EconomicCode اعمال می‌کنه (OR)
                    // پس برای هر دو حالت حقیقی/حقوقی می‌تونیم از همین فیلد استفاده کنیم
                    NationalCode = command.IsLegal ? command.EconomicCode : command.NationalCode
                };
                var list = _personApplication.Search(search);
                // جدیدترین شخص (بیشترین Id) رو بگیر
                return list.OrderByDescending(x => x.Id).FirstOrDefault()?.Id ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        // ======================================================
        //  Sync in Edit Mode (حذف قدیمی + ثبت جدید)
        // ======================================================

        /// <summary>
        /// در حالت ویرایش: حذف تمام تماس‌های قدیمی شخص و ثبت مجدد بر اساس فرم فعلی.
        /// رویکرد ساده و مطمئن — در آینده می‌تونه به Diff-based update ارتقا پیدا کنه.
        /// </summary>
        private void SyncContactsInEdit(long personId)
        {
            try
            {
                var existing = _personContactApplication.GetByPersonId(personId) ?? new List<PersonContactViewModel>();
                foreach (var c in existing)
                {
                    try { _personContactApplication.Remove(c.Id); } catch { /* ignore */ }
                }

                SaveContacts(personId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SyncContactsInEdit failed: " + ex.Message);
            }
        }

        /// <summary>
        /// در حالت ویرایش: حذف تمام آدرس‌های قدیمی و ثبت مجدد.
        /// </summary>
        private void SyncAddressInEdit(long personId)
        {
            try
            {
                var existing = _personAddressApplication.GetByPersonId(personId) ?? new List<PersonAddressViewModel>();
                foreach (var a in existing)
                {
                    try { _personAddressApplication.Remove(a.Id); } catch { /* ignore */ }
                }

                SaveAddress(personId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SyncAddressInEdit failed: " + ex.Message);
            }
        }

        /// <summary>
        /// در حالت ویرایش: حذف تمام حساب‌های بانکی قدیمی و ثبت مجدد.
        /// شامل حساب اصلی (MainBank*) و حساب‌های اضافه (BankAccounts).
        /// </summary>
        private void SyncBanksInEdit(long personId)
        {
            try
            {
                var existing = _personBankApplication.GetByPersonId(personId) ?? new List<PersonBankViewModel>();
                foreach (var b in existing)
                {
                    try { _personBankApplication.Remove(b.Id); } catch { /* ignore */ }
                }

                SaveBanks(personId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SyncBanksInEdit failed: " + ex.Message);
            }
        }

        // ======================================================
        //  Save Sub-Entities
        // ======================================================
        private void SaveContacts(long personId)
        {
            // تلفن ثابت
            if (!string.IsNullOrWhiteSpace(Phone) && _contactTypeByName.TryGetValue("تلفن ثابت", out var phoneTypeId))
            {
                _personContactApplication.Create(new CreatePersonContact
                {
                    PersonId = personId,
                    ContactTypeId = phoneTypeId,
                    Value = Phone.Trim(),
                    Description = "",  // ← ستون Description در DB NOT NULL است
                    IsDefault = false
                });
            }

            // موبایل
            if (!string.IsNullOrWhiteSpace(Mobile) && _contactTypeByName.TryGetValue("موبایل", out var mobileTypeId))
            {
                _personContactApplication.Create(new CreatePersonContact
                {
                    PersonId = personId,
                    ContactTypeId = mobileTypeId,
                    Value = Mobile.Trim(),
                    Description = "",
                    IsDefault = true
                });
            }

            // ایمیل
            if (!string.IsNullOrWhiteSpace(Email) && _contactTypeByName.TryGetValue("ایمیل", out var emailTypeId))
            {
                _personContactApplication.Create(new CreatePersonContact
                {
                    PersonId = personId,
                    ContactTypeId = emailTypeId,
                    Value = Email.Trim(),
                    Description = "",
                    IsDefault = false
                });
            }
        }

        private void SaveAddress(long personId)
        {
            if (string.IsNullOrWhiteSpace(Address) &&
                string.IsNullOrWhiteSpace(PostalCode) &&
                SelectedProvinceId <= 0 &&
                SelectedCityId <= 0)
                return;

            // چک کردن FK: ProvinceId و CityId در DB با Restrict هستن
            if (SelectedProvinceId <= 0 || SelectedCityId <= 0)
            {
                System.Diagnostics.Debug.WriteLine(
                    "⚠️ Address not saved: Province and City are required (FK constraint).");
                return;
            }

            try
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Address save failed: " + ex.Message);
            }
        }

        private void SaveBanks(long personId)
        {
            // حساب بانکی اصلی (اگر شماره شبا یا شماره کارت دارد)
            if (!string.IsNullOrWhiteSpace(MainShaba) || !string.IsNullOrWhiteSpace(MainCardNumber))
            {
                // ★ برای حساب اصلی، از SelectedBankBranchId استفاده می‌کنیم
                TryCreateBankAccount(personId, SelectedBankBranchId, MainBankName, MainAccountNumber, MainCardNumber, MainShaba, MainBankIsDefault);
            }

            // حساب‌های اضافه‌شده — هر ردیف BankBranchId خودش رو داره
            foreach (var row in BankAccounts)
            {
                if (string.IsNullOrWhiteSpace(row.Shaba) && string.IsNullOrWhiteSpace(row.CardNumber))
                    continue;

                TryCreateBankAccount(personId, row.BankBranchId, row.BankName, row.AccountNumber, row.CardNumber, row.Shaba, row.IsDefault);
            }
        }

        /// <summary>
        /// ★ پارامتر bankBranchId اضافه شد — حالا per-row کار می‌کنه (به‌جای global SelectedBankBranchId).
        /// </summary>
        private void TryCreateBankAccount(long personId, long bankBranchId, string bankName, string accountNumber, string cardNumber, string shaba, bool isDefault)
        {
            // نکته: BankBranchId در DB با Restrict هست و اگر 0 باشه، FK خطا می‌ده.
            if (bankBranchId <= 0)
            {
                System.Diagnostics.Debug.WriteLine(
                    "⚠️ Bank account not saved: BankBranch is required (FK constraint with Restrict). " +
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
                    System.Diagnostics.Debug.WriteLine("Bank save failed: " + result.Message);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Bank save exception: " + ex.Message);
            }
        }

        // ======================================================
        //  Edit Mode Loader
        // ======================================================
        public void LoadPerson(long id)
        {
            IsEditMode = true;
            PersonId = id;

            if (_personApplication == null) return;

            var person = _personApplication.GetDetails(id);
            if (person == null)
            {
                MessageBox.Show("شخص یافت نشد.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            FirstName = person.FirstName;
            LastName = person.LastName;

            ContactFirstName = person.ContactFirstName ?? "";
            ContactLastName = person.ContactLastName ?? "";

            // اگه شخص حقوقی هست، FirstName در واقع نام شرکت هست
            if (person.IsLegal)
                CompanyName = person.FirstName;

            NationalCode = person.NationalCode;
            EconomicCode = person.EconomicCode;
            RegistrationNumber = person.RegistrationNumber;
            IsLegal = person.IsLegal;
            SelectedPersonTypeId = person.PersonTypeId;
            SelectedBranchId = person.BranchId;
            CreditLimit = person.CreditLimit;
            ManualCode = person.ManualCode ?? person.CurrentCode ?? "";
            IsCodeAutomatic = person.IsCodeAutomatic;

            // ★ لود IsActive از PersonViewModel (چون GetDetails این فیلد رو نداره)
            try
            {
                var searchResult = _personApplication.Search(new PersonSearchModel());
                var vm = searchResult?.FirstOrDefault(x => x.Id == id);
                if (vm != null)
                {
                    IsActive = vm.IsActive;
                    _originalIsActive = vm.IsActive;
                }
                else
                {
                    IsActive = true;
                    _originalIsActive = true;
                }
            }
            catch
            {
                IsActive = true;
                _originalIsActive = true;
            }

            UpdateLegalTypePanels();
            UpdatePersonTypeToggleSelection();

            // لود دسته‌بندی شخص در حالت ویرایش
            if (person.PersonCategoryId.HasValue && person.PersonCategoryId > 0)
            {
                _selectedPersonCategoryId = person.PersonCategoryId;
                _ = LoadCategoriesAsync(person.PersonTypeId);
            }

            // ★ لود تماس‌ها (موبایل/تلفن/ایمیل) در حالت ویرایش
            LoadContactsForEdit(id);

            // ★ لود آدرس در حالت ویرایش
            LoadAddressForEdit(id);

            // ★ لود حساب‌های بانکی در حالت ویرایش
            LoadBanksForEdit(id);
        }

        /// <summary>
        /// لود تماس‌های شخص از DB و پر کردن فیلدهای Phone/Mobile/Email.
        /// بر اساس عنوان ContactType تطبیق می‌ده.
        /// </summary>
        private void LoadContactsForEdit(long personId)
        {
            try
            {
                var contacts = _personContactApplication.GetByPersonId(personId) ?? new List<PersonContactViewModel>();
                foreach (var c in contacts)
                {
                    var title = c.ContactTypeTitle?.Trim() ?? "";
                    if (title.Contains("موبایل"))
                        Mobile = c.Value ?? "";
                    else if (title.Contains("تلفن"))
                        Phone = c.Value ?? "";
                    else if (title.Contains("ایمیل"))
                        Email = c.Value ?? "";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadContactsForEdit failed: " + ex.Message);
            }
        }

        /// <summary>
        /// لود اولین آدرس شخص از DB و پر کردن فیلدهای استان/شهر/کدپستی/آدرس.
        /// </summary>
        private void LoadAddressForEdit(long personId)
        {
            try
            {
                var addresses = _personAddressApplication.GetByPersonId(personId) ?? new List<PersonAddressViewModel>();
                // اول آدرس پیش‌فرض، اگه نبود اولین آدرس
                var addr = addresses.FirstOrDefault(a => a.Address != null) ?? addresses.FirstOrDefault();
                if (addr == null) return;

                Address = addr.Address ?? "";
                PostalCode = addr.PostalCode ?? "";

                // ★ برای جلوگیری از پرش زنجیره‌ای (Province→City reload)،
                // ابتدا province ست می‌شه، بعد از لود شدن شهرها، city ست می‌شه.
                if (addr.ProvinceId > 0)
                {
                    SelectedProvinceId = addr.ProvinceId;
                    // شهرها async لود می‌شن؛ یه کوچک تأخیر برای ست کردن city
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (addr.CityId > 0)
                            SelectedCityId = addr.CityId;
                    }), System.Windows.Threading.DispatcherPriority.Background);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadAddressForEdit failed: " + ex.Message);
            }
        }

        /// <summary>
        /// لود حساب‌های بانکی شخص از DB.
        /// حساب پیش‌فرض → MainBank* fields
        /// بقیه → BankAccounts collection
        /// </summary>
        private void LoadBanksForEdit(long personId)
        {
            try
            {
                var banks = _personBankApplication.GetByPersonId(personId) ?? new List<PersonBankViewModel>();
                BankAccounts.Clear();

                // اول حساب پیش‌فرض رو پیدا کن
                var defaultBank = banks.FirstOrDefault(b => b.IsDefault) ?? banks.FirstOrDefault();

                if (defaultBank != null)
                {
                    SelectedBankBranchId = defaultBank.BankBranchId;
                    MainBankName = defaultBank.BankName ?? "";
                    MainBranchName = defaultBank.BankBranchName ?? "";
                    MainCardNumber = defaultBank.CardNumber ?? "";
                    MainShaba = defaultBank.Shaba ?? "";
                    MainAccountNumber = defaultBank.AccountNumber ?? "";
                    MainBankIsDefault = defaultBank.IsDefault;
                }

                // بقیه حساب‌ها رو به BankAccounts اضافه کن
                foreach (var b in banks)
                {
                    if (defaultBank != null && b.Id == defaultBank.Id) continue;

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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadBanksForEdit failed: " + ex.Message);
            }
        }

        private void LoadPersonForEdit(long id)
        {
            // این متد پس از لود اولیه‌ی ComboBoxها صدا زده می‌شود
            LoadPerson(id);
        }

        // ======================================================
        //  Helpers
        // ======================================================
        private void ClearForm()
        {
            FirstName = "";
            LastName = "";
            ContactFirstName = "";
            ContactLastName = "";
            NationalCode = "";
            CompanyName = "";
            EconomicCode = "";
            RegistrationNumber = "";
            ManualCode = "";
            IsCodeAutomatic = true;
            OnPropertyChanged(nameof(IsUniqueCodeManual));
            IsLegal = false;
            IsActive = true;
            _originalIsActive = true;
            CreditLimit = 0;
            _selectedPersonCategoryId = null;
            CategorySearch?.ClearSelection();
            Phone = "";
            Mobile = "";
            Email = "";
            PostalCode = "";
            Address = "";
            SelectedProvinceId = 0;
            SelectedCityId = 0;

            MainBankName = "";
            MainBranchName = "";
            MainCardNumber = "";
            MainShaba = "";
            MainAccountNumber = "";
            MainBankIsDefault = true;
            SelectedBankBranchId = 0;

            BankAccounts.Clear();

            IsEditMode = false;
            PersonId = 0;

            UpdatePersonTypeToggleSelection();

            // تولید شناسه یکتای جدید برای شخص بعدی
            try
            {
                ManualCode = GenerateNextUniqueCode();
                if (ManualCodeTextBox != null)
                    ManualCodeTextBox.IsEnabled = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Code generation after clear failed: " + ex.Message);
            }
        }

        // ======================================================
        //  Bank Accounts (Add/Remove)
        // ======================================================
        private void AddBankAccountButton_Click(object sender, RoutedEventArgs e)
        {
            BankAccounts.Add(new BankAccountRow());
        }

        private void RemoveBankAccount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is BankAccountRow row)
                BankAccounts.Remove(row);
        }

        // ======================================================
        //  Tab Switching (تماس/بانک/پرسنل)
        // ======================================================
        private void TabPricing_Checked(object sender, RoutedEventArgs e)
        {
            if (TabPricing == null || TabInventory == null || TabTax == null) return;

            TabInventory.IsChecked = false;
            TabTax.IsChecked = false;

            PricingContent.Visibility = Visibility.Visible;
            InventoryContent.Visibility = Visibility.Collapsed;
            TaxContent.Visibility = Visibility.Collapsed;
        }

        private void TabInventory_Checked(object sender, RoutedEventArgs e)
        {
            if (TabPricing == null || TabTax == null) return;

            TabPricing.IsChecked = false;
            TabTax.IsChecked = false;

            PricingContent.Visibility = Visibility.Collapsed;
            InventoryContent.Visibility = Visibility.Visible;
            TaxContent.Visibility = Visibility.Collapsed;
        }

        private void TabTax_Checked(object sender, RoutedEventArgs e)
        {
            if (TabPricing == null || TabInventory == null) return;

            TabPricing.IsChecked = false;
            TabInventory.IsChecked = false;

            PricingContent.Visibility = Visibility.Collapsed;
            InventoryContent.Visibility = Visibility.Collapsed;
            TaxContent.Visibility = Visibility.Visible;
        }

        private void CategoryToggle_Checked(object sender, RoutedEventArgs e)
        {
            if (PersonnelToggle == null || TabTax == null) return;

            bool isPersonnel = PersonnelToggle.IsChecked == true;
            TabTax.Visibility = isPersonnel ? Visibility.Visible : Visibility.Collapsed;

            if (!isPersonnel && TabTax.IsChecked == true)
                TabPricing.IsChecked = true;
        }

        // ======================================================
        //  Misc UI Handlers
        // ======================================================
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
        private void MyDatePicker_DateChanged(object sender, RoutedEventArgs e) { }
        private void Radio_Checked(object sender, RoutedEventArgs e) { }
        private void OnImageSelected(object sender, RoutedEventArgs e) { }
        private void OnImageRemoved(object sender, RoutedEventArgs e) { }
        private void ImagePickerControl_Loaded(object sender, RoutedEventArgs e) { }
        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e) { }
        private void TextBox_TextChanged_3(object sender, TextChangedEventArgs e) { }
        private void Button_Click(object sender, RoutedEventArgs e) { }
        private void ToggleSwitchControl_SelectionChanged(object sender, bool e) { }

        // ======================================================
        //  INotifyPropertyChanged
        // ======================================================
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // ======================================================
    //  Helper Models
    // ======================================================

    /// <summary>آیتم حساب بانکی اضافه‌شده توسط کاربر در فرم شخص</summary>
    public class BankAccountRow : INotifyPropertyChanged
    {
        private long _bankBranchId;
        private string _bankName = "";
        private string _branchName = "";
        private string _cardNumber = "";
        private string _shaba = "";
        private string _accountNumber = "";
        private bool _isDefault;

        /// <summary>شناسه‌ی شعبه بانک انتخاب‌شده برای این ردیف (per-row)</summary>
        public long BankBranchId
        {
            get => _bankBranchId;
            set { _bankBranchId = value; OnPC(); }
        }

        public string BankName { get => _bankName; set { _bankName = value; OnPC(); } }
        public string BranchName { get => _branchName; set { _branchName = value; OnPC(); } }
        public string CardNumber { get => _cardNumber; set { _cardNumber = value; OnPC(); } }
        public string Shaba { get => _shaba; set { _shaba = value; OnPC(); } }
        public string AccountNumber { get => _accountNumber; set { _accountNumber = value; OnPC(); } }
        public bool IsDefault { get => _isDefault; set { _isDefault = value; OnPC(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPC([CallerMemberName] string n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    // نکته: BranchComboItem قبلاً در NewFinancialPeriodView.xaml.cs تعریف شده و در اینجا reuse می‌شود.
}
