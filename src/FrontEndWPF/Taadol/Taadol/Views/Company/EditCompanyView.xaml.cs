using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GeneralInfoManagement.Application.Contract.Company;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;

namespace Taadol.Views
{
    /// <summary>
    /// فرم ویرایش شرکت — الگوی EditPersonView:
    /// لود اطلاعات از GetDetails، ذخیره با EditCompanies،
    /// ردیابی تغییرات ذخیره‌نشده (IUnsavedChangesAware) و رفرش لیست پشت مودال.
    /// </summary>
    public partial class EditCompanyView : UserControl, INotifyPropertyChanged, IUnsavedChangesAware
    {
        private readonly ICompanyApplication _companyApplication;
        private readonly long _companyId;

        private string _companyName = "";
        private string _officialName = "";
        private DateTime? _foundingDate;
        private bool _isActive = true;

        // ردیابی تغییرات واقعی کاربر:
        // تا پایان لود (IsLoading) تغییرات برنامه‌نویسی نادیده گرفته می‌شوند؛
        // بعد از آن هر تغییر = تغییر کاربر → انصراف فقط در این صورت سؤال می‌پرسد.
        private bool _isLoading = true;
        private bool _userMadeChanges;
        private bool _isSaving;

        public ICommand SaveCommand { get; }

        public bool HasUnsavedChanges => _userMadeChanges;

