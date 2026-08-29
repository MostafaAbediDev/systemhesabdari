using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Application.Contract.FinancialPeriod;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;
using Taadol.Helpers;

namespace Taadol.Views
{
    /// <summary>
    /// فرم ویرایش دوره مالی — الگوی EditCompanyView:
    /// لود اطلاعات از GetDetails، ذخیره با EditFinancialPeriod،
    /// ردیابی تغییرات ذخیره‌نشده (IUnsavedChangesAware) و رفرش لیست پشت مودال.
    /// </summary>
    public partial class EditFinancialPeriodView : UserControl, INotifyPropertyChanged, IUnsavedChangesAware
    {
        private CancellationTokenSource _loadCts = new();
        private CancellationTokenSource _saveCts = new();
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

            SaveCommand = new RelayCommand(async () => await SavePeriodAsync());
            DataContext = this;

            Loaded += OnLoaded;
            this.Unloaded += OnViewUnloaded;
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e)
        {
            CancelAndDispose(ref _loadCts);
            CancelAndDispose(ref _saveCts);
            this.Unloaded -= OnViewUnloaded;
        }

        private static void CancelAndDispose(ref CancellationTokenSource cts)
        {
            var current = Interlocked.Exchange(ref cts, null);
            if (current == null) return;
            try { current.Cancel(); } catch (ObjectDisposedException) { }
            current.Dispose();
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_loadCts == null || _loadCts.IsCancellationRequested)
                return;

            try
            {
                var token = _loadCts.Token;

                // جزئیات دوره و وضعیت فعال به‌صورت موازی لود می‌شوند
                var detailsTask = Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IFinancialPeriodApplication>();
                    return app.GetDetails(_periodId);
                }, token);

                var periodsTask = Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IFinancialPeriodApplication>();
                    return app.GetFinancialPeriods();
                }, token);

                var branchesTask = Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IBranchApplication>();
                    return app.GetBranches().Where(b => b.IsActive)
                        .Select(b => new BranchComboItem { Id = b.Id, Title = b.Title }).ToList();
                }, token);

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

                token.ThrowIfCancellationRequested();
                var branches = await branchesTask;
                token.ThrowIfCancellationRequested();
                Branches.Clear();
                foreach (var b in branches)
                    Branches.Add(b);

                // اگر شعبه‌ای که دوره به آن تعلق دارد غیرفعال شده، همچنان در لیست باشد
                token.ThrowIfCancellationRequested();
                if (SelectedBranchId > 0 && Branches.All(b => b.Id != SelectedBranchId))
                {
                    Branches.Insert(0, new BranchComboItem { Id = details.BranchId, Title = "—" });
                }

                // لود کامل شد — از این به بعد هر تغییری = تغییر کاربر
                _isLoading = false;
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[EditFinancialPeriodView] OnLoaded was cancelled");
            }
            catch (Exception ex)
            {
                if (_loadCts?.IsCancellationRequested != true)
                    _isLoading = false;
                if (_loadCts?.IsCancellationRequested != true)
                {
                    System.Diagnostics.Debug.WriteLine($"[EditFinancialPeriodView] Load info error: {ex}");
                    ToastManager.Error("خطا در لود اطلاعات");
                }
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

        private async Task SavePeriodAsync()
        {
            if (_isSaving) return;
            if (_saveCts == null || _saveCts.IsCancellationRequested) return;

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

            _isSaving = true;
            if (SaveButton != null)
            {
                SaveButton.IsEnabled = false;
                SaveButton.Text = "در حال ذخیره...";
            }

            try
            {
                // ذخیره از طریق سرویس Application روی ترد پس‌زمینه اجرا می‌شود تا UI فریز نشود.
                // سرویس بک‌اند خودش چک همپوشانی و عنوان تکراری را انجام می‌دهد.
                // خروجی null یعنی موفق؛ غیر null پیام هشدار است.
                var saveToken = _saveCts.Token;
                var dbMessage = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IFinancialPeriodApplication>();

                    var result = app.Edit(new EditFinancialPeriod
                    {
                        Id = _periodId,
                        Title = PeriodTitle.Trim(),
                        StartDate = StartDate.Value,
                        EndDate = EndDate.Value,
                        BranchId = SelectedBranchId
                    });

                    if (!result.IsSucceeded)
                        return result.Message;

                    // Edit فیلد IsActive را تغییر نمی‌دهد؛ وضعیت «دوره جاری» را دستی اعمال می‌کنیم
                    if (IsCurrentPeriod)
                    {
                        // بقیه دوره‌های فعال همان شعبه غیرفعال شوند (به‌جز خودِ این دوره)
                        var others = app.GetFinancialPeriods()
                            .Where(p => p.Id != _periodId && p.BranchId == SelectedBranchId && p.IsActive && !p.IsDeleted)
                            .ToList();
                        foreach (var p in others)
                            app.Deactivate(p.Id);

                        app.Activate(_periodId);
                    }
                    else
                    {
                        app.Deactivate(_periodId);
                    }

                    return (string)null;
                }, saveToken);

                if (dbMessage != null)
                {
                    ToastManager.Warning(dbMessage);
                    return;
                }

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
                    mainWindow?.NavigateTo(NavKeys.FinancialPeriod);
            }
            catch (OperationCanceledException) when (_saveCts?.IsCancellationRequested == true)
            {
                System.Diagnostics.Debug.WriteLine("[EditFinancialPeriodView] Edit was cancelled");
            }
            catch (Exception ex)
            {
                if (_saveCts?.IsCancellationRequested == true) return;
                System.Diagnostics.Debug.WriteLine($"[EditFinancialPeriodView] Edit error: {ex}");
                ToastManager.Error("خطا در ویرایش");
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
                _ = SavePeriodAsync();
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
            catch (OperationCanceledException)
            {
                // Cancellation is expected when the view is closed or superseded.
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EditFinancialPeriodView] Error in RefreshYearSelectorSafeAsync: {ex}");
                ToastManager.Error("خطا در بارگذاری اطلاعات. لطفاً اتصال به سرور را بررسی کنید.");
            }
        }

        /// <summary>Safe wrapper for RefreshGridAsync with error handling at call site.</summary>
        private async Task RefreshListViewSafeAsync(FinancialPeriodListView listView)
        {
            try
            {
                await listView.RefreshGridAsync();
            }
            catch (OperationCanceledException)
            {
                // Cancellation is expected when the view is closed or superseded.
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[EditFinancialPeriodView] Error in RefreshListViewSafeAsync: {ex}");
                ToastManager.Error("خطا در بارگذاری اطلاعات. لطفاً اتصال به سرور را بررسی کنید.");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
