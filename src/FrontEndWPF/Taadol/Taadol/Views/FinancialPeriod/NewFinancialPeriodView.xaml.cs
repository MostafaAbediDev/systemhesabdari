using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GeneralInfoManagement.Application.Contract.Branches;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;

namespace Taadol.Views
{
    public partial class NewFinancialPeriodView : UserControl, INotifyPropertyChanged
    {
        private bool _isSaving;
        private bool _userMadeChanges;
        private bool _isLoading = true;
        private CancellationTokenSource _loadCts = new();
        private readonly IBranchApplication _branchApplication;

        private string _periodTitle;
        private long _selectedBranchId;
        private DateTime? _startDate;
        private DateTime? _endDate;
        private bool _isCurrentPeriod = true;

        public ObservableCollection<BranchComboItem> Branches { get; set; } = new ObservableCollection<BranchComboItem>();

        public ICommand SaveCommand { get; }

        public string PeriodTitle
        {
            get => _periodTitle;
            set
            {
                _periodTitle = value;
                OnPropertyChanged(nameof(PeriodTitle));
                MarkUserChange();
            }
        }

        public long SelectedBranchId
        {
            get => _selectedBranchId;
            set
            {
                _selectedBranchId = value;
                OnPropertyChanged(nameof(SelectedBranchId));
                MarkUserChange();
            }
        }

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
                MarkUserChange();
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
                MarkUserChange();
            }
        }

        public bool IsCurrentPeriod
        {
            get => _isCurrentPeriod;
            set
            {
                _isCurrentPeriod = value;
                OnPropertyChanged(nameof(IsCurrentPeriod));
                MarkUserChange();
            }
        }

        public NewFinancialPeriodView()
        {
            InitializeComponent();

            _branchApplication = App.ServiceProvider.GetRequiredService<IBranchApplication>();

            SaveCommand = new FinancialPeriodSaveCommand(async () => await SaveFinancialPeriodAsync());

            DataContext = this;

            Loaded += NewFinancialPeriodView_Loaded;
            this.Unloaded += OnViewUnloaded;
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e)
        {
            _loadCts?.Cancel();
            _loadCts?.Dispose();
            _loadCts = null;
            this.Unloaded -= OnViewUnloaded;
        }

        private async void NewFinancialPeriodView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadBranchesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewFinancialPeriodView] Load branches error: {ex}");
                ToastManager.Error("خطا در بارگذاری شعبه‌ها");
            }

            _isLoading = false;
        }

        private async Task LoadBranchesAsync()
        {
            try
            {
                var token = _loadCts?.Token ?? CancellationToken.None;
                var branches = await Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();
                    using var scope = App.ServiceProvider.CreateScope();

                    var branchApplication = scope.ServiceProvider.GetRequiredService<IBranchApplication>();

                    return branchApplication.GetBranches()
                        .Where(x => x.IsActive)
                        .Select(x => new BranchComboItem
                        {
                            Id = x.Id,
                            Title = x.Title
                        })
                        .ToList();
                }, token);

                Branches.Clear();
                foreach (var b in branches)
                    Branches.Add(b);

                if (Branches.Count > 0)
                    SelectedBranchId = Branches[0].Id;
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("[NewFinancialPeriodView] LoadBranchesAsync was cancelled");
            }
            catch (Exception ex)
            {
                if (_loadCts?.IsCancellationRequested != true)
                {
                    System.Diagnostics.Debug.WriteLine($"[NewFinancialPeriodView] Load branches error: {ex}");
                    ToastManager.Error("خطا در لود شعبه‌ها");
                }
            }
        }

        private async Task SaveFinancialPeriodAsync()
        {
            if (_isSaving) return;

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
                SaveButton.ButtonText = "در حال ذخیره...";
            }

            try
            {
                // عملیات دیتابیس روی ترد پس‌زمینه اجرا می‌شود تا UI فریز نشود
                await Task.Run(() =>
                {
                    using var connection = new SqlConnection(App.ConnectionString);
                    connection.Open();

                    using var transaction = connection.BeginTransaction();

                    if (IsCurrentPeriod)
                    {
                        using var deactivateCommand = new SqlCommand(@"
                            UPDATE FinancialPeriods
                            SET IsActive = 0
                            WHERE BranchId = @BranchId
                              AND IsDeleted = 0;
                        ", connection, transaction);

                        deactivateCommand.Parameters.AddWithValue("@BranchId", SelectedBranchId);
                        deactivateCommand.ExecuteNonQuery();
                    }

                    using var insertCommand = new SqlCommand(@"
                        INSERT INTO FinancialPeriods
                        (
                            Title,
                            StartDate,
                            EndDate,
                            BranchId,
                            IsDeleted,
                            IsActive,
                            CreationDate
                        )
                        VALUES
                        (
                            @Title,
                            @StartDate,
                            @EndDate,
                            @BranchId,
                            0,
                            @IsActive,
                            SYSDATETIME()
                        );
                    ", connection, transaction);

                    insertCommand.Parameters.AddWithValue("@Title", PeriodTitle.Trim());
                    insertCommand.Parameters.AddWithValue("@StartDate", StartDate.Value);
                    insertCommand.Parameters.AddWithValue("@EndDate", EndDate.Value);
                    insertCommand.Parameters.AddWithValue("@BranchId", SelectedBranchId);
                    insertCommand.Parameters.AddWithValue("@IsActive", IsCurrentPeriod);

                    insertCommand.ExecuteNonQuery();

                    transaction.Commit();
                });

                ToastManager.Success("دوره مالی با موفقیت ثبت شد.");

                // کش سال‌های مالی سایدبار را بی‌اعتبار کن تا دوره جدید فوراً دیده شود
                Taadol.Controls.YearSelectorControl.InvalidateCache();
                if ((Window.GetWindow(this) as MainWindow)?.Sidebar?.YearSelector is { } yearSelector)
                    _ = RefreshYearSelectorSafeAsync(yearSelector);

                ClearForm();
                // Focus first focusable element (PeriodTitle field)
                MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewFinancialPeriodView] Save period error: {ex}");
                ToastManager.Error("خطا در ثبت دوره مالی");
            }
            finally
            {
                _isSaving = false;
                if (SaveButton != null)
                {
                    SaveButton.IsEnabled = true;
                    SaveButton.ButtonText = "ثبت دوره";
                }
            }
        }

        private void ClearForm()
        {
            PeriodTitle = "";
            StartDate = null;
            EndDate = null;
            IsCurrentPeriod = true;
        }

        private void NavigateToPeriodList()
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            mainWindow?.NavigateTo("financial_period");
        }

        private void MarkUserChange()
        {
            if (_isLoading) return;
            _userMadeChanges = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (_userMadeChanges)
            {
                var result = MessageBox.Show(
                    "تغییراتی که ایجاد کرده‌اید ذخیره نشده است.\nآیا مایل به خروج هستید؟",
                    "خروج",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            NavigateToPeriodList();
        }

        private void HeaderClose_Click(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow == null) return;
            if (mainWindow.ModalContent.Content == this)
                mainWindow.CloseModal();
            else
                mainWindow.CloseCurrentForm();
        }

        private void DatePicker_DateChanged(object sender, RoutedEventArgs e)
        {
            var picker = sender as PersianDatePickerControl;
            if (picker == null) return;

            // اگر فیلدهای تاریخ پاک شوند مقدار null می‌شود تا ذخیره با تاریخ قبلی رخ ندهد
            if (picker == StartDatePicker)
                StartDate = picker.SelectedDate;

            if (picker == EndDatePicker)
                EndDate = picker.SelectedDate;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void IsCurrentPeriod_Changed(object sender, RoutedEventArgs e)
        {
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
                System.Diagnostics.Debug.WriteLine($"[NewFinancialPeriodView] Error in RefreshYearSelectorSafeAsync: {ex}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BranchComboItem
    {
        public long Id { get; set; }
        public string Title { get; set; }
    }

    public class FinancialPeriodSaveCommand : ICommand
    {
        private readonly Action _execute;

        public FinancialPeriodSaveCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        public event EventHandler CanExecuteChanged;
    }
}