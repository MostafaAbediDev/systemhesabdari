using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Application.Contract.FinancialPeriod;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;

namespace Taadol.Views
{
    /// <summary>
    /// فرم ویرایش دوره مالی — الگوی EditCompanyView:
    /// لود اطلاعات از GetDetails، ذخیره با EditFinancialPeriod،
    /// ردیابی تغییرات ذخیره‌نشده (IUnsavedChangesAware) و رفرش لیست پشت مودال.
    /// </summary>
    public partial class EditFinancialPeriodView : UserControl, INotifyPropertyChanged, IUnsavedChangesAware
    {
        private readonly IFinancialPeriodApplication _financialPeriodApplication;
        private readonly long _periodId;

        private string _periodTitle = "";
        private long _selectedBranchId;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private bool _isCurrentPeriod = true;

        // ردیابی تغییرات واقعی کاربر:
        // تا پایان لود (IsLoading) تغییرات برنامه‌نویسی نادیده گرفته می‌شوند؛
        // بعد از آن هر تغییر = تغییر کاربر → انصراف فقط در این صورت سؤال می‌پرسد.
        private bool _isLoading = true;
        private bool _userMadeChanges;
        private bool _isSaving;

        public ObservableCollection<BranchComboItem> Branches { get; } = new();

        public ICommand SaveCommand { get; }

        public bool HasUnsavedChanges => _userMadeChanges;

        public string PeriodTitle
        {
            get => _periodTitle;
            set { _periodTitle = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public long SelectedBranchId
        {
            get => _selectedBranchId;
            set { _selectedBranchId = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public DateTime? StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public bool IsCurrentPeriod
        {
            get => _isCurrentPeriod;
            set { _isCurrentPeriod = value; OnPropertyChanged(); MarkUserChange(); }
        }

        public EditFinancialPeriodView(long periodId)
        {
            InitializeComponent();

            _periodId = periodId;
            _financialPeriodApplication = App.ServiceProvider.GetRequiredService<IFinancialPeriodApplication>();

            SaveCommand = new RelayCommand(SavePeriod);
            DataContext = this;

            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // جزئیات دوره و وضعیت فعال به‌صورت موازی لود می‌شوند
                var detailsTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IFinancialPeriodApplication>();
                    return app.GetDetails(_periodId);
                });

                var periodsTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IFinancialPeriodApplication>();
                    return app.GetFinancialPeriods();
                });

                var branchesTask = Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IBranchApplication>();
                    return app.GetBranches().Where(b => b.IsActive)
                        .Select(b => new BranchComboItem { Id = b.Id, Title = b.Title }).ToList();
                });

                var details = await detailsTask;
                if (details == null)
                {
                    ToastManager.Error("دوره مالی پیدا نشد.");
                    return;
                }

                PeriodTitle = details.Title ?? "";

                // GetDetails وضعیت فعال را برنمی‌گرداند؛ از لیست دوره‌ها می‌خوانیم
                var periodVm = (await periodsTask).FirstOrDefault(p => p.Id == _periodId);
                IsCurrentPeriod = periodVm?.IsActive ?? true;

                if (details.BranchId > 0)
                    SelectedBranchId = details.BranchId;

                if (details.StartDate != default)
                {
                    StartDate = details.StartDate;
                    if (StartDatePicker != null)
                        StartDatePicker.SelectedDate = details.StartDate;
                }

                if (details.EndDate != default)
                {
                    EndDate = details.EndDate;
                    if (EndDatePicker != null)
                        EndDatePicker.SelectedDate = details.EndDate;
                }

                var branches = await branchesTask;
                Branches.Clear();
                foreach (var b in branches)
                    Branches.Add(b);

