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
using GeneralInfoManagement.Application.Contract.FinancialPeriod;
using Microsoft.Extensions.DependencyInjection;
using Taadol.Controls;
using Taadol.Helpers;

namespace Taadol.Views
{
    public partial class NewFinancialPeriodView : UserControl, INotifyPropertyChanged
    {
        private bool _isSaving;
        private bool _userMadeChanges;
        private CancellationTokenSource _saveCts = new();
        private bool _isLoading = true;
        private CancellationTokenSource _loadCts = new();
        private readonly IBranchApplication _branchApplication;
        private readonly IFinancialPeriodApplication _financialPeriodApplication;

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
            _financialPeriodApplication = App.ServiceProvider.GetRequiredService<IFinancialPeriodApplication>();

            SaveCommand = new FinancialPeriodSaveCommand(async () => await SaveFinancialPeriodAsync());

            DataContext = this;

            Loaded += NewFinancialPeriodView_Loaded;
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

        private async void NewFinancialPeriodView_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loadCts == null || _loadCts.IsCancellationRequested)
                return;

            try
            {
                await LoadBranchesAsync();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewFinancialPeriodView] Load branches error: {ex}");
                ToastManager.Error("خطا در بارگذاری شعبه‌ها");
            }

            if (_loadCts?.IsCancellationRequested != true)
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

                token.ThrowIfCancellationRequested();
                Branches.Clear();
                foreach (var b in branches)
                    Branches.Add(b);

                if (!token.IsCancellationRequested && Branches.Count > 0)
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
            if (_saveCts == null || _saveCts.IsCancellationRequested) return;

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

                var saveToken = _saveCts.Token;
                var dbMessage = await Task.Run(() =>
                {
                    using var scope = App.ServiceProvider.CreateScope();
                    var app = scope.ServiceProvider.GetRequiredService<IFinancialPeriodApplication>();

                    var result = app.Create(new CreateFinancialPeriod
                    {
                        Title = PeriodTitle.Trim(),
                        StartDate = StartDate.Value,
                        EndDate = EndDate.Value,
                        BranchId = SelectedBranchId
                    });

                    if (!result.IsSucceeded)
                        return result.Message;

                    if (IsCurrentPeriod)
                    {

                        var existing = app.GetFinancialPeriods()
                            .Where(p => p.BranchId == SelectedBranchId && p.IsActive && !p.IsDeleted)
                            .ToList();
                        foreach (var p in existing)
                            app.Deactivate(p.Id);

                        var created = app.GetFinancialPeriods()
                            .FirstOrDefault(p => p.Title == PeriodTitle.Trim() && p.BranchId == SelectedBranchId && !p.IsDeleted);
                        if (created != null)
                            app.Activate(created.Id);
                    }

                    return (string)null;
                }, saveToken);

                if (dbMessage != null)
                {
                    ToastManager.Warning(dbMessage);
                    return;
                }

                ToastManager.Success("دوره مالی با موفقیت ثبت شد.");

                Taadol.Controls.YearSelectorControl.InvalidateCache();
                if ((Window.GetWindow(this) as MainWindow)?.Sidebar?.YearSelector is { } yearSelector)
                    _ = RefreshYearSelectorSafeAsync(yearSelector);

                ClearForm();

                MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
            }
            catch (OperationCanceledException) when (_saveCts?.IsCancellationRequested == true)
            {
                System.Diagnostics.Debug.WriteLine("[NewFinancialPeriodView] Save period was cancelled");
            }
            catch (Exception ex)
            {
                if (_saveCts?.IsCancellationRequested == true) return;
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
            mainWindow?.NavigateTo(NavKeys.FinancialPeriod);
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

        private async Task RefreshYearSelectorSafeAsync(Taadol.Controls.YearSelectorControl yearSelector)
        {
            try
            {
                await yearSelector.RefreshAsync();
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NewFinancialPeriodView] Error in RefreshYearSelectorSafeAsync: {ex}");
                ToastManager.Error("خطا در بارگذاری اطلاعات. لطفاً اتصال به سرور را بررسی کنید.");
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