using _0_Framework.Application;
using BankManagement.Application.Contracts.BankBranch;
using CodeManagement.Application.Contracts.Code;
using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Application.Contract.City;
using GeneralInfoManagement.Application.Contract.Picture;
using GeneralInfoManagement.Application.Contract.Province;
using GeneralInfoManagement.Domain.General.CityAgg;
using GeneralInfoManagement.Domain.General.ProvinceAgg;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Helpers;
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
using System.IO;

using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Taadol.Controls;
namespace Taadol.Views
{
    /// <summary>
    /// فرم ثبت شخص جدید.
    /// همه‌ی فیلدها به Property هایی با INotifyPropertyChanged بایند می‌شوند
    /// و در زمان ذخیره، تمام اطلاعات شخص + تماس‌ها + آدرس + حساب‌های بانکی
    /// از طریق Application های مربوطه ذخیره می‌شود.
    ///
    /// ★ محدودیت‌های بک‌اند (نسخه فعلی):
    ///   - CreatePerson فیلد IsActive نداره.
    ///     → برای تنظیم IsActive از IPersonApplication.Activate(id) / Deactivate(id) استفاده می‌کنیم.
    ///   - OperationResult.Id برنمی‌گرده.
    ///     → بعد از Create، با Search(NationalCode/EconomicCode) شخص جدید رو پیدا می‌کنیم.
    /// </summary>
    public partial class NewPersonView : UserControl, INotifyPropertyChanged, IUnsavedChangesAware
    {
        // ===== CancellationToken =====
        private CancellationTokenSource _loadCts = new();
        private CancellationTokenSource _saveCts = new();

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
        private readonly IPictureApplication _pictureApplication;
        private string? _selectedImagePath;
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
        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(); MarkUserChange(); } }
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(); MarkUserChange(); } }

        /// <summary>نام فرد رابط (فقط برای شخص حقوقی) — فردی که از طرف شرکت با ما در ارتباط است</summary>
        public string ContactFirstName { get => _contactFirstName; set { _contactFirstName = value; OnPropertyChanged(); MarkUserChange(); } }

        /// <summary>نام خانوادگی فرد رابط (فقط برای شخص حقوقی)</summary>
        public string ContactLastName { get => _contactLastName; set { _contactLastName = value; OnPropertyChanged(); MarkUserChange(); } }

        public string NationalCode { get => _nationalCode; set { _nationalCode = value; OnPropertyChanged(); MarkUserChange(); } }
        public string CompanyName { get => _companyName; set { _companyName = value; OnPropertyChanged(); MarkUserChange(); } }
        public string EconomicCode { get => _economicCode; set { _economicCode = value; OnPropertyChanged(); MarkUserChange(); } }
        public string RegistrationNumber { get => _registrationNumber; set { _registrationNumber = value; OnPropertyChanged(); MarkUserChange(); } }
        public string ManualCode { get => _manualCode; set { _manualCode = value; OnPropertyChanged(); MarkUserChange(); } }

        public bool IsCodeAutomatic
        {
            get => _isCodeAutomatic;
            set { _isCodeAutomatic = value; OnPropertyChanged(); MarkUserChange(); }
        }

        /// <summary>
        /// وقتی true باشه، TextBox شناسه یکتا قابل ویرایش است (حالت دستی).
        /// وقتی false باشه، TextBox غیرفعال و مقدارش از CodeGeneratorService میاد.
        /// </summary>
        public bool IsUniqueCodeManual { get => !_isCodeAutomatic; }

        public bool IsLegal
        {
            get => _isLegal;
            set { _isLegal = value; OnPropertyChanged(); MarkUserChange(); UpdateLegalTypePanels(); }
        }

        public bool IsActive { get => _isActive; set { _isActive = value; OnPropertyChanged(); MarkUserChange(); } }

        public long SelectedBranchId
        {
            get => _selectedBranchId;
            set { _selectedBranchId = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public long SelectedPersonTypeId
        {
            get => _selectedPersonTypeId;
            set
            {
                if (_selectedPersonTypeId == value) return;
                _selectedPersonTypeId = value;
                OnPropertyChanged();
                MarkUserChange();
                _ = LoadCategoriesSafeAsync(value);
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

            if (CategorySearch3 != null)
                CategorySearch3.PersonTypeId = personTypeId;

            var token = _loadCts?.Token ?? CancellationToken.None;
            var tree = await PersonFormHelper.LoadCategoryTreeAsync(
                personTypeId,
                token,
                onError: ex => System.Diagnostics.Debug.WriteLine($"Categories load failed: {ex.Message}"),
                onCancelled: () => System.Diagnostics.Debug.WriteLine("[NewPersonView] LoadCategoriesAsync was cancelled")
            );

            if (tree == null) return;

            CategorySearch?.LoadFromTreeDto(tree);
            CategorySearch?.ClearSelection();

            // دپارتمان و عنوان شغل هم فعلاً همان درخت را نشان می‌دهند؛
            // اگر روزی درخت جدا (PersonTypeId متفاوت) لازم شد، GetTree جدا صدا زده شود.
            CategorySearch2?.LoadFromTreeDto(tree);
            CategorySearch2?.ClearSelection();

            CategorySearch3?.LoadFromTreeDto(tree);
            CategorySearch3?.ClearSelection();
        }
        public decimal CreditLimit { get => _creditLimit; set { _creditLimit = value; OnPropertyChanged(); MarkUserChange(); } }

        // Contact
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(); MarkUserChange(); } }
        public string Mobile { get => _mobile; set { _mobile = value; OnPropertyChanged(); MarkUserChange(); } }
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); MarkUserChange(); } }
        public string PostalCode { get => _postalCode; set { _postalCode = value; OnPropertyChanged(); MarkUserChange(); } }
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); MarkUserChange(); } }

        // Address
        public long SelectedProvinceId
        {
            get => _selectedProvinceId;
            set
            {
                _selectedProvinceId = value;
                OnPropertyChanged();
                MarkUserChange();
                _ = LoadCitiesSafeAsync(value);
                UpdateCityState();
            }
        }

        /// <summary>فیلد شهرستان را تا انتخاب استان بلاک می‌کند و در صورت تلاش، خطا نشان می‌دهد.</summary>
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

        private void CityCombo_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (SelectedProvinceId <= 0)
            {
                if (CityErrorText != null)
                    CityErrorText.Visibility = Visibility.Visible;
                e.Handled = true;
            }
        }

        private void CityCombo_DropDownOpened(object sender, EventArgs e)
        {
            if (SelectedProvinceId <= 0)
            {
                CityCombo.IsDropDownOpen = false;
                if (CityErrorText != null)
                    CityErrorText.Visibility = Visibility.Visible;
            }
        }

        public long SelectedCityId
        {
            get => _selectedCityId;
            set { _selectedCityId = value; OnPropertyChanged(); MarkUserChange(); }
        }

        // Main bank
        public string MainBankName { get => _mainBankName; set { _mainBankName = value; OnPropertyChanged(); MarkUserChange(); } }
        public string MainBranchName { get => _mainBranchName; set { _mainBranchName = value; OnPropertyChanged(); MarkUserChange(); } }
        public string MainCardNumber { get => _mainCardNumber; set { _mainCardNumber = value; OnPropertyChanged(); MarkUserChange(); } }
        public string MainShaba { get => _mainShaba; set { _mainShaba = value; OnPropertyChanged(); MarkUserChange(); } }
        public string MainAccountNumber { get => _mainAccountNumber; set { _mainAccountNumber = value; OnPropertyChanged(); MarkUserChange(); } }
        public bool MainBankIsDefault { get => _mainBankIsDefault; set { _mainBankIsDefault = value; OnPropertyChanged(); MarkUserChange(); } }
        public long SelectedBankBranchId { get => _selectedBankBranchId; set { _selectedBankBranchId = value; OnPropertyChanged(); MarkUserChange(); } }

        /// <summary>
        /// الپس‌های دو سر جداکننده باید با پس‌زمینه‌ی پشت فرم هم‌رنگ باشند:
        /// وقتی فرم از لیست اشخاص به‌صورت مودال باز می‌شود → رنگ overlay تیره (#66000000)
        /// و وقتی از سایدبار به‌صورت عادی باز می‌شود → رنگ پس‌زمینه‌ی صفحه (#FFF8ED).
        /// </summary>
        private void UpdateDividerEllipseColor()
        {
            bool isModal = Window.GetWindow(this) is MainWindow mw && mw.ModalContent.Content == this;
            var colorHex = isModal ? "#66000000" : "#FFF8ED";
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex));
            DividerEllipseLeft.Fill = brush;
            DividerEllipseRight.Fill = brush;
        }

        // ======================================================
        //  Constructor
        // ======================================================
        public NewPersonView()
        {
            InitializeComponent();

            // رویدادهای جدول حساب‌های بانکی (کنترل مشترک) — در کد وصل می‌شوند چون delegate سفارشی دارد
            BankAccountsTable.EditRequested += BankAccountsTable_EditRequested;
            BankAccountsTable.RemoveRequested += BankAccountsTable_RemoveRequested;

            // شهرستان تا انتخاب استان غیرفعال است
            UpdateCityState();
            CityCombo.DropDownOpened += CityCombo_DropDownOpened;
            CityCombo.PreviewKeyDown += CityCombo_PreviewKeyDown;

            // رنگ الپس‌های جداکننده به حالت میزبانی بستگی دارد؛
            // تا زمانی که فرم در visual tree نیست (و window/مودال مشخص نیست) نمی‌توان تشخیص داد،
            // پس در Loaded ست می‌شود.
            Loaded += (s, e) => UpdateDividerEllipseColor();

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
            _pictureApplication = App.ServiceProvider.GetRequiredService<IPictureApplication>();
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
            SavePersonCommand = new RelayCommand(async () => await SavePersonAsync());
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
            this.Unloaded += OnViewUnloaded;
            CategorySearch.CategorySelected += OnCategorySelected;

            // تغییر حساب‌های بانکی (افزودن/حذف) هم «تغییر کاربر» محسوب می‌شود
            BankAccounts.CollectionChanged += (_, _) => MarkUserChange();

            ShabaInput.Text = "IR";
            DependencyPropertyDescriptor
                .FromProperty(TextBox.TextProperty, typeof(TextBox))
                .AddValueChanged(ShabaInput.PART_TextBox, (s, ev) => ShabaInput_TextChanged());
            DependencyPropertyDescriptor
                .FromProperty(TextBox.TextProperty, typeof(TextBox))
                .AddValueChanged(CardNumberInput.PART_TextBox, (s, ev) => CardNumberInput_TextChanged());
        }
        private void OnCategorySelected(CategorySearchControl.CategoryItem category)
        {
            _selectedPersonCategoryId = category.Id;
            MarkUserChange();
        }

        /// <summary>لغو عملیات‌های در حال اجرا هنگام بسته شدن فرم</summary>
        private void OnViewUnloaded(object sender, RoutedEventArgs e)
        {
            _loadCts?.Cancel();
            _saveCts?.Cancel();
            _loadCts?.Dispose();
            _saveCts?.Dispose();
            _loadCts = null;
            _saveCts = null;
            this.Unloaded -= OnViewUnloaded;
        }

        // ======================================================
        //  Async Loaders
        // ======================================================
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Branches.Count > 0) return;
            if (_loadCts?.IsCancellationRequested == true) return;

            try
            {
                await Task.WhenAll(
                    LoadBranchesAsync(),
                    LoadPersonTypesAsync(),
                    LoadContactTypesAsync(),
                    LoadProvincesAsync(),
                    LoadBankBranchesAsync()
                );
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[NewPersonView] OnLoaded was cancelled");
            }
            catch (Exception ex)
            {
                if (_loadCts?.IsCancellationRequested != true)
                    ToastManager.Error("خطا در بارگذاری اطلاعات: " + ex.Message);
            }
            finally
            {
                if (_loadCts?.IsCancellationRequested != true)
                    _isLoading = false;
            }
        }

        private async Task LoadBranchesAsync()
        {
            ShowBranchComboLoading(true);
            try
            {
                var items = await PersonFormHelper.LoadBranchesAsync(
                    replaceAll: list => Branches.ReplaceAll(list),
                    onError: ex => ToastManager.Error("خطا در لود شعبه‌ها: " + ex.Message));

                if (Branches.Count > 0 && SelectedBranchId == 0)
                    SelectedBranchId = Branches[0].Id;
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
            var items = await PersonFormHelper.LoadPersonTypesAsync(
                PersonTypes,
                ex => ToastManager.Error("خطا در لود انواع شخص: " + ex.Message));

            if (PersonTypes.Count > 0 && SelectedPersonTypeId == 0)
            {
                SelectedPersonTypeId = PersonTypes[0].Id;
                UpdatePersonTypeToggleSelection();
            }

            // رفع باگ ترتیب ساخت: تاگل «مشتری» در XAML از اول IsChecked="True" دارد،
            // پس رویداد Checked در حین InitializeComponent قبل از ساخته‌شدن
            // CategorySearch فایر می‌شود و PersonTypeId روی کنترل اعمال نمی‌شود.
            // اینجا صریحاً اعمال مجدد می‌کنیم تا «افزودن دسته» بدون جابه‌جایی بین
            // انواع شخص کار کند.
            if (SelectedPersonTypeId > 0)
                await LoadCategoriesAsync(SelectedPersonTypeId);
        }

        private async Task LoadContactTypesAsync()
        {
            await PersonFormHelper.LoadContactTypesAsync(
                _contactTypes,
                _contactTypeByName,
                ex => ToastManager.Error("خطا در لود انواع تماس: " + ex.Message));
        }

        private async Task LoadProvincesAsync()
        {
            await PersonFormHelper.LoadProvincesAsync(
                Provinces,
                ex => ToastManager.Error("خطا در لود استان‌ها: " + ex.Message));
        }

        private async Task LoadCitiesAsync(long provinceId)
        {
            await PersonFormHelper.LoadCitiesAsync(
                cities: Cities,
                provinceId: provinceId,
                token: _loadCts?.Token ?? CancellationToken.None,
                getCurrentProvinceId: () => SelectedProvinceId,
                getCurrentCityId: () => SelectedCityId,
                setSelectedCityId: id => SelectedCityId = id,
                formName: "NewPersonView",
                onError: ex => ToastManager.Error("خطا در لود شهرها: " + ex.Message));
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
                System.Diagnostics.Debug.WriteLine($"[NewPersonView] Error in LoadCategoriesSafeAsync: {ex}");
            }
        }

        /// <summary>Safe wrapper for LoadCitiesAsync with error handling at call site.</summary>
        private async Task LoadCitiesSafeAsync(long provinceId)
        {
            try
            {
                await LoadCitiesAsync(provinceId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewPersonView] Error in LoadCitiesSafeAsync: {ex}");
            }
        }

        private async Task LoadBankBranchesAsync()
        {
            await PersonFormHelper.LoadBankBranchesAsync(
                bankBranches: BankBranches,
                hasBankBranchApp: _bankBranchApplication != null,
                onError: ex => System.Diagnostics.Debug.WriteLine("BankBranches load failed: " + ex.Message),
                onSkipped: msg => System.Diagnostics.Debug.WriteLine("⚠️ " + msg));
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
            var tb = sender as ToggleButton;
            if (tb == null) return;

            if (tb.IsChecked == true && tb.Tag != null && long.TryParse(tb.Tag.ToString(), out var id))
            {
                SelectedPersonTypeId = id;
                UncheckOtherPersonTypeToggles(tb);
            }

            UpdatePersonnelTabVisibility();
        }

        private void PersonTypeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            var tb = sender as ToggleButton;
            if (tb == null) return;

            // Check if any other toggle is still checked
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

            // If no toggle is checked, re-check the last one
            if (!anyChecked)
            {
                tb.IsChecked = true;
            }
        }

        private void UpdatePersonnelTabVisibility()
        {
            if (TabTax == null) return;

            bool isPersonnel = PersonTypePersonnel?.IsChecked == true;
            TabTax.Visibility = isPersonnel ? Visibility.Visible : Visibility.Collapsed;

            if (!isPersonnel && TabTax.IsChecked == true)
                TabPricing.IsChecked = true;
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
        // Note: async void here is safe because:
        // 1. try/catch wraps the entire body
        // 2. _isSaving guard prevents reentrancy
        // 3. SavePersonAsync handles all error paths
        private async void SavePerson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await SavePersonAsync();
            }
            catch (Exception ex)
            {
                ToastManager.Error("خطا در ثبت شخص: " + ex.Message);
            }
        }

        /// <summary>بعد از ذخیره‌ی موفق یک شخص صدا زده می‌شود تا گرید پشت مودال رفرش شود.</summary>
        public event Action PersonSaved;

        private bool _isSaving;

        // ===== ردیابی تغییرات کاربر (برای انصراف/بستن با اخطار) =====
        // تا وقتی فرم در حال مقداردهی اولیه/لود است، تغییراتِ برنامه‌ای نادیده گرفته می‌شوند
        private bool _isLoading = true;
        private bool _userMadeChanges;

        public bool HasUnsavedChanges => _userMadeChanges;

        private void MarkUserChange()
        {
            if (_isLoading) return;
            _userMadeChanges = true;
        }

        private async Task SavePersonAsync()
        {
            if (_isSaving) return;
            if (_saveCts?.IsCancellationRequested == true) return;
            _isSaving = true;

            try
            {
                if (SaveButton != null) SaveButton.IsEnabled = false;

                // 1) اعتبارسنجی
                if (!ValidatePerson()) return;
                _saveCts.Token.ThrowIfCancellationRequested();

                // 2) تأیید کاربر
                var dialog = new CustomConfirmDialog();
                if (dialog.ShowDialog() != true) return;

                // 3) ذخیره در دیتابیس
                var snapshot = CaptureSaveSnapshot();
                var saveResult = await ExecutePersonSaveAsync(snapshot);

                if (!saveResult.Success)
                {
                    ToastManager.Error(saveResult.Message);
                    return;
                }

                // 4) ذخیره عکس + رفرش UI
                OnSaveSucceeded(saveResult.PersonId);
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[NewPersonView] SavePersonAsync was cancelled");
            }
            catch (Exception ex)
            {
                LogSaveException(ex);
            }
            finally
            {
                if (_saveCts?.IsCancellationRequested != true)
                {
                    _isSaving = false;
                    if (SaveButton != null) SaveButton.IsEnabled = true;
                }
            }
        }

        /// <summary> snapping all form field values for cross-thread save</summary>
        private record SaveSnapshot(
            bool IsLegal, string CompanyName, string FirstName, string LastName,
            string ContactFirstName, string ContactLastName, string NationalCode,
            string EconomicCode, string RegistrationNumber, long PersonTypeId,
            long BranchId, string ManualCode, long? PersonCategoryId, bool IsActive,
            decimal CreditLimit, string Phone, string Mobile, string Email,
            Dictionary<string, long> ContactTypeNames, string Address, string PostalCode,
            long ProvinceId, long CityId, string MainShaba, string MainCardNumber,
            long MainBankBranchId, string MainAccountNumber, bool MainBankIsDefault,
            List<dynamic> BankAccountsSnapshot);

        private SaveSnapshot CaptureSaveSnapshot()
        {
            return new SaveSnapshot(
                IsLegal, CompanyName, FirstName, LastName,
                ContactFirstName, ContactLastName, NationalCode,
                EconomicCode, RegistrationNumber, SelectedPersonTypeId,
                SelectedBranchId, ManualCode, _selectedPersonCategoryId, IsActive,
                CreditLimit, Phone?.Trim() ?? "", Mobile?.Trim() ?? "", Email?.Trim() ?? "",
                new Dictionary<string, long>(_contactTypeByName), Address, PostalCode,
                SelectedProvinceId, SelectedCityId, MainShaba, MainCardNumber,
                SelectedBankBranchId, MainAccountNumber, MainBankIsDefault,
                BankAccounts.Select(r => new { r.BankBranchId, r.BankName, r.CardNumber, r.Shaba, r.AccountNumber, r.IsDefault })
                    .Select(r => (dynamic)r).ToList());
        }

        /// <summary> persists person + contacts + address + banks in a single scoped Task.Run</summary>
        private Task<(bool Success, string Message, long PersonId)> ExecutePersonSaveAsync(SaveSnapshot s)
        {
            return Task.Run(() =>
            {
                using var scope = App.ServiceProvider.CreateScope();
                var sp = scope.ServiceProvider;
                var personApp = sp.GetRequiredService<IPersonApplication>();
                var contactApp = sp.GetRequiredService<IPersonContactApplication>();
                var addressApp = sp.GetRequiredService<IPersonAddressApplication>();
                var bankApp = sp.GetRequiredService<IPersonBankApplication>();

                // --- Create person ---
                var command = new CreatePerson
                {
                    FirstName = s.IsLegal ? s.CompanyName : s.FirstName,
                    LastName = s.IsLegal ? "" : s.LastName,
                    ContactFirstName = s.IsLegal ? (s.ContactFirstName ?? "") : "",
                    ContactLastName = s.IsLegal ? (s.ContactLastName ?? "") : "",
                    NationalCode = s.IsLegal ? null : s.NationalCode,
                    EconomicCode = s.IsLegal ? s.EconomicCode : null,
                    RegistrationNumber = s.IsLegal ? s.RegistrationNumber : null,
                    IsLegal = s.IsLegal,
                    PersonTypeId = s.PersonTypeId,
                    BranchId = s.BranchId,
                    CreditLimit = s.CreditLimit,
                    IsCodeAutomatic = false,
                    ManualCode = s.ManualCode,
                    PersonCategoryId = s.PersonCategoryId
                };

                var personResult = personApp.Create(command);
                if (!personResult.IsSucceeded)
                    return (false, personResult.Message ?? "ثبت شخص ناموفق بود.", 0L);

                long personId = ResolveCreatedPersonId(personApp, command);
                if (personId <= 0)
                    return (false, "شخص ثبت شد ولی پیدا کردن شناسه‌ی او ناموفق بود.", 0L);

                // Activate / Deactivate
                if (s.IsActive) personApp.Activate(personId); else personApp.Deactivate(personId);

                // --- Save contacts ---
                SaveContactsScoped(contactApp, personId, s.ContactTypeNames, s.Phone, s.Mobile, s.Email);

                // --- Save address ---
                SaveAddressScoped(addressApp, personId, s);

                // --- Save bank accounts ---
                SaveBankAccountsScoped(bankApp, personId, s);

                return (true, "", personId);
            });
        }

        /// <summary> resolves person ID after Create (OperationResult lacks Id property)</summary>
        private static long ResolveCreatedPersonId(IPersonApplication personApp, CreatePerson command)
        {
            var code = command.IsLegal ? command.EconomicCode : command.NationalCode;
            var list = personApp.Search(new PersonSearchModel { NationalCode = code });
            return list?.OrderByDescending(x => x.Id).FirstOrDefault()?.Id ?? 0;
        }

        /// <summary> saves phone / mobile / email contacts</summary>
        private static void SaveContactsScoped(IPersonContactApplication contactApp, long personId,
            Dictionary<string, long> typeNames, string phone, string mobile, string email)
        {
            if (!string.IsNullOrWhiteSpace(phone) && typeNames.TryGetValue("تلفن ثابت", out var phoneTypeId))
                contactApp.Create(new CreatePersonContact { PersonId = personId, ContactTypeId = phoneTypeId, Value = phone, Description = "", IsDefault = false });
            if (!string.IsNullOrWhiteSpace(mobile) && typeNames.TryGetValue("موبایل", out var mobileTypeId))
                contactApp.Create(new CreatePersonContact { PersonId = personId, ContactTypeId = mobileTypeId, Value = mobile, Description = "", IsDefault = true });
            if (!string.IsNullOrWhiteSpace(email) && typeNames.TryGetValue("ایمیل", out var emailTypeId))
                contactApp.Create(new CreatePersonContact { PersonId = personId, ContactTypeId = emailTypeId, Value = email, Description = "", IsDefault = false });
        }

        /// <summary> saves main address</summary>
        private static void SaveAddressScoped(IPersonAddressApplication addressApp, long personId, SaveSnapshot s)
        {
            if (string.IsNullOrWhiteSpace(s.Address) && s.ProvinceId <= 0 && s.CityId <= 0) return;
            if (s.ProvinceId <= 0 || s.CityId <= 0) return;
            addressApp.Create(new CreatePersonAddress
            {
                PersonId = personId,
                Title = "آدرس اصلی",
                Address = s.Address ?? "",
                PostalCode = s.PostalCode ?? "",
                ProvinceId = s.ProvinceId,
                CityId = s.CityId,
                IsDefault = true
            });
        }

        /// <summary> saves main + grid bank accounts</summary>
        private static void SaveBankAccountsScoped(IPersonBankApplication bankApp, long personId, SaveSnapshot s)
        {
            // Main account
            if ((!string.IsNullOrWhiteSpace(s.MainShaba) || !string.IsNullOrWhiteSpace(s.MainCardNumber)) && s.MainBankBranchId > 0)
            {
                bankApp.Create(new CreatePersonBank
                {
                    PersonId = personId, BankBranchId = s.MainBankBranchId,
                    AccountNumber = s.MainAccountNumber ?? "", CardNumber = s.MainCardNumber ?? "",
                    Shaba = s.MainShaba ?? "", IsDefault = s.MainBankIsDefault
                });
            }
            // Grid accounts
            foreach (var row in s.BankAccountsSnapshot)
            {
                if (row.Shaba == null && row.CardNumber == null) continue;
                if (row.BankBranchId <= 0) continue;
                bankApp.Create(new CreatePersonBank
                {
                    PersonId = personId, BankBranchId = row.BankBranchId,
                    AccountNumber = row.AccountNumber ?? "", CardNumber = row.CardNumber ?? "",
                    Shaba = row.Shaba ?? "", IsDefault = row.IsDefault
                });
            }
        }

        /// <summary> post-save: picture + toast + navigate</summary>
        private void OnSaveSucceeded(long personId)
        {
            SavePersonPicture(personId);
            ToastManager.Success("ثبت شخص با موفقیت انجام شد.");
            var mainWindow = Window.GetWindow(this) as MainWindow;
            bool isModal = mainWindow?.ModalContent.Content == this;
            if (isModal) { PersonSaved?.Invoke(); mainWindow?.CloseModal(); }
            else mainWindow?.NavigateTo("person_list");
        }

        /// <summary> logs exception to debug + file</summary>
        private void LogSaveException(Exception ex)
        {
            var fullMessage = BuildFullExceptionMessage(ex);
            if (_saveCts?.IsCancellationRequested != true)
                ToastManager.Error("خطا در ثبت شخص: " + ex.Message);
            try { System.IO.File.AppendAllText(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "taadol-person-save-error.log"), $"[{DateTime.Now:yyyy/MM/dd HH:mm:ss}]{Environment.NewLine}{fullMessage}{Environment.NewLine}{new string('-', 80)}{Environment.NewLine}"); } catch { }
        }
        /// <summary>
        /// ★ ذخیره عکس شخص.
        /// فایل انتخاب‌شده توسط کاربر در پوشه‌ی Pictures/Persons کنار فایل اجرایی کپی می‌شه
        /// و مسیر نسبی اون در جدول Pictures با OwnerType=Person ذخیره می‌شه.
        /// </summary>
        private void SavePersonPicture(long personId)
        {
            // اگه کاربر عکسی انتخاب نکرده، رد شو
            if (string.IsNullOrWhiteSpace(_selectedImagePath) || !File.Exists(_selectedImagePath))
            {
                System.Diagnostics.Debug.WriteLine("📷 SavePersonPicture: No image selected, skipping.");
                return;
            }

            try
            {
                // ساخت پوشه‌ی مقصد در کنار فایل اجرایی
                var appDir = AppDomain.CurrentDomain.BaseDirectory;
                var picturesDir = Path.Combine(appDir, "Pictures", "Persons");
                Directory.CreateDirectory(picturesDir);

                // ساخت نام فایل یکتا با timestamp + personId
                var ext = Path.GetExtension(_selectedImagePath); // مثلاً ".jpg"
                if (string.IsNullOrWhiteSpace(ext)) ext = ".jpg";
                var fileName = $"person_{personId}_{DateTime.Now:yyyyMMdd_HHmmss}{ext}";
                var destPath = Path.Combine(picturesDir, fileName);

                // کپی فایل (حتی اگه مبدا و مقصد یکی باشن، با overwrite=true)
                File.Copy(_selectedImagePath, destPath, overwrite: true);

                // مسیر نسبی برای ذخیره در DB
                var relativeUrl = $"Pictures/Persons/{fileName}";

                // ثبت در جدول Pictures
                // ✅ بک‌اند آپدیت شده و Person = 2 به PictureOwnerTypeDTO اضافه شده
                var result = _pictureApplication.Create(new CreatePicture
                {
                    OwnerId = personId,
                    OwnerType = PictureOwnerTypeDTO.Person,
                    Url = relativeUrl
                });

                if (!result.IsSucceeded)
                {
                    System.Diagnostics.Debug.WriteLine($"📷 SavePersonPicture: Failed to save picture record — {result.Message}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"📷 SavePersonPicture: Picture saved successfully — Url={relativeUrl}");
                }

                // پاک کردن مسیر بعد از ثبت
                _selectedImagePath = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"📷 SavePersonPicture ERROR: {ex.Message}");
                // عکس optional هست، نباید کل عملیات ثبت رو لغو کنه
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
            if (!ValidateRequiredSelections()) return false;
            if (!ValidateMobileField()) return false;
            if (!ValidatePhoneField()) return false;
            if (IsLegal) { if (!ValidateLegalFields()) return false; }
            else { if (!ValidateNaturalPersonFields()) return false; }
            if (!ValidateUniqueCode()) return false;
            if (!ValidateContactFormats()) return false;
            return true;
        }

        /// <summary> validates BranchId and PersonTypeId are selected</summary>
        private bool ValidateRequiredSelections()
        {
            if (SelectedBranchId <= 0) { ToastManager.Warning("لطفاً شعبه را انتخاب کنید."); return false; }
            if (SelectedPersonTypeId <= 0) { ToastManager.Warning("لطفاً نوع شخص را انتخاب کنید."); return false; }
            return true;
        }

        /// <summary> validates mobile format and updates UI indicator</summary>
        private bool ValidateMobileField()
        {
            if (string.IsNullOrWhiteSpace(Mobile)) { MobileInput.ValidationState = Controls.ValidationState.None; MobileInput.ValidationMessage = ""; return true; }
            if (!ValidationHelper.IsValidMobile(Mobile)) { MobileInput.ValidationState = Controls.ValidationState.Invalid; MobileInput.ValidationMessage = "شماره موبایل باید ۱۱ رقم و با 09 شروع شود."; return false; }
            MobileInput.ValidationState = Controls.ValidationState.Valid; MobileInput.ValidationMessage = ""; return true;
        }

        /// <summary> validates phone format and updates UI indicator</summary>
        private bool ValidatePhoneField()
        {
            if (string.IsNullOrWhiteSpace(Phone)) { PhoneInput.ValidationState = Controls.ValidationState.None; PhoneInput.ValidationMessage = ""; return true; }
            if (!ValidationHelper.IsValidPhone(Phone)) { PhoneInput.ValidationState = Controls.ValidationState.Invalid; PhoneInput.ValidationMessage = "شماره تلفن باید ۸ تا ۱۱ رقم باشد."; return false; }
            PhoneInput.ValidationState = Controls.ValidationState.Valid; PhoneInput.ValidationMessage = ""; return true;
        }

        /// <summary> validates legal person required fields: CompanyName, EconomicCode, ContactFirstName/LastName</summary>
        private bool ValidateLegalFields()
        {
            if (string.IsNullOrWhiteSpace(CompanyName)) { ToastManager.Warning("نام شرکت را وارد کنید."); return false; }
            if (string.IsNullOrWhiteSpace(EconomicCode)) { ToastManager.Warning("کد اقتصادی را وارد کنید."); return false; }
            if (string.IsNullOrWhiteSpace(ContactFirstName)) { ToastManager.Warning("نام فرد رابط را وارد کنید."); return false; }
            if (string.IsNullOrWhiteSpace(ContactLastName)) { ToastManager.Warning("نام خانوادگی فرد رابط را وارد کنید."); return false; }
            return true;
        }

        /// <summary> validates natural person required fields: FirstName, LastName, NationalCode</summary>
        private bool ValidateNaturalPersonFields()
        {
            bool hasError = false;
            SetFieldValidation(FirstNameInput, string.IsNullOrWhiteSpace(FirstName), "نام را وارد کنید.", ref hasError);
            SetFieldValidation(LastNameInput, string.IsNullOrWhiteSpace(LastName), "نام خانوادگی را وارد کنید.", ref hasError);
            // NationalCode: empty → error; wrong length → error; invalid checksum → error; else valid
            if (string.IsNullOrWhiteSpace(NationalCode))
                SetNationalCodeValidation(Controls.ValidationState.Invalid, "کد ملی را وارد کنید.", ref hasError);
            else if (NationalCode.Count(char.IsDigit) != 10)
                SetNationalCodeValidation(Controls.ValidationState.Invalid, "کد ملی باید دقیقاً ۱۰ رقم باشد.", ref hasError);
            else if (!ValidationHelper.IsValidNationalCode(NationalCode))
                SetNationalCodeValidation(Controls.ValidationState.Invalid, "کد ملی وارد شده صحیح نیست.", ref hasError);
            else
                SetNationalCodeValidation(Controls.ValidationState.Valid, "", ref hasError);
            return !hasError;
        }

        private void SetFieldValidation(Controls.ModernPersianTextBox control, bool isInvalid, string msg, ref bool hasError)
        {
            if (isInvalid) { control.ValidationState = Controls.ValidationState.Invalid; control.ValidationMessage = msg; hasError = true; }
            else { control.ValidationState = Controls.ValidationState.Valid; control.ValidationMessage = ""; }
        }

        private void SetNationalCodeValidation(Controls.ValidationState state, string msg, ref bool hasError)
        {
            NationalCodeInput.ValidationState = state; NationalCodeInput.ValidationMessage = msg;
            if (state == Controls.ValidationState.Invalid) hasError = true;
        }

        /// <summary> validates unique code (auto-generated or manual)</summary>
        private bool ValidateUniqueCode()
        {
            if (!IsCodeAutomatic && string.IsNullOrWhiteSpace(ManualCode))
            { ToastManager.Warning("شناسه یکتای دستی را وارد کنید یا حالت اتوماتیک را فعال کنید."); return false; }
            if (IsCodeAutomatic && string.IsNullOrWhiteSpace(ManualCode))
            {
                ManualCode = GenerateNextUniqueCode();
                if (string.IsNullOrWhiteSpace(ManualCode))
                { ToastManager.Error("تولید شناسه یکتای اتوماتیک ناموفق بود. لطفاً حالت دستی را انتخاب کرده و کد را وارد کنید."); return false; }
            }
            return true;
        }

        /// <summary> validates email and shaba formats</summary>
        private bool ValidateContactFormats()
        {
            if (!string.IsNullOrWhiteSpace(Email) && !ValidationHelper.IsValidEmail(Email))
            { ToastManager.Warning("فرمت ایمیل صحیح نیست. مثال صحیح: name@example.com"); return false; }
            if (!string.IsNullOrWhiteSpace(MainShaba) && !ValidationHelper.IsValidShaba(MainShaba))
            { ToastManager.Warning("فرمت شبا صحیح نیست. باید با IR شروع و در مجموع ۲۶ کاراکتر باشد."); return false; }
            return true;
        }

        // Validation methods moved to Taadol.Helpers.ValidationHelper

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
                var code = command.IsLegal ? command.EconomicCode : command.NationalCode;
                var search = new PersonSearchModel
                {
                    NationalCode = code
                };
                var list = _personApplication.Search(search);
                var result = list?.OrderByDescending(x => x.Id).FirstOrDefault()?.Id ?? 0;
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"🔍 GetCreatedPersonId EXCEPTION: {ex.Message}");
                return 0;
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
            System.Diagnostics.Debug.WriteLine($"🏦 SaveBanks: personId={personId}, MainShaba='{MainShaba}', MainCardNumber='{MainCardNumber}', SelectedBankBranchId={SelectedBankBranchId}, BankAccounts.Count={BankAccounts.Count}");

            // حساب بانکی اصلی (اگر شماره شبا یا شماره کارت دارد)
            if (!string.IsNullOrWhiteSpace(MainShaba) || !string.IsNullOrWhiteSpace(MainCardNumber))
            {
                System.Diagnostics.Debug.WriteLine($"🏦 SaveBanks: Saving main account — BankBranchId={SelectedBankBranchId}");
                TryCreateBankAccount(personId, SelectedBankBranchId, MainBankName, MainAccountNumber, MainCardNumber, MainShaba, MainBankIsDefault);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"🏦 SaveBanks: Main account skipped — MainShaba and MainCardNumber both empty");
            }

            // حساب‌های اضافه‌شده — هر ردیف BankBranchId خودش رو داره
            int idx = 0;
            foreach (var row in BankAccounts)
            {
                idx++;
                if (string.IsNullOrWhiteSpace(row.Shaba) && string.IsNullOrWhiteSpace(row.CardNumber))
                {
                    System.Diagnostics.Debug.WriteLine($"🏦 SaveBanks: Row #{idx} skipped — Shaba and CardNumber both empty");
                    continue;
                }
                System.Diagnostics.Debug.WriteLine($"🏦 SaveBanks: Saving row #{idx} — BankBranchId={row.BankBranchId}, BankName='{row.BankName}', Shaba='{row.Shaba}', CardNumber='{row.CardNumber}'");
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
                System.Diagnostics.Debug.WriteLine($"🏦 TryCreateBankAccount: PersonId={personId}, BankBranchId={bankBranchId}, BankName='{bankName}', Shaba='{shaba}', CardNumber='{cardNumber}'");
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
                    System.Diagnostics.Debug.WriteLine($"❌ Bank save FAILED: {result.Message}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Bank save SUCCEEDED: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Bank save EXCEPTION: {ex.Message}");
            }
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
            _selectedImagePath = null;
            if (PersonImagePicker != null)
                PersonImagePicker.ImagePath = null;
            MainBankName = "";
            MainBranchName = "";
            MainCardNumber = "";
            MainShaba = "";
            MainAccountNumber = "";
            MainBankIsDefault = true;
            SelectedBankBranchId = 0;

            BankAccounts.Clear();

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

        /// <summary>
        /// بستن فرم: اگر در مودال است (از لیست اشخاص) فقط مودال بسته می‌شود؛
        /// اگر از سایدبار به‌صورت عادی باز شده، فرم بسته و زیرمنوی سایدبار پاک می‌شود.
        /// </summary>
        private void CloseFormOrModal()
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow == null) return;

            if (mainWindow.ModalContent.Content == this)
                mainWindow.CloseModal();
            else
                mainWindow.CloseCurrentForm();
        }

        private async void Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (HasUnsavedChanges)
                {
                    var result = MessageBox.Show(
                        "تغییراتی که ایجاد کرده‌اید ذخیره نشده است.\nآیا می‌خواهید آن‌ها را ذخیره کنید؟",
                        "ذخیره تغییرات",
                        MessageBoxButton.YesNoCancel,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // ✅ غیرفعال کردن دکمه‌ها برای جلوگیری از کلیک مجدد
                        if (SaveButton != null) SaveButton.IsEnabled = false;

                        await SavePersonAsync();
                        // SavePersonAsync بعد از موفقیت خودش فرم را می‌بندد
                        return;
                    }
                    if (result == MessageBoxResult.Cancel)
                        return;
                }

                CloseFormOrModal();
            }
            catch (Exception ex)
            {
                ToastManager.Error("خطا در عملیات: " + ex.Message);
            }
        }

        // ======================================================
        //  Bank Accounts (Add/Remove)
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
            if (!MainBankHasData())
            {
                ToastManager.Warning("لطفاً ابتدا اطلاعات حساب بانکی را وارد کنید.");
                return;
            }

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

            ReindexBankAccounts();
            ClearBankForm();
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

        private void BankAccountsTable_RemoveRequested(object sender, BankAccountRow row)
        {
            BankAccounts.Remove(row);
            ReindexBankAccounts();
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
        private bool _isUpdatingShaba;

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
            if (digitIndex <= 0) return 0;
            int seen = 0;
            for (int i = 0; i < formatted.Length; i++)
            {
                if (IsDigitChar(formatted[i]))
                {
                    seen++;
                    if (seen == digitIndex)
                        return i + 1;
                }
            }
            return formatted.Length;
        }
        private bool _isUpdatingCard;

        private void DefaultCheckBox_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is BankAccountRow row)
            {
                row.IsDefault = !row.IsDefault;

                if (row.IsDefault)
                {
                    foreach (var other in BankAccounts.Where(r => r != row && r.IsDefault))
                        other.IsDefault = false;
                }
            }
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

        // ======================================================
        //  Misc UI Handlers
        // ======================================================
        private void OnImageSelected(object sender, RoutedEventArgs e)
        {
            _selectedImagePath = PersonImagePicker?.ImagePath;
            MarkUserChange();
        }

        private void OnImageRemoved(object sender, RoutedEventArgs e)
        {
            _selectedImagePath = null;
            MarkUserChange();
        }

        // ======================================================
        //  Close & Clear
        // ======================================================
        private void CloseButton_Click(object sender, MouseButtonEventArgs e)
        {
            CloseFormOrModal();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "آیا از پاک کردن فرم مطمئن هستید؟",
                "پاک کردن",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            if (result != MessageBoxResult.Yes) return;

            FirstName = "";
            LastName = "";
            NationalCode = "";
            Phone = "";
            Mobile = "";
            Email = "";
            Address = "";
            PostalCode = "";
            CompanyName = "";
            EconomicCode = "";
            RegistrationNumber = "";
            ContactFirstName = "";
            ContactLastName = "";
            ManualCode = "";
            IsActive = true;
            _selectedImagePath = null;
            PersonImagePicker.ImagePath = null;
            CategorySearch?.ClearSelection();
            BankAccounts?.Clear();
            MainBankName = "";
            MainCardNumber = "";
            MainShaba = "";
            MainAccountNumber = "";
            MainBankIsDefault = false;
        }

        // ======================================================
        //  INotifyPropertyChanged
        // ======================================================
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void ValidationField_TextChanged(object sender, RoutedEventArgs e)
        {
            if (sender is Controls.ModernPersianTextBox textBox)
            {
                string text = textBox.Text?.Trim() ?? "";

                if (textBox == NationalCodeInput)
                {
                    if (string.IsNullOrWhiteSpace(text) || text.Length < 10)
                    {
                        // هنوز کامل نشده — آیکونی نشان نده
                        textBox.ValidationState = Controls.ValidationState.None;
                        textBox.ValidationMessage = "";
                    }
                    else if (ValidationHelper.IsValidNationalCode(text))
                    {
                        textBox.ValidationState = Controls.ValidationState.Valid;
                        textBox.ValidationMessage = "";
                    }
                    else
                    {
                        textBox.ValidationState = Controls.ValidationState.Invalid;
                        textBox.ValidationMessage = "کد ملی وارد شده صحیح نیست.";
                    }
                }
                else if (textBox == PhoneInput)
                {
                    string digits = new string(text.Where(char.IsDigit).ToArray());
                    if (digits.Length == 8 || digits.Length == 11)
                    {
                        textBox.ValidationState = Controls.ValidationState.Valid;
                        textBox.ValidationMessage = "";
                    }
                    else
                    {
                        textBox.ValidationState = Controls.ValidationState.None;
                        textBox.ValidationMessage = "";
                    }
                }
                else if (textBox == MobileInput)
                {
                    string digits = new string(text.Where(char.IsDigit).ToArray());
                    if (digits.Length == 11)
                    {
                        textBox.ValidationState = Controls.ValidationState.Valid;
                        textBox.ValidationMessage = "";
                    }
                    else
                    {
                        textBox.ValidationState = Controls.ValidationState.None;
                        textBox.ValidationMessage = "";
                    }
                }
                else if (textBox == EmailInput)
                {
                    if (!string.IsNullOrWhiteSpace(text) && ValidationHelper.IsValidEmail(text))
                    {
                        textBox.ValidationState = Controls.ValidationState.Valid;
                        textBox.ValidationMessage = "";
                    }
                    else
                    {
                        textBox.ValidationState = Controls.ValidationState.None;
                        textBox.ValidationMessage = "";
                    }
                }
                else if (textBox == FirstNameInput || textBox == LastNameInput)
                {
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        textBox.ValidationState = Controls.ValidationState.Valid;
                        textBox.ValidationMessage = "";
                    }
                    else
                    {
                        textBox.ValidationState = Controls.ValidationState.None;
                        textBox.ValidationMessage = "";
                    }
                }
                else if (textBox == ManualCodeTextBox)
                {
                    if (!string.IsNullOrWhiteSpace(text) && text.All(c => char.IsLetterOrDigit(c) && c <= 127))
                    {
                        textBox.ValidationState = Controls.ValidationState.Valid;
                        textBox.ValidationMessage = "";
                    }
                    else
                    {
                        textBox.ValidationState = Controls.ValidationState.None;
                        textBox.ValidationMessage = "";
                    }
                }
            }
        }
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
        private int _index;

        public int Index { get => _index; set { _index = value; OnPC(); } }

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

    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is int count && count > 0 ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    // نکته: BranchComboItem قبلاً در NewFinancialPeriodView.xaml.cs تعریف شده و در اینجا reuse می‌شود.
}