                // اگر شعبه‌ای که دوره به آن تعلق دارد غیرفعال شده، همچنان در لیست باشد
                if (SelectedBranchId > 0 && Branches.All(b => b.Id != SelectedBranchId))
                {
                    Branches.Insert(0, new BranchComboItem { Id = details.BranchId, Title = "—" });
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

        private void StartDatePicker_DateChanged(object sender, RoutedEventArgs e)
        {
            var picker = sender as PersianDatePickerControl;
            if (picker == null) return;

            // اگر فیلدهای تاریخ پاک شوند مقدار null می‌شود تا ذخیره با تاریخ قبلی رخ ندهد
            StartDate = picker.SelectedDate;
        }

        private void EndDatePicker_DateChanged(object sender, RoutedEventArgs e)
        {
            var picker = sender as PersianDatePickerControl;
            if (picker == null) return;

            // اگر فیلدهای تاریخ پاک شوند مقدار null می‌شود تا ذخیره با تاریخ قبلی رخ ندهد
            EndDate = picker.SelectedDate;
        }

        private void SavePeriod()
        {
            if (_isSaving) return;

            // اعتبارسنجی اول — دکمه فقط وقتی وارد حالت «در حال ذخیره» می‌شود که فرم معتبر باشد
            if (string.IsNullOrWhiteSpace(PeriodTitle))
            {
                ToastManager.Warning("عنوان دوره مالی را وارد کنید.");
                return;
            }

            if (SelectedBranchId <= 0)
            {
                ToastManager.Warning("لطفاً شعبه را انتخاب کنید.");
                return;
            }

            if (!StartDate.HasValue)
            {
                ToastManager.Warning("تاریخ شروع دوره را انتخاب کنید.");
                return;
            }

            if (!EndDate.HasValue)
            {
                ToastManager.Warning("تاریخ پایان دوره را انتخاب کنید.");
                return;
            }

            if (EndDate.Value < StartDate.Value)
            {
                ToastManager.Warning("تاریخ پایان نمی‌تواند قبل از تاریخ شروع باشد.");
                return;
            }

            try
            {
                // چک‌های سمت دیتابیس (همپوشانی و عنوان تکراری) — خودِ دوره مستثنی است.
                // این چک‌ها قبل از تغییر دکمه انجام می‌شوند تا «در حال ذخیره» با خطا همزمان دیده نشود.
                using (var checkConnection = new SqlConnection(App.ConnectionString))
                {
                    checkConnection.Open();

                    using (var overlapCommand = new SqlCommand(@"
                        SELECT COUNT(1) FROM FinancialPeriods
                        WHERE Id != @Id AND IsDeleted = 0
                          AND @StartDate <= EndDate AND @EndDate >= StartDate;
                    ", checkConnection))
                    {
                        overlapCommand.Parameters.AddWithValue("@Id", _periodId);
                        overlapCommand.Parameters.AddWithValue("@StartDate", StartDate.Value);
                        overlapCommand.Parameters.AddWithValue("@EndDate", EndDate.Value);
                        var overlapCount = Convert.ToInt32(overlapCommand.ExecuteScalar());
                        if (overlapCount > 0)
                        {
                            ToastManager.Warning("بازه زمانی دوره مالی با یک دوره مالی دیگر همپوشانی دارد.");
                            return;
                        }
                    }

                    using (var titleCommand = new SqlCommand(@"
                        SELECT COUNT(1) FROM FinancialPeriods
                        WHERE Id != @Id AND IsDeleted = 0 AND Title = @Title;
                    ", checkConnection))
                    {
                        titleCommand.Parameters.AddWithValue("@Id", _periodId);
                        titleCommand.Parameters.AddWithValue("@Title", PeriodTitle.Trim());
                        var titleCount = Convert.ToInt32(titleCommand.ExecuteScalar());
                        if (titleCount > 0)
                        {
                            ToastManager.Warning("نام دوره مالی تکراری است.");
                            return;
                        }
                    }
                }

                _isSaving = true;
                if (SaveButton != null)
                {
                    SaveButton.IsEnabled = false;
                    SaveButton.Text = "در حال ذخیره...";
                }

                using var connection = new SqlConnection(App.ConnectionString);
                connection.Open();
                using var transaction = connection.BeginTransaction();

                // اگر دوره جاری است، بقیه دوره‌های همان شعبه غیرفعال شوند
                if (IsCurrentPeriod)
                {
                    using var deactivateCommand = new SqlCommand(@"
                        UPDATE FinancialPeriods
                        SET IsActive = 0
                        WHERE BranchId = @BranchId AND IsDeleted = 0;
                    ", connection, transaction);
                    deactivateCommand.Parameters.AddWithValue("@BranchId", SelectedBranchId);
                    deactivateCommand.ExecuteNonQuery();
                }

                using var updateCommand = new SqlCommand(@"
                    UPDATE FinancialPeriods
                    SET Title = @Title,
                        StartDate = @StartDate,
                        EndDate = @EndDate,
                        BranchId = @BranchId,
                        IsActive = @IsActive
                    WHERE Id = @Id AND IsDeleted = 0;
                ", connection, transaction);
                updateCommand.Parameters.AddWithValue("@Id", _periodId);
                updateCommand.Parameters.AddWithValue("@Title", PeriodTitle.Trim());
                updateCommand.Parameters.AddWithValue("@StartDate", StartDate.Value);
                updateCommand.Parameters.AddWithValue("@EndDate", EndDate.Value);
                updateCommand.Parameters.AddWithValue("@BranchId", SelectedBranchId);
                updateCommand.Parameters.AddWithValue("@IsActive", IsCurrentPeriod);
                updateCommand.ExecuteNonQuery();

                transaction.Commit();

                ToastManager.Success("ویرایش دوره مالی با موفقیت انجام شد.");

                // کش سال‌های مالی سایدبار را بی‌اعتبار کن تا تغییرات فوراً دیده شود
                Taadol.Controls.YearSelectorControl.InvalidateCache();
                if ((Window.GetWindow(this) as MainWindow)?.Sidebar?.YearSelector is { } yearSelector)
                    _ = RefreshYearSelectorSafeAsync(yearSelector);

                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.CloseModal();

                // اگر پشت مودال لیست دوره‌های مالی بود همان لیست درجا رفرش می‌شود (بدون از دست رفتن State)
                if (mainWindow?.MainContent.Content is FinancialPeriodListView listView)
                    _ = RefreshListViewSafeAsync(listView);
                else
                    mainWindow?.NavigateTo("financial_period");
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
                SavePeriod();
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

        /// <summary>Safe wrapper for yearSelector.RefreshAsync with error handling at call site.</summary>
        private async Task RefreshYearSelectorSafeAsync(Taadol.Controls.YearSelectorControl yearSelector)
        {
            try
            {
                await yearSelector.RefreshAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EditFinancialPeriodView] Error in RefreshYearSelectorSafeAsync: {ex}");
            }
        }

        /// <summary>Safe wrapper for RefreshGridAsync with error handling at call site.</summary>
        private async Task RefreshListViewSafeAsync(FinancialPeriodListView listView)
        {
            try
            {
                await listView.RefreshGridAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EditFinancialPeriodView] Error in RefreshListViewSafeAsync: {ex}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
