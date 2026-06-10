using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
            }
        }

        public long SelectedBranchId
        {
            get => _selectedBranchId;
            set
            {
                _selectedBranchId = value;
                OnPropertyChanged(nameof(SelectedBranchId));
            }
        }

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        public bool IsCurrentPeriod
        {
            get => _isCurrentPeriod;
            set
            {
                _isCurrentPeriod = value;
                OnPropertyChanged(nameof(IsCurrentPeriod));
            }
        }

        public NewFinancialPeriodView()
        {
            InitializeComponent();

            _branchApplication = App.ServiceProvider.GetRequiredService<IBranchApplication>();

            SaveCommand = new FinancialPeriodSaveCommand(SaveFinancialPeriod);

            DataContext = this;

            Loaded += NewFinancialPeriodView_Loaded;
        }

        private async void NewFinancialPeriodView_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadBranchesAsync();
        }

        private async Task LoadBranchesAsync()
        {
            try
            {
                var branches = await Task.Run(() =>
                {
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
                });

                Branches = new ObservableCollection<BranchComboItem>(branches);
                OnPropertyChanged(nameof(Branches));

                if (Branches.Count > 0)
                    SelectedBranchId = Branches[0].Id;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطا در لود شعبه‌ها", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveFinancialPeriod()
        {
            if (string.IsNullOrWhiteSpace(PeriodTitle))
            {
                MessageBox.Show("عنوان دوره مالی را وارد کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedBranchId <= 0)
            {
                MessageBox.Show("لطفاً شعبه را انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!StartDate.HasValue)
            {
                MessageBox.Show("تاریخ شروع دوره را انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!EndDate.HasValue)
            {
                MessageBox.Show("تاریخ پایان دوره را انتخاب کنید.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (EndDate.Value < StartDate.Value)
            {
                MessageBox.Show("تاریخ پایان نمی‌تواند قبل از تاریخ شروع باشد.", "خطا", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
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

                MessageBox.Show("دوره مالی با موفقیت ثبت شد.", "موفق", MessageBoxButton.OK, MessageBoxImage.Information);

                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.GetBaseException().Message,
                    "خطا در ثبت دوره مالی",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            PeriodTitle = "";
            StartDate = null;
            EndDate = null;
            IsCurrentPeriod = true;
        }

        private void DatePicker_DateChanged(object sender, RoutedEventArgs e)
        {
            var picker = sender as PersianDatePickerControl;

            if (picker == null || !picker.SelectedDate.HasValue)
                return;

            if (picker == StartDatePicker)
                StartDate = picker.SelectedDate.Value;

            if (picker == EndDatePicker)
                EndDate = picker.SelectedDate.Value;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void IsCurrentPeriod_Changed(object sender, RoutedEventArgs e)
        {
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