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
using PersonManagement.Application.Contract.PersonContact;
using PersonManagement.Application.Contract.Persons;
using PersonManagement.Application.Contract.PersonTypes;
using System.Collections.ObjectModel;
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
    /// و در زمان ذخیره، toànوان اطلاعات شخص + تماس‌ها + آدرس + حساب‌های بانکی
    /// از طریق Application های مربوطه ذخیره می‌شود.
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

        // ===== وضعیت ویرایش =====
        public bool IsEditMode { get; set; }
        public long PersonId { get; set; }

        // ===== Command =====
        public ICommand SavePersonCommand { get; }

        // ===== Combo Sources =====
        public ObservableCollection<BranchComboItem> Branches { get; } = new();
        public ObservableCollection<PersonTypeViewModel> PersonTypes { get; } = new();
        public ObservableCollection<ProvinceViewModel> Provinces { get; } = new();
        public ObservableCollection<CityViewModel> Cities { get; } = new();
        public ObservableCollection<BankBranchViewModel> BankBranches { get; } = new();
        public ObservableCollection<BankAccountRow> BankAccounts { get; } = new();

        // ===== Lookup Cache =====
        private List<ContactTypeViewModel> _contactTypes = new();
        private readonly Dictionary<string, long> _contactTypeByName = new(StringComparer.OrdinalIgnoreCase);

        // ===== Property backing fields =====
        private string _firstName = "";
        private string _lastName = "";
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
            set { _selectedPersonTypeId = value; OnPropertyChanged(); }
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

            SavePersonCommand = new RelayCommand(() => SavePerson());
            DataContext = this;

            // مقداردهی اولیه شناسه یکتا در حالت اتوماتیک
            // (الگو از NewBranchView.LoadInitialDataAsync)
            try
            {
                ManualCode = GenerateNextUniqueCode();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Initial code generation failed: " + ex.Message);
            }

            Loaded += OnLoaded;
        }

        // ======================================================
        //  Async Loaders
        // ======================================================
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // از تکرار جلوگیری می‌کنیم (در صورت Load مجدد)
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
            // نمایش اسپینر لود و غیرفعال‌کردن ComboBox
            ShowBranchComboLoading(true);

            // دادن فرصت به UI برای نمایش اسپینر قبل از شروع کار سنگین
            await Dispatcher.InvokeAsync(() => { }, System.Windows.Threading.DispatcherPriority.Background);

            try
            {
                var items = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IBranchApplication>();
                    return app.GetBranches();
                });

                Branches.Clear();
                foreach (var b in items)
                    Branches.Add(new BranchComboItem { Id = b.Id, Title = b.Title });

                // انتخاب اولین آیتم به‌صورت پیش‌فرض
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

        /// <summary>
        /// نمایش/مخفی‌کردن اسپینر لود روی ComboBox شعبه.
        /// الگو از NewBranchView.ShowCompanyComboLoading برداشته شده.
        /// </summary>
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

                // انتخاب پیش‌فرض: اولین نوع
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
            // اگه سرویس بانک ثبت نشده، فقط لاگ می‌کنیم و برمی‌گردیم
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
                // در صورت عدم دسترسی به جدول شعب بانک، فقط لاگ می‌کنیم تا فرم باز بماند
                System.Diagnostics.Debug.WriteLine("BankBranches load failed: " + ex.Message);
            }
        }

        // ======================================================
        //  Toggle Handlers
        // ======================================================

        /// <summary>
        /// تoggle اتوماتیک/دستی برای شناسه یکتا
        /// الگو از NewBranchView.UniqueCodeMode_SelectionChanged برداشته شده.
        /// </summary>
        private void CodeModeToggle_SelectionChanged(object sender, bool isFirstSelected)
        {
            IsCodeAutomatic = isFirstSelected;
            OnPropertyChanged(nameof(IsUniqueCodeManual));

            if (isFirstSelected)
            {
                // حالت اتوماتیک: TextBox غیرفعال، شناسه از CodeGenerator تولید می‌شه
                ManualCode = GenerateNextUniqueCode();

                if (ManualCodeTextBox != null)
                    ManualCodeTextBox.IsEnabled = false;
            }
            else
            {
                // حالت دستی: TextBox فعال، کاربر خودش می‌نویسه
                ManualCode = "";

                if (ManualCodeTextBox != null)
                    ManualCodeTextBox.IsEnabled = true;
            }
        }

        /// <summary>
        /// تولید شناسه یکتا برای شخص با استفاده از CodeGeneratorService.
        /// پیشوند شخص "PE-" است (طبق CodeGeneratorService.Generate).
        /// </summary>
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

        /// <summary>تoggle حقیقی/حقوقی</summary>
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
            }
            else
            {
                FirstNamePanel.Visibility = Visibility.Visible;
                LastNamePanel.Visibility = Visibility.Visible;
                NationalCodePanel.Visibility = Visibility.Visible;

                CompanyNamePanel.Visibility = Visibility.Collapsed;
                EconomicCodePanel.Visibility = Visibility.Collapsed;
                RegistrationNumberPanel.Visibility = Visibility.Collapsed;
            }
        }

        // ======================================================
        //  Person Type Toggle Buttons (مشتری/تامین‌کننده/هردو/پرسنل)
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

            // 3) ساخت Command
            var command = new CreatePerson
            {
                FirstName = IsLegal ? CompanyName : FirstName,
                LastName = IsLegal ? "" : LastName,
                NationalCode = IsLegal ? null : NationalCode,
                EconomicCode = IsLegal ? EconomicCode : null,
                RegistrationNumber = IsLegal ? RegistrationNumber : null,
                IsLegal = IsLegal,
                PersonTypeId = SelectedPersonTypeId,
                BranchId = SelectedBranchId,
                CreditLimit = CreditLimit,
                IsCodeAutomatic = IsCodeAutomatic,
                ManualCode = IsCodeAutomatic ? null : ManualCode
            };

            try
            {
                OperationResult personResult;

                if (IsEditMode && PersonId > 0)
                {
                    var editCommand = new EditPerson
                    {
                        Id = PersonId,
                        FirstName = command.FirstName,
                        LastName = command.LastName,
                        NationalCode = command.NationalCode,
                        EconomicCode = command.EconomicCode,
                        RegistrationNumber = command.RegistrationNumber,
                        IsLegal = command.IsLegal,
                        PersonTypeId = command.PersonTypeId,
                        BranchId = command.BranchId,
                        CreditLimit = command.CreditLimit,
                        IsCodeAutomatic = command.IsCodeAutomatic,
                        ManualCode = command.ManualCode
                    };
                    personResult = _personApplication.Edit(editCommand);
                }
                else
                {
                    personResult = _personApplication.Create(command);
                }

                if (!personResult.IsSucceeded)
                {
                    MessageBox.Show(
                        string.IsNullOrWhiteSpace(personResult.Message) ? "ثبت شخص ناموفق بود." : personResult.Message,
                        "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // 4) ذخیره‌ی موجودیت‌های فرزند (تنها در حالت Create)
                if (!IsEditMode)
                {
                    var newPersonId = GetCreatedPersonId(command);
                    if (newPersonId > 0)
                    {
                        SaveContacts(newPersonId);
                        SaveAddress(newPersonId);
                        SaveBanks(newPersonId);
                    }
                    else
                    {
                        MessageBox.Show(
                            "شخص ثبت شد ولی پیدا کردن شناسه‌ی او برای ذخیره‌ی تماس/آدرس/بانک ناموفق بود.",
                            "هشدار", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                MessageBox.Show(
                    IsEditMode ? "ویرایش شخص با موفقیت انجام شد." : "ثبت شخص با موفقیت انجام شد.",
                    "موفق", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearForm();
            }
            catch (Exception ex)
            {
                // نمایش خطای کامل شامل Inner Exception ها
                // (پیام اصلی معمولاً "An error occurred while saving..." هست و علت واقعی در Inner است)
                var fullMessage = BuildFullExceptionMessage(ex);
                MessageBox.Show(fullMessage, "خطا در ثبت شخص",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                // همزمان در فایل لاگ هم بنویس
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

        /// <summary>
        /// ساخت پیام خطای کامل شامل تمام Inner Exception ها (برای دیباگ خطاهای EF Core)
        /// </summary>
        private static string BuildFullExceptionMessage(Exception ex)
        {
            if (ex == null) return "خطای ناشناخته.";

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"🔴 Type: {ex.GetType().Name}");
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
            }

            if (!IsCodeAutomatic && string.IsNullOrWhiteSpace(ManualCode))
            {
                MessageBox.Show("شناسه یکتای دستی را وارد کنید یا حالت اتوماتیک را فعال کنید.",
                    "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// از آنجا که OperationResult شناسه‌ی رکورد جدید را برنمی‌گرداند،
        /// شخص تازه‌ایجادشده را با NationalCode یا EconomicCode جست‌وجو می‌کنیم.
        /// </summary>
        private long GetCreatedPersonId(CreatePerson command)
        {
            try
            {
                var search = new PersonSearchModel
                {
                    NationalCode = command.IsLegal ? command.EconomicCode : command.NationalCode
                };
                var list = _personApplication.Search(search);
                return list.FirstOrDefault()?.Id ?? 0;
            }
            catch
            {
                return 0;
            }
        }

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
                    Description = "",  // ← ستون Description در DB NOT NULL است
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
                    Description = "",  // ← ستون Description در DB NOT NULL است
                    IsDefault = false
                });
            }
        }

        private void SaveAddress(long personId)
        {
            // اگه هیچ آدرسی وارد نشده، چیزی ذخیره نکن
            if (string.IsNullOrWhiteSpace(Address) &&
                string.IsNullOrWhiteSpace(PostalCode) &&
                SelectedProvinceId <= 0 &&
                SelectedCityId <= 0)
                return;

            // چک کردن FK: ProvinceId و CityId در DB با Restrict هستن
            // اگر کاربر آدرس وارد کرده ولی استان/شهر انتخاب نکرده، هشدار بده و رد شو
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
                TryCreateBankAccount(personId, MainBankName, MainAccountNumber, MainCardNumber, MainShaba, MainBankIsDefault);
            }

            // حساب‌های اضافه‌شده
            foreach (var row in BankAccounts)
            {
                if (string.IsNullOrWhiteSpace(row.Shaba) && string.IsNullOrWhiteSpace(row.CardNumber))
                    continue;

                TryCreateBankAccount(personId, row.BankName, row.AccountNumber, row.CardNumber, row.Shaba, row.IsDefault);
            }
        }

        private void TryCreateBankAccount(long personId, string bankName, string accountNumber, string cardNumber, string shaba, bool isDefault)
        {
            // نکته: BankBranchId در DB با Restrict هست و اگر 0 باشه، FK خطا می‌ده.
            // اگر کاربر شعبه بانک انتخاب نکرده، فقط لاگ می‌کنیم و رد می‌شیم.
            if (SelectedBankBranchId <= 0)
            {
                System.Diagnostics.Debug.WriteLine(
                    "⚠️ Bank account not saved: BankBranch is required (FK constraint with Restrict). " +
                    "BankBranchId = 0");
                return;
            }

            try
            {
                var result = _personBankApplication.Create(new CreatePersonBank
                {
                    PersonId = personId,
                    BankBranchId = SelectedBankBranchId,
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
            NationalCode = person.NationalCode;
            EconomicCode = person.EconomicCode;
            RegistrationNumber = person.RegistrationNumber;
            IsLegal = person.IsLegal;
            SelectedPersonTypeId = person.PersonTypeId;
            SelectedBranchId = person.BranchId;
            CreditLimit = person.CreditLimit;
            ManualCode = person.ManualCode ?? person.CurrentCode ?? "";
            IsCodeAutomatic = person.IsCodeAutomatic;

            UpdateLegalTypePanels();
            UpdatePersonTypeToggleSelection();
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
            NationalCode = "";
            CompanyName = "";
            EconomicCode = "";
            RegistrationNumber = "";
            ManualCode = "";
            IsCodeAutomatic = true;
            OnPropertyChanged(nameof(IsUniqueCodeManual));
            IsLegal = false;
            IsActive = true;
            CreditLimit = 0;

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

        //private void CategoryToggle_Checked(object sender, RoutedEventArgs e)
        //{
        //    if (PersonnelToggle == null || TabTax == null) return;

        //    bool isPersonnel = PersonnelToggle.IsChecked == true;
        //    TabTax.Visibility = isPersonnel ? Visibility.Visible : Visibility.Collapsed;

        //    if (!isPersonnel && TabTax.IsChecked == true)
        //        TabPricing.IsChecked = true;
        //}

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
        private string _bankName = "";
        private string _branchName = "";
        private string _cardNumber = "";
        private string _shaba = "";
        private string _accountNumber = "";
        private bool _isDefault;

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