        public string CompanyName
        {
            get => _companyName;
            set { _companyName = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public string OfficialName
        {
            get => _officialName;
            set { _officialName = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public DateTime? FoundingDate
        {
            get => _foundingDate;
            set { _foundingDate = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public bool IsActive
        {
            get => _isActive;
            set { _isActive = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public EditCompanyView(long companyId)
        {
            InitializeComponent();

            _companyId = companyId;
            _companyApplication = App.ServiceProvider.GetRequiredService<ICompanyApplication>();

            SaveCommand = new RelayCommand(SaveCompany);
            DataContext = this;

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // جزئیات شرکت و وضعیت فعال به‌صورت موازی لود می‌شوند
                var detailsTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<ICompanyApplication>();
                    return app.GetDetails(_companyId);
                });

                var companiesTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<ICompanyApplication>();
                    return app.GetCompanies();
                });

                var details = await detailsTask;
                if (details == null)
                {
                    ToastManager.Error("شرکت پیدا نشد.");
                    return;
                }

                CompanyName = details.Title ?? "";
                OfficialName = details.LegalName ?? "";

                // GetDetails وضعیت فعال را برنمی‌گرداند؛ از لیست شرکت‌ها می‌خوانیم
                var companyVm = (await companiesTask).FirstOrDefault(c => c.Id == _companyId);
                IsActive = companyVm?.IsActive ?? true;

                if (details.EstablishedDate != default)
                {
                    // هم مقدار property (برای ولیدیشن/ذخیره) هم فیلدهای نمایشی تاریخ
                    FoundingDate = details.EstablishedDate;
                    if (FoundingDatePicker != null)
                        FoundingDatePicker.SelectedDate = details.EstablishedDate;
                }

                if (!string.IsNullOrWhiteSpace(details.Logo))
                {
                    var fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, details.Logo);
                    if (System.IO.File.Exists(fullPath))
                        CompanyImagePicker.ImagePath = fullPath;
                }

                // لود کامل شد — از این به بعد هر تغییری = تغییر کاربر
                _isLoading = false;
            }
            catch (Exception ex)
            {
                _isLoading = false;
                ToastManager.Error("خطا در لود اطلاعات: " + ex.Message);
            }
        }

        private void MarkUserChange()
        {
            if (_isLoading) return;
            _userMadeChanges = true;
        }

        private void DatePicker_DateChanged(object sender, RoutedEventArgs e)
        {
            var picker = sender as PersianDatePickerControl;
            if (picker == null) return;

            // اگر فیلدهای تاریخ پاک شوند مقدار null می‌شود تا ذخیره با تاریخ قبلی رخ ندهد
            FoundingDate = picker.SelectedDate;
        }

        private void OnImageSelected(object sender, RoutedEventArgs e)
        {
            MarkUserChange();
        }

        private void OnImageRemoved(object sender, RoutedEventArgs e)
        {
            MarkUserChange();
        }

        private void SaveCompany()
        {
            if (_isSaving) return;

            // اعتبارسنجی اول — دکمه فقط وقتی وارد حالت «در حال ذخیره» می‌شود که فرم معتبر باشد
            if (string.IsNullOrWhiteSpace(CompanyName))
            {
                ToastManager.Warning("نام شرکت / کسب‌وکار را وارد کنید.");
                return;
            }

            if (string.IsNullOrWhiteSpace(OfficialName))
            {
                ToastManager.Warning("نام رسمی ثبتی را وارد کنید.");
                return;
            }

            if (!FoundingDate.HasValue)
            {
                ToastManager.Warning("تاریخ تاسیس را انتخاب کنید.");
                return;
            }

            if (FoundingDate.Value.Date > DateTime.Today)
            {
                ToastManager.Warning("تاریخ تاسیس نمی‌تواند در آینده باشد.");
                return;
            }

            _isSaving = true;
            if (SaveButton != null)
            {
                SaveButton.IsEnabled = false;
                SaveButton.Text = "در حال ذخیره...";
            }

            try
            {

                var command = new EditCompanies
                {
                    Id = _companyId,
                    Title = CompanyName.Trim(),
                    LegalName = OfficialName.Trim(),
                    EstablishedDate = FoundingDate.Value,
                    Logo = CompanyImagePicker?.ImagePath ?? ""
                };

                var operation = _companyApplication.Edit(command);
                if (!operation.IsSucceeded)
                {
                    ToastManager.Warning(
                        string.IsNullOrWhiteSpace(operation.Message) ? "ویرایش شرکت انجام نشد." : operation.Message);
                    return;
                }

                // هماهنگ‌سازی وضعیت فعال/غیرفعال با لیست
                if (IsActive)
                    _companyApplication.Activate(_companyId);
                else
                    _companyApplication.Deactivate(_companyId);

                ToastManager.Success("ویرایش شرکت با موفقیت انجام شد.");

                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.CloseModal();

                // اگر پشت مودال لیست شرکت‌ها بود همان لیست درجا رفرش می‌شود (بدون از دست رفتن State)
                if (mainWindow?.MainContent.Content is CompanyListView listView)
                    _ = RefreshListViewSafeAsync(listView);
                else
                    mainWindow?.NavigateTo("company_list");
            }
            catch (Exception ex)
            {
                ToastManager.Error("خطا در ویرایش: " + ex.Message);
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

        /// <summary>
        /// اگر تغییرات ذخیره‌نشده وجود داشته باشد، از کاربر می‌پرسد (ذخیره/انصراف/بستن).
        /// خروجی false یعنی بستن ادامه پیدا نکند (کاربر Cancel زده یا انتخاب کرده ذخیره کند).
        /// </summary>
        private bool ConfirmCloseWithUnsavedWarning()
        {
            if (!_userMadeChanges)
                return true;

            var result = MessageBox.Show(
                "تغییراتی که ایجاد کرده‌اید ذخیره نشده است.\nآیا می‌خواهید آن‌ها را ذخیره کنید؟",
                "ذخیره تغییرات",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SaveCompany();
                return false; // ذخیره خودش فرم را می‌بندد
            }

            return result == MessageBoxResult.No;
        }

        private void Cancel_Click(object sender, MouseButtonEventArgs e)
        {
            if (!ConfirmCloseWithUnsavedWarning())
                return;

            (Window.GetWindow(this) as MainWindow)?.CloseModal();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmCloseWithUnsavedWarning())
                return;

            (Window.GetWindow(this) as MainWindow)?.CloseModal();
        }

        /// <summary>Safe wrapper for RefreshGridAsync with error handling at call site.</summary>
        private async Task RefreshListViewSafeAsync(CompanyListView listView)
        {
            try
            {
                await listView.RefreshGridAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EditCompanyView] Error in RefreshListViewSafeAsync: {ex}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
